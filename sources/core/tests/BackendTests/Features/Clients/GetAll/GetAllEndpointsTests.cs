using FastEndpoints;
using Ggio.BikeSherpa.Backend.Domain;
using Ggio.BikeSherpa.Backend.Features.Customers;
using Ggio.BikeSherpa.Backend.Features.Customers.GetAll;
using Ggio.BikeSherpa.Backend.Services.Hateoas;
using Mediator;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace BackendTests.Features.Clients.GetAll;

public class GetAllEndpointsTests
{
     private readonly Mock<IMediator> _mockMediator = new();
     private readonly Mock<IHateoasService> _mockHateoasService = new();
     
     private readonly CustomerCrud _mockCustomerA;
     private readonly CustomerCrud _mockCustomerB;

     public GetAllEndpointsTests()
     {
          _mockCustomerA = CustomerTestHelper.CreateCustomerCrud(
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
          _mockCustomerB = CustomerTestHelper.CreateCustomerCrud(
               Guid.NewGuid(),
               "Client B",
               "BBB",
               null,
               "b@h.com",
               "9876543210",
               new Address
               {
                    name = "Client B",
                    streetInfo = "321 rue des roses",
                    postcode = "54855",
                    city = "Anakin"
               });
     }

     [Fact]
     public async Task HandleAsync_ShouldCallMediatorAndSendResponse_WhenClientsExist()
     {
          // Arrange
          var clients = new List<CustomerCrud>
          {
               _mockCustomerA,
               _mockCustomerB
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
          var sut = CreateSut(new List<CustomerCrud>());

          // Act
          await sut.HandleAsync(CancellationToken.None);

          // Assert
          VerifyMediatorCalledOnce();
          Assert.Equal(StatusCodes.Status200OK, sut.HttpContext.Response.StatusCode);
     }

     private GetAllEndpoint CreateSut(List<CustomerCrud> returnClients)
     {
          _mockMediator
               .Setup(m => m.Send(It.IsAny<GetAllClientsQuery>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(returnClients);
          
          Factory.RegisterTestServices(s =>
          {
               s.AddSingleton(_mockMediator.Object);
               s.AddSingleton(_mockHateoasService.Object);
          });

          var endpoint = Factory.Create<GetAllEndpoint>(
               ctx =>
               {
                    ctx.Request.Method = "GET";
                    ctx.Request.Path = "/api/clients";
               },
               _mockMediator.Object,
               _mockHateoasService.Object
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
}
