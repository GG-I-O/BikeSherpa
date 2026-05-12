using System.Net;
using System.Text.Json;
using Ardalis.Result;
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
public class GetAllDailyStepsWebApplicationFactory() : TestWebApplicationFactory("read:steps", "read:steps", "mockEmail@mail.com");

public class GetAllDailyStepsEndpointTests(
     GetAllDailyStepsWebApplicationFactory factory,
     ITestContextAccessor testContextAccessor) : IClassFixture<GetAllDailyStepsWebApplicationFactory>
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
                    It.IsAny<GetAllDailyStepsQuery>(),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(Result<List<DeliveryCrud>>.Success(expectedDeliveries));

          // Act
          var response = await _client.GetAsync($"/api/deliveries/dailySteps/{date:O}", _cancellationToken);

          // Assert
          response.StatusCode.Should().Be(HttpStatusCode.OK);

          _mockMediator.Verify(
               m => m.Send(
                    It.Is<GetAllDailyStepsQuery>(q => q.Date == date),
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
                    It.IsAny<GetAllDailyStepsQuery>(),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(Result<List<DeliveryCrud>>.Success([]));

          // Act
          var response = await _client.GetAsync($"/api/deliveries/dailySteps/{date:O}", _cancellationToken);

          // Assert
          response.StatusCode.Should().Be(HttpStatusCode.OK);

          _mockMediator.Verify(
               m => m.Send(
                    It.Is<GetAllDailyStepsQuery>(q => q.Date == date),
                    It.IsAny<CancellationToken>()),
               Times.Once);

          var responseBody = await response.Content.ReadAsStringAsync(_cancellationToken);
          var responseArray = JsonSerializer.Deserialize<JsonElement>(responseBody, _jsonSerializerOptions);

          responseArray.GetArrayLength().Should().Be(0);
     }

     [Fact]
     public async Task HandleAsync_ShouldThrowUnauthorizedAccessException_WhenMediatorReturnsUnauthorized()
     {
          // Arrange
          _mockMediator.Reset();
          var date = new DateTimeOffset(2026, 5, 12, 0, 0, 0, TimeSpan.Zero);

          _mockMediator
               .Setup(m => m.Send(
                    It.IsAny<GetAllDailyStepsQuery>(),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(Result<List<DeliveryCrud>>.Unauthorized());

          // Act
          var act = async () => await _client.GetAsync($"/api/deliveries/dailySteps/{date:O}", _cancellationToken);

          // Assert
          await act.Should().ThrowAsync<UnauthorizedAccessException>()
               .WithMessage("User unauthorized");

          _mockMediator.Verify(
               m => m.Send(
                    It.IsAny<GetAllDailyStepsQuery>(),
                    It.IsAny<CancellationToken>()),
               Times.Once);
     }
}