using System.Text.Json;
using Ardalis.Result;
using AutoFixture;
using AwesomeAssertions;
using FastEndpoints;
using Ggio.BikeSherpa.Backend.Features.Customers.Add;
using Ggio.BikeSherpa.Backend.Features.Customers.Model;
using Mediator;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace BackendTests.Features.Customers.Add;

public class AddCustomerEndpointTests(ITestContextAccessor testContextAccessor)
{
     private readonly Mock<IMediator> _mockMediator = new();
     private readonly Fixture _fixture = new();
     private readonly CancellationToken _cancellationToken = testContextAccessor.Current.CancellationToken;

     [Fact]
     public async Task AddCustomer_ValidCustomer_ReturnsCreated()
     {
          // Arrange
          var customerCrud = _fixture.Create<CustomerCrud>();
          var expectedId = Guid.NewGuid();
          var sut = CreateSut(expectedId);

          // Act
          await sut.HandleAsync(customerCrud, CancellationToken.None);

          // Assert
          VerifyMediatorCalledOnce();
          sut.HttpContext.Response.StatusCode.Should().Be(StatusCodes.Status201Created);

          // Read the response body
          sut.HttpContext.Response.Body.Position = 0;
          using var reader = new StreamReader(sut.HttpContext.Response.Body);
          var responseBody = await reader.ReadToEndAsync(_cancellationToken);
          var responseObject = JsonSerializer.Deserialize<JsonElement>(responseBody);
          var actualId = responseObject.GetProperty("id").GetGuid();

          actualId.Should().Be(expectedId);
     }

     private AddCustomerEndpoint CreateSut(Guid expectedId)
     {
          _mockMediator
               .Setup(m => m.Send(
                    It.IsAny<AddCustomerCommand>(),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(new Result<Guid>(expectedId));

          Factory.RegisterTestServices(s =>
          {
               s.AddSingleton(_mockMediator.Object);
          });

          var endpoint = Factory.Create<AddCustomerEndpoint>(
               ctx =>
               {
                    ctx.Request.Method = "POST";
                    ctx.Request.Path = "/api/customer";
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
                    It.IsAny<AddCustomerCommand>(),
                    It.IsAny<CancellationToken>()),
               Times.Once
          );
     }
}
