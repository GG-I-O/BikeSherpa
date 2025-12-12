using Ardalis.Specification;
using Ggio.BikeSherpa.Backend.Domain;
using Ggio.BikeSherpa.Backend.Domain.CustomerAggregate;
using Ggio.BikeSherpa.Backend.Domain.CustomerAggregate.Specification;
using Ggio.BikeSherpa.Backend.Features.Customers.Get;
using Ggio.DddCore;
using Moq;

namespace BackendTests.Features.Customers.Get;

public class GetCustomerHandlerTests
{
     private readonly Mock<IReadRepository<Customer>> _mockRepository = new();

     private readonly Customer _mockCustomer = new()
     {
          Id = Guid.NewGuid(),
          Name = "Client A",
          Code = "AAA",
          Siret = null,
          Email = "a@g.com",
          PhoneNumber = "0123456789",
          Address = new Address
          {
               name = "Client A",
               streetInfo = "123 rue des roses",
               postcode = "12502",
               city = "Obi-wan"
          }
     };


     [Fact]
     public async Task Handle_ShouldReturnOneCustomer_WhenCustomerExists()
     {
          // Arrange
          var guid = Guid.NewGuid();
          _mockCustomer.Id = guid;
          var sut = CreateSut(_mockCustomer);
          var query = new GetClientQuery(guid);
          
          // Act
          var result = await sut.Handle(query, CancellationToken.None);
          
          // Assert
          Assert.NotNull(result);//CACA
          Assert.Equal(guid, result.Id);
          Assert.Equal("Client A", result.Name);
          VerifyRepositoryCalledOnce();
     }
     
     [Theory]
     [InlineData(true)]
     [InlineData(false)]
     public async Task Handle_ShouldReturnNull_WhenCustomerDoesNotExist(bool emptyRepository)
     {
          // Arrange
          var guidA = Guid.NewGuid();
          var guidB = Guid.NewGuid();
          _mockCustomer.Id = guidA;
          var sut = CreateSut(emptyRepository ? null : _mockCustomer);
          var query = new GetClientQuery(guidB);
          
          // Act
          var result = await sut.Handle(query, CancellationToken.None);
          
          // Assert
          Assert.Null(result);
          VerifyRepositoryCalledOnce();
     }

     private GetCustomerHandler CreateSut(Customer? existingCustomer)
     {
          _mockRepository
               .Setup(repo => repo.FirstOrDefaultAsync(
                    It.Is<ISpecification<Customer>>(s => s is CustomerByIdSpecification && existingCustomer != null && s.IsSatisfiedBy(existingCustomer)), 
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(existingCustomer);
          return new GetCustomerHandler(_mockRepository.Object);
     }

     private void VerifyRepositoryCalledOnce()
     {
          _mockRepository.Verify(repo => repo.FirstOrDefaultAsync(
               It.IsAny<ISpecification<Customer>>(), 
               It.IsAny<CancellationToken>()),
               Times.Once
          );
     }

     
}
