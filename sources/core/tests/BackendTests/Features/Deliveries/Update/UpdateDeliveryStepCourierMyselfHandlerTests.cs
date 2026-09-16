using Ardalis.Result;
using Ardalis.Specification;
using AutoFixture;
using AutoFixture.AutoMoq;
using AwesomeAssertions;
using FluentValidation;
using Ggio.BikeSherpa.Backend.Domain.CourierAggregate;
using Ggio.BikeSherpa.Backend.Domain.CourierAggregate.Specification;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Specification;
using Ggio.BikeSherpa.Backend.Features.Deliveries.Update;
using Ggio.DddCore;
using Moq;

namespace BackendTests.Features.Deliveries.Update;

public class UpdateDeliveryStepCourierMyselfHandlerTests
{
     private readonly Mock<IReadRepository<Delivery>> _mockDeliveryRepository = new();
     private readonly Mock<IReadRepository<Courier>> _mockCourierRepository = new();
     private readonly Mock<IApplicationTransaction> _mockTransaction = new();
     private readonly Mock<IValidator<UpdateDeliveryStepCourierMyselfCommand>> _mockValidator = new();
     private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());

     private readonly Delivery _delivery;
     private readonly Courier _courier;
     private readonly Guid _deliveryId;
     private readonly Guid _stepId;
     private readonly string _courierEmail = "courier@example.com";

     public UpdateDeliveryStepCourierMyselfHandlerTests()
     {
          _deliveryId = Guid.NewGuid();

          _delivery = _fixture.Build<Delivery>()
               .With(d => d.Id, _deliveryId)
               .With(d => d.Steps, [])
               .Create();

          var steps = _fixture.Build<DeliveryStep>()
               .With(s => s.ParentDelivery, _delivery)
               .CreateMany(3)
               .ToList();
          _delivery.Steps.AddRange(steps);

          _stepId = _delivery.Steps.First().Id;

          _courier = _fixture.Build<Courier>()
               .With(c => c.Email, _courierEmail)
               .Create();

          _mockValidator
               .Setup(x => x.ValidateAsync(
                    It.IsAny<UpdateDeliveryStepCourierMyselfCommand>(),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(new FluentValidation.Results.ValidationResult());

          _mockDeliveryRepository
               .Setup(x => x.FirstOrDefaultAsync(
                    It.Is<ISpecification<Delivery>>(s => s is DeliveryByIdSpecification),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(_delivery);

          _mockCourierRepository
               .Setup(x => x.FirstOrDefaultAsync(
                    It.Is<ISpecification<Courier>>(s => s is CourierByEmailSpecification),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(_courier);
     }

     private UpdateDeliveryStepCourierMyselfHandler CreateSut()
     {
          return new UpdateDeliveryStepCourierMyselfHandler(
               _mockDeliveryRepository.Object,
               _mockCourierRepository.Object,
               _mockValidator.Object,
               _mockTransaction.Object
          );
     }

     [Fact]
     public async Task Handle_ShouldReturnSuccess_WhenRequestIsValid()
     {
          // Arrange
          var sut = CreateSut();
          var command = new UpdateDeliveryStepCourierMyselfCommand(_deliveryId, _stepId, _courierEmail);

          // Act
          var result = await sut.Handle(command, CancellationToken.None);

          // Assert
          result.IsSuccess.Should().BeTrue();

          var updatedStep = _delivery.Steps.First(s => s.Id == _stepId);
          updatedStep.CourierId.Should().Be(_courier.Id);

          _mockTransaction.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
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
          var command = new UpdateDeliveryStepCourierMyselfCommand(_deliveryId, _stepId, _courierEmail);

          // Act
          var result = await sut.Handle(command, CancellationToken.None);

          // Assert
          result.IsSuccess.Should().BeFalse();
          result.IsNotFound().Should().BeTrue();

          _mockTransaction.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
     }

     [Fact]
     public async Task Handle_ShouldReturnNotFound_WhenCourierDoesNotExist()
     {
          // Arrange
          _mockCourierRepository
               .Setup(x => x.FirstOrDefaultAsync(
                    It.Is<ISpecification<Courier>>(s => s is CourierByEmailSpecification),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(null as Courier);

          var sut = CreateSut();
          var command = new UpdateDeliveryStepCourierMyselfCommand(_deliveryId, _stepId, "unknown@example.com");

          // Act
          var result = await sut.Handle(command, CancellationToken.None);

          // Assert
          result.IsSuccess.Should().BeFalse();
          result.IsNotFound().Should().BeTrue();

          _mockTransaction.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
     }
}