using FastEndpoints;
using Ggio.BikeSherpa.Backend.Features.Clients;
using Ggio.BikeSherpa.Backend.Features.Clients.GetAll;
using Mediator;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace BackendTests.Features.Clients.GetAll;

public class GetAllEndpointsTests
{
     private readonly Mock<IMediator> _mockMediator = new();

     [Fact]
     public async Task HandleAsync_ShouldCallMediatorAndSendResponse_WhenClientsExist()
     {
          // Arrange
          var clients = new List<ClientCrud>
          {
               CreateClientCrud(Guid.NewGuid(), "Client A", "AAA", null, "a@g.com", "0123456789", "123 rue des roses"),
               CreateClientCrud(Guid.NewGuid(), "Client B", "BBB", null, "b@h.com", "9876543210", "12 avenue des hortensias")
          };
          var sut = CreateSut(clients);

          // Act
          await sut.HandleAsync(CancellationToken.None);

          // Assert
          VerifyMediatorCalledOnce();
          Assert.Equal(StatusCodes.Status200OK, sut.HttpContext.Response.StatusCode);
     }

     [Fact]
     public async Task HandleAsync_ShouldCallMediatorAndSendResponse_WhenNoClientsExist()
     {
          // Arrange
          var sut = CreateSut(new List<ClientCrud>());

          // Act
          await sut.HandleAsync(CancellationToken.None);

          // Assert
          VerifyMediatorCalledOnce();
          Assert.Equal(StatusCodes.Status200OK, sut.HttpContext.Response.StatusCode);
     }

     private GetAllEndpoint CreateSut(List<ClientCrud> returnClients)
     {
          _mockMediator
               .Setup(m => m.Send(It.IsAny<GetAllClientsQuery>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(returnClients);
          
          Factory.RegisterTestServices(s => s.AddSingleton(_mockMediator.Object));

          var endpoint = Factory.Create<GetAllEndpoint>(
               ctx =>
               {
                    ctx.Request.Method = "GET";
                    ctx.Request.Path = "/api/clients";
               },
               _mockMediator.Object
          );

          return endpoint;
     }

     private void VerifyMediatorCalledOnce()
     {
          _mockMediator.Verify(
               m => m.Send(It.IsAny<GetAllClientsQuery>(), It.IsAny<CancellationToken>()),
               Times.Once
          );
     }

     private ClientCrud CreateClientCrud(
          Guid id,
          string name,
          string code,
          string? siret,
          string email,
          string phoneNumber,
          string address
     )
     {
          return new ClientCrud
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
