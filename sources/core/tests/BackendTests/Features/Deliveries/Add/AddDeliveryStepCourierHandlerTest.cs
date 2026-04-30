using Ardalis.Result;
using Ardalis.Specification;
using AutoFixture;
using AutoFixture.AutoMoq;
using AwesomeAssertions;
using Ggio.BikeSherpa.Backend.Domain.CourierAggregate;
using Ggio.BikeSherpa.Backend.Domain.CourierAggregate.Specification;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Specification;
using Ggio.BikeSherpa.Backend.Features.Deliveries.Add;
using Ggio.DddCore;
using JetBrains.Annotations;
using Moq;

namespace BackendTests.Features.Deliveries.Add;

[TestSubject(typeof(AddDeliveryStepCourierHandler))]
public class AddDeliveryStepCourierHandlerTest
{
     private readonly Mock<IApplicationTransaction> _mockTransaction = new();
     private readonly Mock<IReadRepository<Delivery>> _mockDeliveryRepository = new();
     private readonly Mock<IReadRepository<Courier>> _mockCourierRepository = new();
     private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());

     private readonly AddDeliveryStepCourierCommand _mockCommand;
     private readonly Delivery _mockDelivery;
     private readonly DeliveryStep _mockStep;
     private readonly Courier _mockCourier;

     public AddDeliveryStepCourierHandlerTest()
     {

          _mockDelivery = _fixture.Build<Delivery>()
               .With(d => d.Steps, [])
               .With(d => d.ContractDate, DateTime.UtcNow)
               .With(d => d.StartDate, DateTime.UtcNow)
               .Create();
          
          _mockStep = _fixture.Build<DeliveryStep>()
               .With(s => s.ParentDelivery, _mockDelivery)
               .Create();
          
          _mockDelivery.Steps.Add(_mockStep);

          _mockCourier = _fixture.Create<Courier>();

          _mockCommand = new AddDeliveryStepCourierCommand(
               _mockDelivery.Id,
               _mockStep.Id,
               _mockCourier.Id
          );

          _mockDeliveryRepository
               .Setup(x => x.FirstOrDefaultAsync(
                    It.Is<ISpecification<Delivery>>(s => s is DeliveryByIdSpecification),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(_mockDelivery);

          _mockCourierRepository
               .Setup(x => x.FirstOrDefaultAsync(
                    It.Is<ISpecification<Courier>>(s => s is CourierByIdSpecification),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(_mockCourier);
     }

     private AddDeliveryStepCourierHandler CreateSut()
     {
          return new AddDeliveryStepCourierHandler(
               _mockDeliveryRepository.Object,
               _mockCourierRepository.Object,
               _mockTransaction.Object
          );
     }

     [Fact]
     public async Task Handle_ShouldAssignCourierToDeliveryStep_WhenCommandIsValid()
     {
          // Arrange
          var sut = CreateSut();

          // Act
          var result = await sut.Handle(_mockCommand, CancellationToken.None);

          // Assert
          result.IsSuccess.Should().BeTrue();
          _mockStep.CourierId.Should().Be(_mockCourier.Id);

          _mockTransaction.Verify(
               x => x.CommitAsync(It.IsAny<CancellationToken>()),
               Times.Once);
     }

     [Fact]
     public async Task Handle_ShouldReturnNotFound_WhenDeliveryDoesNotExist()
     {
          // Arrange
          Delivery? missingDelivery = null;

          _mockDeliveryRepository
               .Setup(x => x.FirstOrDefaultAsync(
                    It.Is<ISpecification<Delivery>>(s => s is DeliveryByIdSpecification),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(missingDelivery);

          var sut = CreateSut();

          // Act
          var result = await sut.Handle(_mockCommand, CancellationToken.None);

          // Assert
          result.IsSuccess.Should().BeFalse();
          result.Status.Should().Be(ResultStatus.NotFound);

          _mockTransaction.Verify(
               x => x.CommitAsync(It.IsAny<CancellationToken>()),
               Times.Never);
     }

     [Fact]
     public async Task Handle_ShouldReturnNotFound_WhenCourierDoesNotExist()
     {
          // Arrange
          Courier? missingCourier = null;

          _mockCourierRepository
               .Setup(x => x.FirstOrDefaultAsync(
                    It.Is<ISpecification<Courier>>(s => s is CourierByIdSpecification),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(missingCourier);

          var sut = CreateSut();

          // Act
          var result = await sut.Handle(_mockCommand, CancellationToken.None);

          // Assert
          result.IsSuccess.Should().BeFalse();
          result.Status.Should().Be(ResultStatus.NotFound);

          _mockTransaction.Verify(
               x => x.CommitAsync(It.IsAny<CancellationToken>()),
               Times.Never);
     }

     [Fact]
     public async Task Handle_ShouldThrowInvalidOperationException_WhenStepDoesNotExist()
     {
          // Arrange
          var command = new AddDeliveryStepCourierCommand(
               _mockDelivery.Id,
               Guid.NewGuid(),
               _mockCourier.Id
          );

          var sut = CreateSut();

          // Act
          var act = async () => await sut.Handle(command, CancellationToken.None);

          // Assert
          await act.Should().ThrowAsync<InvalidOperationException>();

          _mockTransaction.Verify(
               x => x.CommitAsync(It.IsAny<CancellationToken>()),
               Times.Never);
     }
}