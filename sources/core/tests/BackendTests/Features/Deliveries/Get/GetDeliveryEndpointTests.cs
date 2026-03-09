using System.Net;
using System.Text.Json;
using AutoFixture;
using AwesomeAssertions;
using BackendTests.Services;
using Ggio.BikeSherpa.Backend.Features.Deliveries.Get;
using JetBrains.Annotations;
using Ggio.BikeSherpa.Backend.Features.Deliveries.Model;
using Mediator;
using Moq;

namespace BackendTests.Features.Deliveries.Get;

[UsedImplicitly]
public class GetDeliveryWebApplicationFactory() : TestWebApplicationFactory("read:deliveries", "read:deliveries");

public class GetDeliveryEndpointTests(
     GetDeliveryWebApplicationFactory factory,
     ITestContextAccessor testContextAccessor) : IClassFixture<GetDeliveryWebApplicationFactory>
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
          var expectedDelivery = _fixture.Create<DeliveryCrud>();

          _mockMediator
               .Setup(m => m.Send(
                    It.IsAny<GetDeliveryQuery>(),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(expectedDelivery);

          // Act
          var response = await _client.GetAsync($"/api/delivery/{expectedDelivery.Id}", _cancellationToken);

          // Assert
          response.StatusCode.Should().Be(HttpStatusCode.OK);

          _mockMediator.Verify(
               m => m.Send(
                    It.IsAny<GetDeliveryQuery>(),
                    It.IsAny<CancellationToken>()),
               Times.Once);

          var responseBody = await response.Content.ReadAsStringAsync(_cancellationToken);
          var responseObject = JsonSerializer.Deserialize<JsonElement>(responseBody, _jsonSerializerOptions);
          var actualDelivery = responseObject.GetProperty("data");

          actualDelivery.GetProperty("id").GetGuid().Should().Be(expectedDelivery.Id);
     }

     [Fact]
     public async Task HandleAsync_ShouldCallMediatorAndSendResponse_WhenClientDoesNotExist()
     {
          // Arrange
          _mockMediator.Reset();
          var deliveryId = Guid.NewGuid();

          _mockMediator
               .Setup(m => m.Send(
                    It.IsAny<GetDeliveryQuery>(),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync((DeliveryCrud?)null);

          // Act
          var response = await _client.GetAsync($"/api/delivery/{deliveryId}", _cancellationToken);

          // Assert
          response.StatusCode.Should().Be(HttpStatusCode.NotFound);

          _mockMediator.Verify(
               m => m.Send(
                    It.IsAny<GetDeliveryQuery>(),
                    It.IsAny<CancellationToken>()),
               Times.Once);
     }
}
