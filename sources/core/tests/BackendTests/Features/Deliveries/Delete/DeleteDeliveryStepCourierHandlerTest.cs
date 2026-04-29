using Ardalis.Result;
using Ardalis.Specification;
using AutoFixture;
using AutoFixture.AutoMoq;
using AwesomeAssertions;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Specification;
using Ggio.BikeSherpa.Backend.Features.Deliveries.Delete;
using Ggio.DddCore;
using JetBrains.Annotations;
using Moq;

namespace BackendTests.Features.Deliveries.Delete;

[TestSubject(typeof(DeleteDeliveryStepCourierHandler))]
public class DeleteDeliveryStepCourierHandlerTest
{
     private readonly Mock<IApplicationTransaction> _mockTransaction = new();
     private readonly Mock<IReadRepository<Delivery>> _mockDeliveryRepository = new();
     private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());

     private readonly DeleteDeliveryStepCourierCommand _mockCommand;
     private readonly Delivery _mockDelivery;
     private readonly DeliveryStep _mockStep;
     private readonly Guid _courierId = Guid.NewGuid();

     public DeleteDeliveryStepCourierHandlerTest()
     {
          _mockDelivery = _fixture.Build<Delivery>()
               .With(d => d.Steps, [])
               .With(d => d.ContractDate, DateTime.UtcNow)
               .With(d => d.StartDate, DateTime.UtcNow)
               .With(d => d.CreatedAt, DateTime.UtcNow)
               .With(d => d.UpdatedAt, DateTime.UtcNow)
               .Create();
          
          _mockStep = _fixture.Build<DeliveryStep>()
               .With(s => s.ParentDelivery, _mockDelivery)
               .With(s => s.CourierId, _courierId)
               .With(s => s.EstimatedDeliveryDate, DateTime.UtcNow)
               .With(s => s.RealDeliveryDate, (DateTimeOffset?)null)
               .With(s => s.CreatedAt, DateTime.UtcNow)
               .With(s => s.UpdatedAt, DateTime.UtcNow)
               .Create();
          _mockDelivery.Steps.Add(_mockStep);

          _mockCommand = new DeleteDeliveryStepCourierCommand(
               _mockDelivery.Id,
               _mockStep.Id
          );

          _mockDeliveryRepository
               .Setup(x => x.FirstOrDefaultAsync(
                    It.Is<ISpecification<Delivery>>(s => s is DeliveryByIdSpecification),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(_mockDelivery);
     }

     private DeleteDeliveryStepCourierHandler CreateSut()
     {
          return new DeleteDeliveryStepCourierHandler(
               _mockDeliveryRepository.Object,
               _mockTransaction.Object
          );
     }

     [Fact]
     public async Task Handle_ShouldRemoveCourierFromDeliveryStep_WhenCommandIsValid()
     {
          // Arrange
          var sut = CreateSut();

          // Act
          var result = await sut.Handle(_mockCommand, CancellationToken.None);

          // Assert
          result.IsSuccess.Should().BeTrue();
          _mockStep.CourierId.Should().BeNull();

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
     public async Task Handle_ShouldThrowInvalidOperationException_WhenStepDoesNotExist()
     {
          // Arrange
          var command = new DeleteDeliveryStepCourierCommand(
               _mockDelivery.Id,
               Guid.NewGuid()
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