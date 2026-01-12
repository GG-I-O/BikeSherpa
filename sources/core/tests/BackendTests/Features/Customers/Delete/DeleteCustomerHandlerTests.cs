using Ardalis.Result;
using Ardalis.Specification;
using AutoFixture;
using AwesomeAssertions;
using Ggio.BikeSherpa.Backend.Domain.CustomerAggregate;
using Ggio.BikeSherpa.Backend.Features.Customers.Delete;
using Ggio.DddCore;
using Moq;

namespace BackendTests.Features.Customers.Delete;

public class DeleteCustomerHandlerTests
{
     private readonly Mock<IReadRepository<Customer>> _mockRepository = new();
     private readonly Mock<IApplicationTransaction> _mockTransaction = new();
     private readonly Mock<ICustomerTrash> _mockTrash = new();
     private readonly Fixture _fixture = new();

     private readonly DeleteCustomerCommand _mockCommand;

     public DeleteCustomerHandlerTests()
     {
          _mockCommand = _fixture.Create<DeleteCustomerCommand>();
          var mockCustomer = _fixture.Create<Customer>();

          _mockRepository
               .Setup(x => x.FirstOrDefaultAsync(
                    It.IsAny<ISpecification<Customer>>(),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(mockCustomer);
     }

     private DeleteCustomerHandler CreateSut()
     {
          return new DeleteCustomerHandler(_mockTrash.Object, _mockRepository.Object, _mockTransaction.Object);
     }

     private void VerifyTransactionCommittedOnce()
     {
          _mockTransaction.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
     }

     private void VerifyTrashCalledOnce()
     {
          _mockTrash.Verify(x => x.DeleteCustomerAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()), Times.Once);
     }

     [Fact]
     public async Task Handle_ShouldDeleteCustomerAndReturnId_WhenCommandIsValid()
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
                    It.IsAny<ISpecification<Customer>>(),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(null as Customer);
          var sut = CreateSut();

          // Act
          var result = await sut.Handle(_mockCommand, CancellationToken.None);

          // Assert
          result.IsSuccess.Should().BeFalse();
          result.IsNotFound().Should().BeTrue();
          _mockTransaction.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
          _mockTrash.Verify(x => x.DeleteCustomerAsync(It.IsAny<Customer>(), It.IsAny<CancellationToken>()), Times.Never);
     }
}
