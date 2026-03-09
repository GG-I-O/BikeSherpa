using AutoFixture;
using Ggio.BikeSherpa.Backend.Domain.CustomerAggregate;
using Ggio.BikeSherpa.Backend.Features.Customers.GetAll;
using Ggio.DddCore;
using Moq;

namespace BackendTests.Features.Customers.GetAll;

public class GetAllCustomersHandlerTests
{
     private readonly Mock<IReadRepository<Customer>> _mockRepository = new();
     private readonly IFixture _fixture = new Fixture();
     private readonly Customer _mockCustomerA;
     private readonly Customer _mockCustomerB;

     public GetAllCustomersHandlerTests()
     {
          _mockCustomerA = _fixture.Create<Customer>();
          _mockCustomerB = _fixture.Create<Customer>();
     }

     [Fact]
     public async Task Handle_ShouldReturnAllCustomers_WhenCustomersExist()
     {
          // Arrange
          var customers = new List<Customer>
          {
               _mockCustomerA,
               _mockCustomerB
          };

          var sut = CreateSut(customers);
          var query = new GetAllCustomersQuery(null);

          // Act
          var result = await sut.Handle(query, CancellationToken.None);

          // Assert
          Assert.NotNull(result);
          Assert.Equal(2, result.Count);
          Assert.Contains(result, customer => customer.Name == _mockCustomerA.Name);
          Assert.Contains(result, customer => customer.Code == _mockCustomerB.Code);
          VerifyRepositoryCalledOnce();
     }

     [Fact]
     public async Task Handle_ShouldReturnEmptyList_WhenNoCustomersExist()
     {
          // Arrange
          var sut = CreateSut(new List<Customer>());
          var query = new GetAllCustomersQuery(null);

          // Act
          var result = await sut.Handle(query, CancellationToken.None);

          // Assert
          Assert.NotNull(result);
          Assert.Empty(result);
          VerifyRepositoryCalledOnce();
     }

     private GetAllCustomersHandler CreateSut(List<Customer> returnCustomers)
     {
          _mockRepository
               .Setup(repo => repo.ListAsync(It.IsAny<CancellationToken>()))
               .ReturnsAsync(returnCustomers);

          return new GetAllCustomersHandler(_mockRepository.Object);
     }

     private void VerifyRepositoryCalledOnce()
     {
          _mockRepository.Verify(
               repo => repo.ListAsync(It.IsAny<CancellationToken>()),
               Times.Once
          );
     }
}
