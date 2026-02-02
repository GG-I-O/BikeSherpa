using System.Net;
using System.Text.Json;
using AutoFixture;
using AwesomeAssertions;
using BackendTests.Services;
using Ggio.BikeSherpa.Backend.Features.Customers.Get;
using JetBrains.Annotations;
using Mediator;
using Moq;
using CustomerCrud = Ggio.BikeSherpa.Backend.Features.Customers.Model.CustomerCrud;

namespace BackendTests.Features.Customers.Get;

[UsedImplicitly]
public class GetCustomerWebApplicationFactory() : TestWebApplicationFactory("read:customers", "read:customers");

public class GetCustomerEndpointTests(
     GetCustomerWebApplicationFactory factory,
     ITestContextAccessor testContextAccessor) : IClassFixture<GetCustomerWebApplicationFactory>
{
     private readonly HttpClient _client = factory.CreateClient();
     private readonly Mock<IMediator> _mockMediator = factory.MockMediator;
     private readonly Fixture _fixture = new();
     private readonly CancellationToken _cancellationToken = testContextAccessor.Current.CancellationToken;
     private readonly JsonSerializerOptions _jsonSerializerOptions = new()
     {
          PropertyNameCaseInsensitive = false,
          PropertyNamingPolicy = JsonNamingPolicy.CamelCase
     };

     [Fact]
     public async Task HandleAsync_ShouldCallMediatorAndSendResponse_WhenClientExists()
     {
          // Arrange
          _mockMediator.Reset();
          var expectedCustomer = _fixture.Create<CustomerCrud>();

          _mockMediator
               .Setup(m => m.Send(
                    It.IsAny<GetCustomerQuery>(),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(expectedCustomer);

          // Act
          var response = await _client.GetAsync($"/api/customer/{expectedCustomer.Id}", _cancellationToken);

          // Assert
          response.StatusCode.Should().Be(HttpStatusCode.OK);

          _mockMediator.Verify(
               m => m.Send(
                    It.IsAny<GetCustomerQuery>(),
                    It.IsAny<CancellationToken>()),
               Times.Once);

          var responseBody = await response.Content.ReadAsStringAsync(_cancellationToken);
          var responseObject = JsonSerializer.Deserialize<JsonElement>(responseBody, _jsonSerializerOptions);
          var actualCustomer = responseObject.GetProperty("data");

          actualCustomer.GetProperty("id").GetGuid().Should().Be(expectedCustomer.Id);
     }

     [Fact]
     public async Task HandleAsync_ShouldCallMediatorAndSendResponse_WhenClientDoesNotExist()
     {
          // Arrange
          _mockMediator.Reset();
          var customerId = Guid.NewGuid();

          _mockMediator
               .Setup(m => m.Send(
                    It.IsAny<GetCustomerQuery>(),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync((CustomerCrud?)null);

          // Act
          var response = await _client.GetAsync($"/api/customer/{customerId}", _cancellationToken);

          // Assert
          response.StatusCode.Should().Be(HttpStatusCode.NotFound);

          _mockMediator.Verify(
               m => m.Send(
                    It.IsAny<GetCustomerQuery>(),
                    It.IsAny<CancellationToken>()),
               Times.Once);
     }
}
