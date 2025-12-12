using AutoFixture;
using AwesomeAssertions;
using FastEndpoints;
using Ggio.BikeSherpa.Backend.Features.Customers;
using Ggio.BikeSherpa.Backend.Features.Customers.Get;
using Ggio.BikeSherpa.Backend.Features.Customers.Services;
using Mediator;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace BackendTests.Features.Customers.Get;

public class GetCustomerEndpointTests
{
     private readonly Mock<IMediator> _mockMediator = new();
     private readonly Mock<ICustomerLinks> _mockCustomerLinks = new();
     private readonly Fixture _fixture = new();

     [Fact]
     public async Task HandleAsync_ShouldCallMediatorAndSendResponse_WhenClientExists()
     {
          // Arrange
          var expectedCustomer = _fixture.Create<CustomerCrud>();
          var sut = CreateSut(expectedCustomer);

          // Act
          await sut.HandleAsync(CancellationToken.None);

          // Assert
          VerifyMediatorCalledOnce();
          sut.HttpContext.Response.StatusCode.Should().Be(StatusCodes.Status200OK);
          sut.Response.Data.Should().BeEquivalentTo(expectedCustomer);
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
          sut.HttpContext.Response.StatusCode.Should().Be(StatusCodes.Status404NotFound);
          sut.HttpContext.Response.Body.Length.Should().Be(0);
     }

     private GetCustomerEndpoint CreateSut(CustomerCrud? existingClient)
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
               s.AddSingleton(_mockCustomerLinks.Object);
          });

          var endpoint = Factory.Create<GetCustomerEndpoint>(
               ctx =>
               {
                    ctx.Request.Method = "GET";
                    ctx.Request.Path = $"/api/customer/{id}";
                    ctx.Request.QueryString = new QueryString($"?customerId={id}");
                    ctx.Request.RouteValues["customerId"] = id;
                    ctx.Response.Body = new MemoryStream();
               },
               _mockMediator.Object,
               _mockCustomerLinks.Object
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
