using Ggio.BikeSherpa.Backend.Domain;
using Ggio.BikeSherpa.Backend.Domain.CustomerAggregate;
using Ggio.BikeSherpa.Backend.Features.Customers.GetAll;
using Ggio.DddCore;
using Moq;

namespace BackendTests.Features.Clients.GetAll;

public class GetAllCustomersHandlerTests
{
     private readonly Mock<IReadRepository<Customer>> _mockRepository = new();

     private readonly Customer _mockCustomerA;
     private readonly Customer _mockCustomerB;

     public GetAllCustomersHandlerTests()
     {
          _mockCustomerA = CustomerTestHelper.CreateCustomer(
               Guid.NewGuid(),
               "Client A",
               "AAA",
               null,
               "a@g.com",
               "0123456789",
               new Address
               {
                    name = "Client A",
                    streetInfo = "123 rue des roses",
                    postcode = "12502",
                    city = "Obiwan"
               }
          );

          _mockCustomerB = CustomerTestHelper.CreateCustomer(
               Guid.NewGuid(),
               "Client B",
               "BBB",
               null,
               "b@h.com",
               "9876543210",
               new Address
               {
                    name = "Client B",
                    streetInfo = "321 rue des hortensias",
                    postcode = "78458",
                    city = "Anakin"
               }
          );
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
          var query = new GetAllClientsQuery(null);

          // Act
          var result = await sut.Handle(query, CancellationToken.None);

          // Assert
          Assert.NotNull(result);
          Assert.Equal(2, result.Count);
          Assert.Contains(result, customer => customer.Name == "Client A");
          Assert.Contains(result, customer => customer.Code == "BBB");
          VerifyRepositoryCalledOnce();
     }

     [Fact]
     public async Task Handle_ShouldReturnEmptyList_WhenNoCustomersExist()
     {
          // Arrange
          var sut = CreateSut(new List<Customer>());
          var query = new GetAllClientsQuery(null);

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
