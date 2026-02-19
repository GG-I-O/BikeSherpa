using System.Net;
using System.Text.Json;
using AutoFixture;
using AwesomeAssertions;
using BackendTests.Services;
using Ggio.BikeSherpa.Backend.Features.Deliveries.GetAll;
using JetBrains.Annotations;
using Mediator;
using Moq;
using DeliveryCrud = Ggio.BikeSherpa.Backend.Features.Deliveries.Model.DeliveryCrud;

namespace BackendTests.Features.Deliveries.GetAll;

[UsedImplicitly]
public class GetAllDeliveriesWebApplicationFactory() : TestWebApplicationFactory("read:deliveries", "read:deliveries");

public class GetAllDeliveriesEndpointTests(
     GetAllDeliveriesWebApplicationFactory factory,
     ITestContextAccessor testContextAccessor) : IClassFixture<GetAllDeliveriesWebApplicationFactory>
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
     public async Task HandleAsync_ShouldCallMediatorAndSendResponse_WhenDeliveriesExist()
     {
          // 
          _mockMediator.Reset();
          var expectedDeliveries = _fixture.Create<List<DeliveryCrud>>();

          _mockMediator
               .Setup(m => m.Send(
                    It.IsAny<GetAllDeliveriesQuery>(),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(expectedDeliveries);

          // Act
          var response = await _client.GetAsync("/api/deliveries", _cancellationToken);

          // Assert
          response.StatusCode.Should().Be(HttpStatusCode.OK);

          _mockMediator.Verify(
               m => m.Send(It.IsAny<GetAllDeliveriesQuery>(), It.IsAny<CancellationToken>()),
               Times.Once);

          var responseBody = await response.Content.ReadAsStringAsync(_cancellationToken);
          var responseArray = JsonSerializer.Deserialize<JsonElement>(responseBody, _jsonSerializerOptions);

          responseArray.GetArrayLength().Should().Be(expectedDeliveries.Count);
     }

     [Fact]
     public async Task HandleAsync_ShouldCallMediatorAndSendResponse_WhenNoDeliveriesExist()
     {
          // Arrange
          _mockMediator.Reset();
          _mockMediator
               .Setup(m => m.Send(
                    It.IsAny<GetAllDeliveriesQuery>(),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(new List<DeliveryCrud>());

          // Act
          var response = await _client.GetAsync("/api/deliveries", _cancellationToken);

          // Assert
          response.StatusCode.Should().Be(HttpStatusCode.OK);

          _mockMediator.Verify(
               m => m.Send(It.IsAny<GetAllDeliveriesQuery>(), It.IsAny<CancellationToken>()),
               Times.Once);

          var responseBody = await response.Content.ReadAsStringAsync(_cancellationToken);
          var responseArray = JsonSerializer.Deserialize<JsonElement>(responseBody, _jsonSerializerOptions);

          responseArray.GetArrayLength().Should().Be(0);
     }
}
