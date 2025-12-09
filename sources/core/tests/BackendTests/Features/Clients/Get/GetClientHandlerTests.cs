using System.Runtime.CompilerServices;
using Ardalis.Specification;
using Ggio.BikeSherpa.Backend.Domain.ClientAggregate;
using Ggio.BikeSherpa.Backend.Domain.ClientAggregate.Specification;
using Ggio.BikeSherpa.Backend.Features.Clients.Get;
using Ggio.DddCore;
using Moq;

namespace BackendTests.Features.Clients.Get;

public class GetClientHandlerTests
{
     private readonly Mock<IReadRepository<Client>> _mockRepository = new();

     [Fact]
     public async Task Handle_ShouldReturnOneClient_WhenClientExists()
     {
          // Arrange
          var guid = Guid.NewGuid();
          var client = CreateClient(guid, "Client A", "AAA", null, "a@g.com", "0123456789", "123 rue des roses");
          var sut = CreateSut(client);
          var query = new GetClientQuery(guid);
          
          // Act
          var result = await sut.Handle(query, CancellationToken.None);
          
          // Assert
          Assert.NotNull(result);
          Assert.Equal(guid, result.Id);
          Assert.Equal("Client A", result.Name);
          VerifyRepositoryCalledOnce();
     }
     
     [Theory]
     [InlineData(true)]
     [InlineData(false)]
     public async Task Handle_ShouldReturnNull_WhenClientDoesNotExist(bool emptyRepository)
     {
          // Arrange
          var guidA = Guid.NewGuid();
          var guidB = Guid.NewGuid();
          var client = CreateClient(guidA, "Client A", "AAA", null, "a@g.com", "0123456789", "123 rue des roses");
          if (emptyRepository)
               client = null;
          var sut = CreateSut(client);
          var query = new GetClientQuery(guidB);
          
          // Act
          var result = await sut.Handle(query, CancellationToken.None);
          
          // Assert
          Assert.Null(result);
          VerifyRepositoryCalledOnce();
     }

     private GetClientHandler CreateSut(Client? existingClient)
     {
          _mockRepository
               .Setup(repo => repo.FirstOrDefaultAsync(
                    It.Is<ISpecification<Client>>(s => s is ClientByIdSpecification && existingClient != null && s.IsSatisfiedBy(existingClient)), 
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(existingClient);
          return new GetClientHandler(_mockRepository.Object);
     }

     private void VerifyRepositoryCalledOnce()
     {
          _mockRepository.Verify(repo => repo.FirstOrDefaultAsync(
               It.IsAny<ISpecification<Client>>(), 
               It.IsAny<CancellationToken>()),
               Times.Once
          );
     }

     private Client CreateClient(
          Guid id,
          string name,
          string code,
          string? siret,
          string email,
          string phoneNumber,
          string address
     )
     {
          return new Client
          {
               Id = id,
               Name = name,
               Code = code,
               Siret = siret,
               Email = email,
               PhoneNumber = phoneNumber,
               Address = address
          };
     }
}
