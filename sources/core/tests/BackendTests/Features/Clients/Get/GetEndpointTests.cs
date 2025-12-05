using FastEndpoints;
using Ggio.BikeSherpa.Backend.Features.Clients;
using Ggio.BikeSherpa.Backend.Features.Clients.Get;
using Ggio.BikeSherpa.Backend.Features.Clients.GetAll;
using Mediator;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace BackendTests.Features.Clients.Get;

public class GetEndpointTests
{
     private readonly Mock<IMediator> _mockMediator = new();

     [Fact]
     public async Task HandleAsync_ShouldCallMediatorAndSendResponse_WhenClientExists()
     {
          // Arrange
          var client = CreateClientCrud(Guid.NewGuid(), "Client A", "AAA", null, "a@g.com", "0123456789", "123 rue des roses");
          var sut = CreateSut(client);
          
          // Act
          await sut.HandleAsync(CancellationToken.None);
          
          // Assert
          VerifyMediatorCalledOnce();
          Assert.Equal(StatusCodes.Status200OK, sut.HttpContext.Response.StatusCode);
     }

     [Fact]
     public async Task HandleAsync_ShouldCallMediatorAndSendResponse_WhenClientDoesNotExist()
     {
          // Arrange
          var sut = CreateSut(null);
          
          // Act
          await sut.HandleAsync(CancellationToken.None);
          
          // Assert
          VerifyMediatorCalledOnce();
          Assert.Equal(StatusCodes.Status404NotFound, sut.HttpContext.Response.StatusCode);
     }
          
     private GetEndpoint CreateSut(ClientCrud? existingClient)
     {
          var id = existingClient?.Id ?? Guid.NewGuid();
          _mockMediator
               .Setup(m => m.Send(
                    It.IsAny<GetClientQuery>(),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(existingClient);
          
          Factory.RegisterTestServices(s => s.AddSingleton(_mockMediator.Object));

          var endpoint = Factory.Create<GetEndpoint>(
               ctx =>
               {
                    ctx.Request.Method = "GET";
                    ctx.Request.Path = $"/api/client/{id}";
                    ctx.Request.QueryString = new QueryString($"?Id={id}");
               },
               _mockMediator.Object
          );

          return endpoint;
     }
          
     private void VerifyMediatorCalledOnce()
     {
          _mockMediator.Verify(
               m => m.Send(
                    It.IsAny<GetClientQuery>(), 
                    It.IsAny<CancellationToken>()),
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

internal class RoutingFeature : IRoutingFeature
{
     public RouteData RouteData { get; set; } = new();
}
