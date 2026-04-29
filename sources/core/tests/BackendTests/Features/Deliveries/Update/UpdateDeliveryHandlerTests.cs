using Ardalis.Result;
using Ardalis.Specification;
using AutoFixture;
using AutoFixture.AutoMoq;
using AwesomeAssertions;
using FluentValidation;
using FluentValidation.Results;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Services.PricingStrategy;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Services.Repositories;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Specification;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.SPI;
using Ggio.BikeSherpa.Backend.Features.Deliveries.Model;
using Ggio.BikeSherpa.Backend.Features.Deliveries.Update;
using Ggio.DddCore;
using Moq;

namespace BackendTests.Features.Deliveries.Update;

public class UpdateDeliveryHandlerTests
{
     private readonly Mock<IReadRepository<Delivery>> _mockDeliveryRepository = new();
     private readonly Mock<IValidator<UpdateDeliveryCommand>> _mockValidator = new();
     private readonly Mock<IApplicationTransaction> _mockTransaction = new();
     private readonly Mock<IDeliveryZoneRepository> _mockDeliveryZoneRepository = new();
     private readonly Mock<IPricingStrategyService> _mockPricingStrategyService = new();
     private readonly Mock<IItinerarySpi> _mockItineraryService = new();
     private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());

     private readonly DeliveryZone _deliveryZone;
     private readonly Delivery _delivery;
     private readonly UpdateDeliveryCommand _command;

     public UpdateDeliveryHandlerTests()
     {
          _deliveryZone = _fixture.Create<DeliveryZone>();

          _mockDeliveryZoneRepository
               .Setup(x => x.GetByAddress(It.IsAny<string>()))
               .Returns(_deliveryZone);

          _mockPricingStrategyService
               .Setup(x => x.CalculateDeliveryPriceWithoutVat(It.IsAny<Delivery>()))
               .Returns(123.45);

          _mockItineraryService
               .Setup(x => x.GetItineraryInfoAsync(
                    It.IsAny<GeoPoint>(),
                    It.IsAny<GeoPoint>(),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(new ItineraryResult(10, 20));

          _mockValidator
               .Setup(x => x.ValidateAsync(
                    It.IsAny<UpdateDeliveryCommand>(),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(new ValidationResult());

          var firstStep = _fixture.Build<DeliveryStep>()
               .With(s => s.Order, 1)
               .With(s => s.StepType, StepType.Pickup)
               .With(s => s.StepZone, _deliveryZone)
               .With(s => s.EstimatedDeliveryDate, DateTimeOffset.UtcNow.AddHours(1))
               .Create();

          var secondStep = _fixture.Build<DeliveryStep>()
               .With(s => s.Order, 2)
               .With(s => s.StepType, StepType.Dropoff)
               .With(s => s.StepZone, _deliveryZone)
               .With(s => s.EstimatedDeliveryDate, DateTimeOffset.UtcNow.AddHours(2))
               .Create();

          _delivery = _fixture.Build<Delivery>()
               .With(d => d.Steps, [firstStep, secondStep])
               .Create();

          var commandSteps = _delivery.Steps
               .Select(step => _fixture.Build<DeliveryStepCrud>()
                    .With(s => s.Id, step.Id)
                    .With(s => s.StepType, step.StepType)
                    .With(s => s.Order, step.Order)
                    .With(s => s.StepAddress, step.StepAddress)
                    .With(s => s.StepZone, _deliveryZone)
                    .With(s => s.EstimatedDeliveryDate, step.EstimatedDeliveryDate)
                    .Create())
               .ToList();

          _command = _fixture.Build<UpdateDeliveryCommand>()
               .With(c => c.Id, _delivery.Id)
               .With(c => c.Steps, commandSteps)
               .With(c => c.TotalPrice, 100)
               .With(c => c.Discount, 10)
               .With(c => c.ReportId, "REPORT-001")
               .With(c => c.Details, ["detail-1", "detail-2"])
               .Create();

          _mockDeliveryRepository
               .Setup(x => x.FirstOrDefaultAsync(
                    It.Is<ISpecification<Delivery>>(s => s is DeliveryByIdSpecification),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(_delivery);
     }

     private UpdateDeliveryHandler CreateSut()
     {
          return new UpdateDeliveryHandler(
               _mockDeliveryRepository.Object,
               _mockValidator.Object,
               _mockTransaction.Object,
               _mockDeliveryZoneRepository.Object,
               _mockPricingStrategyService.Object,
               _mockItineraryService.Object);
     }

     [Fact]
     public async Task Handle_ShouldReturnSuccess_WhenCommandIsValid()
     {
          // Arrange
          var sut = CreateSut();

          // Act
          var result = await sut.Handle(_command, CancellationToken.None);

          // Assert
          result.IsSuccess.Should().BeTrue();

          _mockTransaction.Verify(
               x => x.CommitAsync(It.IsAny<CancellationToken>()),
               Times.Once);
     }

     [Fact]
     public async Task Handle_ShouldValidateCommand()
     {
          // Arrange
          var sut = CreateSut();

          // Act
          await sut.Handle(_command, CancellationToken.None);

          // Assert
          _mockValidator.Verify(
               x => x.ValidateAsync(
                    It.Is<ValidationContext<UpdateDeliveryCommand>>(context => context.InstanceToValidate == _command),
                    It.IsAny<CancellationToken>()),
               Times.Once);
     }

     [Fact]
     public async Task Handle_ShouldUpdateDeliveryInformation()
     {
          // Arrange
          var sut = CreateSut();

          // Act
          var result = await sut.Handle(_command, CancellationToken.None);

          // Assert
          result.IsSuccess.Should().BeTrue();

          _delivery.PricingStrategy.Should().Be(_command.PricingStrategy);
          _delivery.Status.Should().Be(_command.Status);
          _delivery.Code.Should().Be(_command.Code);
          _delivery.CustomerId.Should().Be(_command.CustomerId);
          _delivery.Urgency.Should().Be(_command.Urgency);
          _delivery.TotalPrice.Should().Be(123.45);
          _delivery.Discount.Should().Be(_command.Discount);
          _delivery.ReportId.Should().Be(_command.ReportId);
          _delivery.Details.Should().BeEquivalentTo(_command.Details);
          _delivery.PackingSize.Should().Be(_command.PackingSize);
          _delivery.InsulatedBox.Should().Be(_command.InsulatedBox);
          _delivery.ContractDate.Should().Be(_command.ContractDate);
          _delivery.StartDate.Should().Be(_command.StartDate);
     }

     [Fact]
     public async Task Handle_ShouldUpdateExistingSteps()
     {
          // Arrange
          var sut = CreateSut();

          // Act
          var result = await sut.Handle(_command, CancellationToken.None);

          // Assert
          result.IsSuccess.Should().BeTrue();

          _delivery.Steps.Should().HaveCount(_command.Steps.Count);

          for (var index = 0; index < _command.Steps.Count; index++)
          {
               var expectedStep = _command.Steps[index];
               var actualStep = _delivery.Steps.Single(s => s.Id == expectedStep.Id);

               actualStep.StepType.Should().Be(expectedStep.StepType);
               actualStep.Order.Should().Be(index + 1);
               actualStep.Completed.Should().Be(expectedStep.Completed);
               actualStep.StepAddress.Should().BeEquivalentTo(expectedStep.StepAddress);
               actualStep.StepZone.Should().Be(_deliveryZone);
               actualStep.Comment.Should().Be(expectedStep.Comment);

               if (index == 0)
               {
                    actualStep.EstimatedDeliveryDate.Should().Be(expectedStep.EstimatedDeliveryDate);
                    actualStep.Distance.Should().Be(0);
               }
               else
               {
                    actualStep.EstimatedDeliveryDate.Should().Be(
                         _delivery.Steps[index - 1].EstimatedDeliveryDate + TimeSpan.FromMinutes(15));
               }
          }
     }

     [Fact]
     public async Task Handle_ShouldReturnNotFound_WhenDeliveryDoesNotExist()
     {
          // Arrange
          _mockDeliveryRepository
               .Setup(x => x.FirstOrDefaultAsync(
                    It.Is<ISpecification<Delivery>>(s => s is DeliveryByIdSpecification),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(null as Delivery);

          var sut = CreateSut();

          // Act
          var result = await sut.Handle(_command, CancellationToken.None);

          // Assert
          result.IsSuccess.Should().BeFalse();
          result.IsNotFound().Should().BeTrue();

          _mockTransaction.Verify(
               x => x.CommitAsync(It.IsAny<CancellationToken>()),
               Times.Never);
     }
}