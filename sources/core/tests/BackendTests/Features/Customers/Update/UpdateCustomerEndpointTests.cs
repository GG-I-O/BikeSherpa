using Ardalis.Result;
using AutoFixture;
using AwesomeAssertions;
using FastEndpoints;
using Ggio.BikeSherpa.Backend.Features.Customers.Model;
using Ggio.BikeSherpa.Backend.Features.Customers.Update;
using Mediator;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace BackendTests.Features.Customers.Update;

public class UpdateCustomerEndpointTests
{
     private readonly Fixture _fixture = new();
     private readonly Mock<IMediator> _mockMediator = new();

     [Fact]
     public async Task UpdateCustomer_ValidCustomer_ReturnsOk()
     {
          // Arrange
          var customerCrud = _fixture.Create<CustomerCrud>();
          var expectedId = Guid.NewGuid();
          var sut = CreateSut(expectedId);

          // Act
          await sut.HandleAsync(customerCrud, CancellationToken.None);

          // Assert
          VerifyMediatorCalledOnce();
          sut.HttpContext.Response.StatusCode.Should().Be(StatusCodes.Status200OK);
     }

     private UpdateCustomerEndpoint CreateSut(Guid expectedId)
     {
          _mockMediator
               .Setup(m => m.Send(
                    It.IsAny<UpdateCustomerCommand>(),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(Result.Success());

          Factory.RegisterTestServices(s => { s.AddSingleton(_mockMediator.Object); });

          var endpoint = Factory.Create<UpdateCustomerEndpoint>(
               ctx =>
               {
                    ctx.Request.Method = "PUT";
                    ctx.Request.Path = $"/api/customer/{expectedId}";
                    ctx.Request.QueryString = new QueryString($"?customerId={expectedId}");
                    ctx.Request.RouteValues["customerId"] = expectedId;
                    ctx.Response.Body = new MemoryStream();
               },
               _mockMediator.Object
          );

          return endpoint;
     }

     private void VerifyMediatorCalledOnce()
     {
          _mockMediator.Verify(
               m => m.Send(
                    It.IsAny<UpdateCustomerCommand>(),
                    It.IsAny<CancellationToken>()),
               Times.Once
          );
     }
}
