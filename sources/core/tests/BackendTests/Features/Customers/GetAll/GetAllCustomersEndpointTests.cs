using System.Net;
using System.Text.Json;
using AutoFixture;
using AwesomeAssertions;
using BackendTests.Services;
using Ggio.BikeSherpa.Backend.Features.Customers.GetAll;
using Mediator;
using Moq;
using CustomerCrud = Ggio.BikeSherpa.Backend.Features.Customers.Model.CustomerCrud;

namespace BackendTests.Features.Customers.GetAll;

public class GetAllCustomersWebApplicationFactory() : TestWebApplicationFactory("read:customers", "read:customers") {}

public class GetAllCustomersEndpointTests(
     GetAllCustomersWebApplicationFactory factory,
     ITestContextAccessor testContextAccessor) : IClassFixture<GetAllCustomersWebApplicationFactory>
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
     public async Task HandleAsync_ShouldCallMediatorAndSendResponse_WhenCustomersExist()
     {
          // 
          _mockMediator.Reset();
          var expectedCustomers = _fixture.Create<List<CustomerCrud>>();

          _mockMediator
               .Setup(m => m.Send(
                    It.IsAny<GetAllCustomersQuery>(),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(expectedCustomers);

          // Act
          var response = await _client.GetAsync("/api/customers", _cancellationToken);

          // Assert
          response.StatusCode.Should().Be(HttpStatusCode.OK);

          _mockMediator.Verify(
               m => m.Send(It.IsAny<GetAllCustomersQuery>(), It.IsAny<CancellationToken>()),
               Times.Once);

          var responseBody = await response.Content.ReadAsStringAsync(_cancellationToken);
          var responseArray = JsonSerializer.Deserialize<JsonElement>(responseBody, _jsonSerializerOptions);

          responseArray.GetArrayLength().Should().Be(expectedCustomers.Count);
     }

     [Fact]
     public async Task HandleAsync_ShouldCallMediatorAndSendResponse_WhenNoCustomersExist()
     {
          // Arrange
          _mockMediator.Reset();
          _mockMediator
               .Setup(m => m.Send(
                    It.IsAny<GetAllCustomersQuery>(),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(new List<CustomerCrud>());

          // Act
          var response = await _client.GetAsync("/api/customers", _cancellationToken);

          // Assert
          response.StatusCode.Should().Be(HttpStatusCode.OK);

          _mockMediator.Verify(
               m => m.Send(It.IsAny<GetAllCustomersQuery>(), It.IsAny<CancellationToken>()),
               Times.Once);

          var responseBody = await response.Content.ReadAsStringAsync(_cancellationToken);
          var responseArray = JsonSerializer.Deserialize<JsonElement>(responseBody, _jsonSerializerOptions);

          responseArray.GetArrayLength().Should().Be(0);
     }
}