using System.Net;
using System.Text.Json;
using AutoFixture;
using AwesomeAssertions;
using BackendTests.Services;
using Ggio.BikeSherpa.Backend.Features.StaticData.PricingStrategies.GetAll;
using Ggio.BikeSherpa.Backend.Features.StaticData.PricingStrategies.Model;
using JetBrains.Annotations;
using Mediator;
using Moq;

namespace BackendTests.Features.StaticData.PricingStrategies.GetAll;

[UsedImplicitly]
public class GetAllPricingStrategiesWebApplicationFactory() : TestWebApplicationFactory("read:deliveries", "read:deliveries");

public class GetAllPricingStrategiesEndpointTests(
     GetAllPricingStrategiesWebApplicationFactory factory,
     ITestContextAccessor testContextAccessor
     ): IClassFixture<GetAllPricingStrategiesWebApplicationFactory>
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
     public async Task HandleAsync_ShouldCallMediatorAndSendResponse_WhenPricingStrategiesExist()
     {
          // Arrange
          _mockMediator.Reset();
          var expectedPricingStrategies = _fixture.Create<List<PricingStrategyDto>>();

          _mockMediator.Setup(m => m.Send(
                    It.IsAny<GetAllPricingStrategiesQuery>(),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(expectedPricingStrategies);

          // Act
          var response = await _client.GetAsync("/api/public/pricingStrategies", _cancellationToken);

          // Assert
          response.StatusCode.Should().Be(HttpStatusCode.OK);

          _mockMediator.Verify(
               m => m.Send(
                    It.IsAny<GetAllPricingStrategiesQuery>(),
                    It.IsAny<CancellationToken>()
               ),
               Times.Once
          );

          var responseBody = await response.Content.ReadAsStringAsync(_cancellationToken);
          var responseArray = JsonSerializer.Deserialize<JsonElement>(responseBody, _jsonSerializerOptions);

          responseArray.GetArrayLength().Should().Be(expectedPricingStrategies.Count);
     }

     [Fact]
     public async Task HandleAsync_ShouldCallMediatorAndSendResponse_WhenNoUrgenciesExist()
     {
          // Arrange
          _mockMediator.Reset();
          _mockMediator.Setup(m => m.Send(
                    It.IsAny<GetAllPricingStrategiesQuery>(),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync([]);

          // Act
          var response = await _client.GetAsync("/api/public/pricingStrategies", _cancellationToken);

          // Assert
          response.StatusCode.Should().Be(HttpStatusCode.OK);

          _mockMediator.Verify(
               m => m.Send(
                    It.IsAny<GetAllPricingStrategiesQuery>(),
                    It.IsAny<CancellationToken>()
               ),
               Times.Once
          );

          var responseBody = await response.Content.ReadAsStringAsync(_cancellationToken);
          var responseArray = JsonSerializer.Deserialize<JsonElement>(responseBody, _jsonSerializerOptions);

          responseArray.GetArrayLength().Should().Be(0);
     }
}
