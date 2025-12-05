using Ggio.BikeSherpa.Backend.Domain.ClientAggregate;
using Ggio.BikeSherpa.Backend.Features.Clients.GetAll;
using Ggio.DddCore;
using Moq;

namespace BackendTests.Features.Clients.GetAll;

public class GetAllClientsHandlerTests
{
     private readonly Mock<IReadRepository<Client>> _mockRepository = new();

     [Fact]
     public async Task Handle_ShouldReturnAllClients_WhenClientsExist()
     {
          // Arrange
          var clients = new List<Client>
          {
               CreateClient(Guid.NewGuid(), "Client A", "AAA", null, "a@g.com", "0123456789", "123 rue des roses"),
               CreateClient(Guid.NewGuid(), "Client B", "BBB", null, "b@h.com", "9876543210", "12 avenue des hortensias")
          };

          var sut = CreateSut(clients);
          var query = new GetAllClientsQuery();

          // Act
          var result = await sut.Handle(query, CancellationToken.None);

          // Assert
          Assert.NotNull(result);
          Assert.Equal(2, result.Count);
          Assert.Contains(result, client => client.Name == "Client A");
          Assert.Contains(result, client => client.Code == "BBB");
          VerifyRepositoryCalledOnce();
     }

     [Fact]
     public async Task Handle_ShouldReturnEmptyList_WhenNoClientsExist()
     {
          // Arrange
          var sut = CreateSut(new List<Client>());
          var query = new GetAllClientsQuery();

          // Act
          var result = await sut.Handle(query, CancellationToken.None);

          // Assert
          Assert.NotNull(result);
          Assert.Empty(result);
          VerifyRepositoryCalledOnce();
     }

     private GetAllClientsHandler CreateSut(List<Client> returnClients)
     {
          _mockRepository
               .Setup(repo => repo.ListAsync(It.IsAny<CancellationToken>()))
               .ReturnsAsync(returnClients);
          return new GetAllClientsHandler(_mockRepository.Object);
     }

     private void VerifyRepositoryCalledOnce()
     {
          _mockRepository.Verify(
               repo => repo.ListAsync(It.IsAny<CancellationToken>()),
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
