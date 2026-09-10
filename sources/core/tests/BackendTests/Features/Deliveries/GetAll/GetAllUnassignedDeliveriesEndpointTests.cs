using System.Net;
using System.Text.Json;
using AutoFixture;
using AwesomeAssertions;
using BackendTests.Services;
using Ggio.BikeSherpa.Backend.Features.Deliveries.GetAll;
using Ggio.BikeSherpa.Backend.Features.Deliveries.Model;
using JetBrains.Annotations;
using Mediator;
using Moq;

namespace BackendTests.Features.Deliveries.GetAll;

[UsedImplicitly]
public class GetAllUnassignedStepsWebApplicationFactory() : TestWebApplicationFactory("read:myDeliveries", "read:myDeliveries", "mockEmail@mail.com");

public class GetAllUnassignedDeliveriesEndpointTests(
     GetAllUnassignedStepsWebApplicationFactory factory,
     ITestContextAccessor testContextAccessor) : IClassFixture<GetAllUnassignedStepsWebApplicationFactory>
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
     public async Task HandleAsync_ShouldCallMediatorAndSendResponse_WhenDailyStepsExist()
     {
          // Arrange
          _mockMediator.Reset();
          var expectedDeliveries = _fixture.Create<List<DeliveryCrud>>();
          var date = new DateTimeOffset(2026, 5, 12, 0, 0, 0, TimeSpan.Zero);

          _mockMediator
               .Setup(m => m.Send(
                    It.IsAny<GetAllUnassignedDeliveriesQuery>(),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(new GetAllDailyDeliveriesResult.Success(expectedDeliveries));

          // Act
          var response = await _client.GetAsync($"/api/deliveries/unassignedDeliveries/{date:O}", _cancellationToken);

          // Assert
          response.StatusCode.Should().Be(HttpStatusCode.OK);

          _mockMediator.Verify(
               m => m.Send(
                    It.Is<GetAllUnassignedDeliveriesQuery>(q => q.Date == date),
                    It.IsAny<CancellationToken>()),
               Times.Once);

          var responseBody = await response.Content.ReadAsStringAsync(_cancellationToken);
          var responseArray = JsonSerializer.Deserialize<JsonElement>(responseBody, _jsonSerializerOptions);

          responseArray.GetArrayLength().Should().Be(expectedDeliveries.Count);
     }

     [Fact]
     public async Task HandleAsync_ShouldCallMediatorAndSendEmptyResponse_WhenNoDailyStepsExist()
     {
          // Arrange
          _mockMediator.Reset();
          var date = new DateTimeOffset(2026, 5, 12, 0, 0, 0, TimeSpan.Zero);

          _mockMediator
               .Setup(m => m.Send(
                    It.IsAny<GetAllUnassignedDeliveriesQuery>(),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(new GetAllDailyDeliveriesResult.Success([]));

          // Act
          var response = await _client.GetAsync($"/api/deliveries/unassignedDeliveries/{date:O}", _cancellationToken);

          // Assert
          response.StatusCode.Should().Be(HttpStatusCode.OK);

          _mockMediator.Verify(
               m => m.Send(
                    It.Is<GetAllUnassignedDeliveriesQuery>(q => q.Date == date),
                    It.IsAny<CancellationToken>()),
               Times.Once);

          var responseBody = await response.Content.ReadAsStringAsync(_cancellationToken);
          var responseArray = JsonSerializer.Deserialize<JsonElement>(responseBody, _jsonSerializerOptions);

          responseArray.GetArrayLength().Should().Be(0);
     }
}