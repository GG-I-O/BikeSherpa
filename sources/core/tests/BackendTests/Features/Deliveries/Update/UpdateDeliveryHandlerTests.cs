using Ardalis.Result;
using Ardalis.Specification;
using AutoFixture;
using AutoFixture.AutoMoq;
using AwesomeAssertions;
using FluentValidation;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Services.PricingStrategy;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Services.Repositories;
using Ggio.BikeSherpa.Backend.Features.Deliveries.Update;
using Ggio.DddCore;
using Moq;

namespace BackendTests.Features.Deliveries.Update;

public class UpdateDeliveryHandlerTests
{
     private readonly Mock<IApplicationTransaction> _mockTransaction = new();
     private readonly Mock<IPricingStrategyService> _mockPricingStrategyService = new();
     private readonly Mock<IReadRepository<Delivery>> _mockDeliveryRepository = new();
     private readonly Mock<IUrgencyRepository> _mockUrgencyRepository = new();
     private readonly Mock<IDeliveryZoneRepository> _mockDeliveryZoneRepository = new();
     private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());
     private readonly UpdateDeliveryCommand _mockCommand;
     private readonly Delivery _mockDelivery;

     public UpdateDeliveryHandlerTests()
     {
          var urgencies = new List<Urgency>(_fixture.CreateMany<Urgency>(2));
          _mockUrgencyRepository
               .Setup(x => x.Urgencies)
               .Returns(urgencies);

          _mockDeliveryZoneRepository
               .Setup(x => x.DeliveryZones)
               .Returns(new List<DeliveryZone>(_fixture.CreateMany<DeliveryZone>(2)));

          _mockCommand = _fixture.Build<UpdateDeliveryCommand>()
               .With(c => c.Urgency, urgencies[0].Name)
               .Create();

          _mockDelivery = _fixture.Build<Delivery>()
               .With(c => c.Id, _mockCommand.Id)
               .Create();

          _mockDeliveryRepository
               .Setup(x => x.FirstOrDefaultAsync(
                    It.IsAny<ISpecification<Delivery>>(),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(_mockDelivery);
     }

     private UpdateDeliveryHandler CreateSut()
     {
          var validator = new UpdateDeliveryCommandValidator(_mockDeliveryRepository.Object, _mockUrgencyRepository.Object);
          return new UpdateDeliveryHandler(_mockDeliveryRepository.Object, validator, _mockTransaction.Object, _mockDeliveryZoneRepository.Object, _mockPricingStrategyService.Object);
     }

     private void SetupRepositoryTestingIfReportIdExists(bool doesReportIdExist)
     {
          _mockDeliveryRepository
               .Setup(x => x.AnyAsync(
                    It.Is<ISpecification<Delivery>>(spec => spec.GetType().Name == "DeliveryByReportIdSpecification"),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(doesReportIdExist);
     }

     private void VerifyTransactionCommittedOnce()
     {
          _mockTransaction.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
     }

     [Fact]
     public async Task Handle_ShouldUpdateDeliveryAndReturnOk_WhenCommandIsValid()
     {
          // Arrange
          SetupRepositoryTestingIfReportIdExists(false);

          var sut = CreateSut();

          // Act
          var result = await sut.Handle(_mockCommand, CancellationToken.None);

          // Assert
          result.IsSuccess.Should().BeTrue();
          VerifyTransactionCommittedOnce();
     }

     [Fact]
     public async Task Handle_ShouldUpdateDeliveryWithCorrectInformation()
     {
          // Arrange
          SetupRepositoryTestingIfReportIdExists(false);

          _mockDelivery.InsulatedBox = !_mockCommand.InsulatedBox;
          _mockDelivery.Status = _mockCommand.StatusEnum == DeliveryStatusEnum.Pending
               ? DeliveryStatusEnum.Completed
               : DeliveryStatusEnum.Pending;

          var originalCode = _mockDelivery.Code;
          var originalCustomerId = _mockDelivery.CustomerId;
          var originalDiscount = _mockDelivery.Discount;
          var originalPackingSize = _mockDelivery.PackingSize;
          var originalContractDate = _mockDelivery.ContractDate;
          var originalStartDate = _mockDelivery.StartDate;
          var originalStatus = _mockDelivery.Status;
          var originalSteps = _mockDelivery.Steps.ToList();
          var originalInsulatedBox = _mockDelivery.InsulatedBox;
          var originalDetails = _mockDelivery.Details;
          var originalReportId = _mockDelivery.ReportId;
          var originalPricingStrategy = _mockDelivery.PricingStrategy;
          var originalUrgency = _mockDelivery.Urgency;

          var sut = CreateSut();

          // Act
          var result = await sut.Handle(_mockCommand, CancellationToken.None);

          // Assert
          result.IsSuccess.Should().BeTrue();

          // Verify the delivery was updated with command values
          _mockDelivery.Code.Should().Be(_mockCommand.Code);
          _mockDelivery.CustomerId.Should().Be(_mockCommand.CustomerId);
          _mockDelivery.Discount.Should().Be(_mockCommand.Discount);
          _mockDelivery.PackingSize.Should().Be(_mockCommand.PackingSize);
          _mockDelivery.ContractDate.Should().Be(_mockCommand.ContractDate);
          _mockDelivery.StartDate.Should().Be(_mockCommand.StartDate);
          _mockDelivery.Urgency.Should().Be(_mockCommand.Urgency);
          _mockDelivery.Details.Should().BeEquivalentTo(_mockCommand.Details);
          _mockDelivery.Steps.Should().BeEquivalentTo(_mockCommand.Steps);
          _mockDelivery.InsulatedBox.Should().Be(_mockCommand.InsulatedBox);
          _mockDelivery.ReportId.Should().Be(_mockCommand.ReportId);

          // Verify original values were different (ensures update actually happened)
          _mockDelivery.Code.Should().NotBe(originalCode);
          _mockDelivery.CustomerId.Should().NotBe(originalCustomerId);
          _mockDelivery.Discount.Should().NotBe(originalDiscount);
          _mockDelivery.PackingSize.Should().NotBe(originalPackingSize);
          _mockDelivery.ContractDate.Should().NotBe(originalContractDate);
          _mockDelivery.StartDate.Should().NotBe(originalStartDate);
          _mockDelivery.Status.Should().NotBe(originalStatus);
          _mockDelivery.Steps.Should().NotBeEquivalentTo(originalSteps);
          _mockDelivery.InsulatedBox.Should().NotBe(originalInsulatedBox);
          _mockDelivery.Details.Should().NotBeEquivalentTo(originalDetails);
          _mockDelivery.ReportId.Should().NotBe(originalReportId);
          _mockDelivery.PricingStrategy.Should().NotBe(originalPricingStrategy);
          _mockDelivery.Urgency.Should().NotBe(originalUrgency);
     }

     [Fact]
     public async Task Handle_ShouldReturnNotFoundIfIdDoesNotExist()
     {
          // Arrange
          SetupRepositoryTestingIfReportIdExists(false);
          _mockDeliveryRepository
               .Setup(x => x.FirstOrDefaultAsync(
                    It.IsAny<ISpecification<Delivery>>(),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(null as Delivery);
          var sut = CreateSut();

          // Act
          var result = await sut.Handle(_mockCommand, CancellationToken.None);

          // Assert
          result.IsSuccess.Should().BeFalse();
          result.IsNotFound().Should().BeTrue();
          _mockTransaction.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
     }

     [Fact]
     public async Task Handle_ShouldThrowValidationException_WhenReportIdIsEmpty()
     {
          // Arrange
          var commandWithEmptyReportId = _mockCommand with { ReportId = "" };
          SetupRepositoryTestingIfReportIdExists(false);
          var sut = CreateSut();

          // Act & Assert
          await Assert.ThrowsAsync<ValidationException>(() =>
               sut.Handle(commandWithEmptyReportId, CancellationToken.None).AsTask());

          _mockTransaction.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
     }

     [Fact]
     public async Task Handle_ShouldThrowValidationException_WhenReportIdAlreadyExists()
     {
          // Arrange
          SetupRepositoryTestingIfReportIdExists(true);
          var sut = CreateSut();

          // Act & Assert
          await Assert.ThrowsAsync<ValidationException>(() =>
               sut.Handle(_mockCommand, CancellationToken.None).AsTask());

          _mockTransaction.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
     }

     [Fact]
     public async Task Handle_ShouldRespectCancellationToken()
     {
          // Arrange
          var cancellationTokenSource = new CancellationTokenSource();
          await cancellationTokenSource.CancelAsync();
          SetupRepositoryTestingIfReportIdExists(false);
          var sut = CreateSut();

          // Act & Assert
          await Assert.ThrowsAnyAsync<Exception>(() =>
               sut.Handle(_mockCommand, cancellationTokenSource.Token).AsTask());
     }
}
