using Ardalis.Result;
using AutoFixture;
using AwesomeAssertions;
using Ggio.BikeSherpa.Backend.Features.Customers.Model;
using Ggio.BikeSherpa.Backend.Features.Customers.Update;
using Mediator;
using System.Net;
using System.Net.Http.Json;
using BackendTests.Services;
using Moq;

namespace BackendTests.Features.Customers.Update;

public class UpdateCustomerWebApplicationFactory() : TestWebApplicationFactory("write:customers", "write:customers");

public class UpdateCustomerEndpointTests(
     UpdateCustomerWebApplicationFactory factory,
     ITestContextAccessor testContextAccessor) : IClassFixture<UpdateCustomerWebApplicationFactory>
{
     private readonly HttpClient _client = factory.CreateClient();
     private readonly Mock<IMediator> _mockMediator = factory.MockMediator;
     private readonly Fixture _fixture = new();
     private readonly CancellationToken _cancellationToken = testContextAccessor.Current.CancellationToken;

     [Fact]
     public async Task UpdateCustomer_ValidCustomer_ReturnsOk()
     {
          // Arrange
          var customerCrud = _fixture.Create<CustomerCrud>();
          var expectedId = Guid.NewGuid();

          _mockMediator
               .Setup(m => m.Send(
                    It.IsAny<UpdateCustomerCommand>(),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(Result.Success());

          // Act
          var response = await _client.PutAsJsonAsync($"/api/customer/{expectedId}", customerCrud, _cancellationToken);

          // Assert
          response.StatusCode.Should().Be(HttpStatusCode.OK);

          _mockMediator.Verify(
               m => m.Send(
                    It.IsAny<UpdateCustomerCommand>(),
                    It.IsAny<CancellationToken>()),
               Times.Once);
     }
}
