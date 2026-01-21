using Ardalis.Result;
using AwesomeAssertions;
using FastEndpoints;
using Ggio.BikeSherpa.Backend.Features.Customers.Delete;
using Mediator;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace BackendTests.Features.Customers.Delete;

public class DeleteCustomerEndpointTests
{
     private readonly Mock<IMediator> _mockMediator = new();
     
     private DeleteCustomerEndpoint CreateSut(Guid expectedId)
     {
          _mockMediator
               .Setup(m => m.Send(
                    It.IsAny<DeleteCustomerCommand>(),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(new Result());

          Factory.RegisterTestServices(s =>
          {
               s.AddSingleton(_mockMediator.Object);
          });

          var endpoint = Factory.Create<DeleteCustomerEndpoint>(
               ctx =>
               {
                    ctx.Request.Method = "DELETE";
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
                    It.IsAny<DeleteCustomerCommand>(),
                    It.IsAny<CancellationToken>()),
               Times.Once
          );
     }

     [Fact]
     public async Task DeleteCustomer_ReturnsOkOnSuccessfulDelete()
     {
          // Arrange
          var expectedId = Guid.NewGuid();
          var sut = CreateSut(expectedId);
          
          //Act
          await sut.HandleAsync(CancellationToken.None);
          
          // Assert
          VerifyMediatorCalledOnce();
          sut.HttpContext.Response.StatusCode.Should().Be(StatusCodes.Status200OK);
     }
}
