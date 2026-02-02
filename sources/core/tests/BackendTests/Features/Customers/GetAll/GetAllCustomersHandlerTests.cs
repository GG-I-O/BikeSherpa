using Ggio.BikeSherpa.Backend.Domain;
using Ggio.BikeSherpa.Backend.Domain.CustomerAggregate;
using Ggio.BikeSherpa.Backend.Features.Customers.GetAll;
using Ggio.DddCore;
using Moq;

namespace BackendTests.Features.Customers.GetAll;

public class GetAllCustomersHandlerTests
{
     private readonly Mock<IReadRepository<Customer>> _mockRepository = new();

     private readonly Customer _mockCustomerA = new()
     {
          Id = Guid.NewGuid(),
          Name = "Client A",
          Code = "AAA",
          Siret = null,
          Email = "a@g.com",
          PhoneNumber = "0123456789",
          Address = new Address
          {
               Name = "Client A",
               StreetInfo = "123 rue des roses",
               Postcode = "12502",
               City = "Obi-wan"
          }
     };
     private readonly Customer _mockCustomerB = new ()
     {
          Id = Guid.NewGuid(),
          Name = "Client B",
          Code = "BBB",
          Siret = null,
          Email = "b@h.com",
          PhoneNumber = "9876543210",
          Address = new Address
          {
               Name = "Client B",
               StreetInfo = "321 rue des roses",
               Postcode = "54855",
               City = "Anakin"
          }
     };

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
          Assert.Contains(result, customer => customer.Name == "Client A");
          Assert.Contains(result, customer => customer.Code == "BBB");
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
