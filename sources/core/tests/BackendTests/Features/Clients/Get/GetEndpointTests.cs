using FastEndpoints;
using Ggio.BikeSherpa.Backend.Domain;
using Ggio.BikeSherpa.Backend.Features.Customers;
using Ggio.BikeSherpa.Backend.Features.Customers.Get;
using Ggio.BikeSherpa.Backend.Services.Hateoas;
using Mediator;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace BackendTests.Features.Clients.Get;

public class GetEndpointTests
{
     private readonly Mock<IMediator> _mockMediator = new();
     private readonly Mock<IHateoasService> _mockHateoasService = new();
     
     private readonly CustomerCrud _mockCustomer;

     public GetEndpointTests()
     {
          _mockCustomer = CustomerTestHelper.CreateCustomerCrud(
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
               });
     }

     [Fact]
     public async Task HandleAsync_ShouldCallMediatorAndSendResponse_WhenClientExists()
     {
          // Arrange
          var sut = CreateSut(_mockCustomer);
          
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
          
     private GetEndpoint CreateSut(CustomerCrud? existingClient)
     {
          var id = existingClient?.Id ?? Guid.NewGuid();
          _mockMediator
               .Setup(m => m.Send(
                    It.IsAny<GetClientQuery>(),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(existingClient);
          
          Factory.RegisterTestServices(s =>
          {
               s.AddSingleton(_mockMediator.Object);
               s.AddSingleton(_mockHateoasService.Object);
          });

          var endpoint = Factory.Create<GetEndpoint>(
               ctx =>
               {
                    ctx.Request.Method = "GET";
                    ctx.Request.Path = $"/api/customer/{id}";
                    ctx.Request.QueryString = new QueryString($"?customerId={id}");
               },
               _mockMediator.Object,
               _mockHateoasService.Object
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
}

internal class RoutingFeature : IRoutingFeature
{
     public RouteData RouteData { get; set; } = new();
}
