using Ardalis.Result;
using Ardalis.Specification;
using AutoFixture;
using AutoFixture.AutoMoq;
using AwesomeAssertions;
using FluentValidation;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Specification;
using Ggio.BikeSherpa.Backend.Features.Deliveries.Services;
using Ggio.BikeSherpa.Backend.Features.Deliveries.Update;
using Ggio.DddCore;
using Moq;

namespace BackendTests.Features.Deliveries.Update;

public class UpdateDeliveryStepTimeHandlerTests
{
     private readonly Mock<IReadRepository<Delivery>> _mockDeliveryRepository = new();
     private readonly Mock<IValidator<UpdateDeliveryStepTimeCommand>> _mockValidator = new();
     private readonly Mock<IDeliveryChangeTimeService> _mockDeliveryChangeTimeService = new();
     private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());

     private readonly Guid _deliveryId;
     private readonly Guid _stepId;
     private readonly Delivery _delivery;

     public UpdateDeliveryStepTimeHandlerTests()
     {
          _deliveryId = Guid.NewGuid();

          _delivery = _fixture.Build<Delivery>()
               .With(d => d.Id, _deliveryId)
               .With(d => d.Steps, _fixture.Build<DeliveryStep>().CreateMany(3).ToList())
               .Create();

          _stepId = _delivery.Steps.First().Id;

          _mockValidator
               .Setup(x => x.ValidateAsync(
                    It.IsAny<UpdateDeliveryStepTimeCommand>(),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(new FluentValidation.Results.ValidationResult());

          _mockDeliveryRepository
               .Setup(x => x.FirstOrDefaultAsync(
                    It.Is<ISpecification<Delivery>>(s => s is DeliveryByIdSpecification),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(_delivery);
     }

     private UpdateDeliveryStepTimeHandler CreateSut()
     {
          return new UpdateDeliveryStepTimeHandler(
               _mockDeliveryRepository.Object,
               _mockValidator.Object,
               _mockDeliveryChangeTimeService.Object
          );
     }

     [Fact]
     public async Task Handle_ShouldReturnSuccess_WhenRequestIsValid()
     {
          // Arrange
          var sut = CreateSut();
          var date = DateTimeOffset.UtcNow.AddHours(1);
          var command = new UpdateDeliveryStepTimeCommand(_deliveryId, _stepId, date);

          // Act
          var result = await sut.Handle(command, CancellationToken.None);

          // Assert
          result.IsSuccess.Should().BeTrue();

          _mockDeliveryChangeTimeService.Verify(
               x => x.ChangeTime(
                    _delivery,
                    It.Is<DeliveryStep>(s => s.Id == _stepId),
                    command.Date,
                    It.IsAny<CancellationToken>()),
               Times.Once);
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
          var command = new UpdateDeliveryStepTimeCommand(_deliveryId, _stepId, DateTimeOffset.UtcNow.AddHours(1));

          // Act
          var result = await sut.Handle(command, CancellationToken.None);

          // Assert
          result.IsSuccess.Should().BeFalse();
          result.IsNotFound().Should().BeTrue();

          _mockDeliveryChangeTimeService.Verify(
               x => x.ChangeTime(
                    It.IsAny<Delivery>(),
                    It.IsAny<DeliveryStep>(),
                    It.IsAny<DateTimeOffset>(),
                    It.IsAny<CancellationToken>()),
               Times.Never);
     }

     [Fact]
     public async Task Handle_ShouldReturnNotFound_WhenStepDoesNotExist()
     {
          // Arrange
          var sut = CreateSut();
          var command = new UpdateDeliveryStepTimeCommand(_deliveryId, Guid.NewGuid(), DateTimeOffset.UtcNow.AddHours(1));

          // Act
          var result = await sut.Handle(command, CancellationToken.None);

          // Assert
          result.IsSuccess.Should().BeFalse();
          result.IsNotFound().Should().BeTrue();

          _mockDeliveryChangeTimeService.Verify(
               x => x.ChangeTime(
                    It.IsAny<Delivery>(),
                    It.IsAny<DeliveryStep>(),
                    It.IsAny<DateTimeOffset>(),
                    It.IsAny<CancellationToken>()),
               Times.Never);
     }
}