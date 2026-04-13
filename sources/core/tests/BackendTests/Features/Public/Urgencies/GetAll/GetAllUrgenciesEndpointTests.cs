using System.Net;
using System.Text.Json;
using AutoFixture;
using AwesomeAssertions;
using BackendTests.Services;
using Ggio.BikeSherpa.Backend.Features.Public.Urgencies.GetAll;
using Ggio.BikeSherpa.Backend.Features.Public.Urgencies.Model;
using JetBrains.Annotations;
using Mediator;
using Moq;

namespace BackendTests.Features.Public.Urgencies.GetAll;

[UsedImplicitly]
public class GetAllUrgenciesWebApplicationFactory() : TestWebApplicationFactory();

public class GetAllUrgenciesEndpointTests(
     GetAllUrgenciesWebApplicationFactory factory,
     ITestContextAccessor testContextAccessor
) : IClassFixture<GetAllUrgenciesWebApplicationFactory>
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
     public async Task HandleAsync_ShouldCallMediatorAndSendResponse_WhenUrgenciesExist()
     {
          // Arrange
          _mockMediator.Reset();
          var expectedUrgencies = _fixture.Create<List<UrgencyDto>>();

          _mockMediator.Setup(m => m.Send(
                    It.IsAny<GetAllUrgenciesQuery>(),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(expectedUrgencies);

          // Act
          var response = await _client.GetAsync("/api/public/urgencies", _cancellationToken);

          // Assert
          response.StatusCode.Should().Be(HttpStatusCode.OK);

          _mockMediator.Verify(
               m => m.Send(
                    It.IsAny<GetAllUrgenciesQuery>(),
                    It.IsAny<CancellationToken>()
               ),
               Times.Once
          );

          var responseBody = await response.Content.ReadAsStringAsync(_cancellationToken);
          var responseArray = JsonSerializer.Deserialize<JsonElement>(responseBody, _jsonSerializerOptions);

          responseArray.GetArrayLength().Should().Be(expectedUrgencies.Count);
     }

     [Fact]
     public async Task HandleAsync_ShouldCallMediatorAndSendResponse_WhenNoUrgenciesExist()
     {
          // Arrange
          _mockMediator.Reset();
          _mockMediator.Setup(m => m.Send(
                    It.IsAny<GetAllUrgenciesQuery>(),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync([]);

          // Act
          var response = await _client.GetAsync("/api/public/urgencies", _cancellationToken);

          // Assert
          response.StatusCode.Should().Be(HttpStatusCode.OK);

          _mockMediator.Verify(
               m => m.Send(
                    It.IsAny<GetAllUrgenciesQuery>(),
                    It.IsAny<CancellationToken>()
               ),
               Times.Once
          );

          var responseBody = await response.Content.ReadAsStringAsync(_cancellationToken);
          var responseArray = JsonSerializer.Deserialize<JsonElement>(responseBody, _jsonSerializerOptions);

          responseArray.GetArrayLength().Should().Be(0);
     }
}
