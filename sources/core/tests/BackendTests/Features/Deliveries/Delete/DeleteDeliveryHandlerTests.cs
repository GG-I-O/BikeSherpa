using Ardalis.Result;
using Ardalis.Specification;
using AutoFixture;
using AwesomeAssertions;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Features.Deliveries.Delete;
using Ggio.DddCore;
using Moq;

namespace BackendTests.Features.Deliveries.Delete;

public class DeleteDeliveryHandlerTests
{
     private readonly Mock<IReadRepository<Delivery>> _mockRepository = new();
     private readonly Mock<IApplicationTransaction> _mockTransaction = new();
     private readonly Mock<IDeliveryDeleteEventHandler> _mockDeleteEventHandler = new();
     private readonly Fixture _fixture = new();

     private readonly DeleteDeliveryCommand _mockCommand;

     public DeleteDeliveryHandlerTests()
     {
          _mockCommand = _fixture.Create<DeleteDeliveryCommand>();
          var mockDelivery = _fixture.Create<Delivery>();

          _mockRepository
               .Setup(x => x.FirstOrDefaultAsync(
                    It.IsAny<ISpecification<Delivery>>(),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(mockDelivery);
     }

     private DeleteDeliveryHandler CreateSut()
     {
          return new DeleteDeliveryHandler(_mockDeleteEventHandler.Object, _mockRepository.Object, _mockTransaction.Object);
     }

     private void VerifyTransactionCommittedOnce()
     {
          _mockTransaction.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
     }

     private void VerifyTrashCalledOnce()
     {
          _mockDeleteEventHandler.Verify(x => x.DeleteDeliveryAsync(It.IsAny<Delivery>(), It.IsAny<CancellationToken>()), Times.Once);
     }

     [Fact]
     public async Task Handle_ShouldDeleteDeliveryAndReturnId_WhenCommandIsValid()
     {
          // Arrange
          var sut = CreateSut();

          // Act
          var result = await sut.Handle(_mockCommand, CancellationToken.None);

          // Assert
          result.IsSuccess.Should().BeTrue();
          VerifyTrashCalledOnce();
          VerifyTransactionCommittedOnce();
     }

     [Fact]
     public async Task Handle_ShouldReturnNotFoundIfIdDoesNotExist()
     {
          // Arrange
          _mockRepository
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
          _mockDeleteEventHandler.Verify(x => x.DeleteDeliveryAsync(It.IsAny<Delivery>(), It.IsAny<CancellationToken>()), Times.Never);
     }
}
