using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Ardalis.Result;
using AutoFixture;
using AwesomeAssertions;
using BackendTests.Services;
using Ggio.BikeSherpa.Backend.Features.Deliveries.Add;
using Ggio.BikeSherpa.Backend.Features.Deliveries.Model;
using JetBrains.Annotations;
using Mediator;
using Moq;

namespace BackendTests.Features.Deliveries.Add;

[UsedImplicitly]
public class AddDeliveryWebApplicationFactory() : TestWebApplicationFactory("write:deliveries", "write:deliveries");

public class AddDeliveryEndpointTests
(
     AddDeliveryWebApplicationFactory factory,
     ITestContextAccessor testContextAccessor) : IClassFixture<AddDeliveryWebApplicationFactory>
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
     public async Task AddDelivery_ValidDelivery_ReturnsCreated()
     {
          // Arrange
          _mockMediator.Reset();
          var deliveryCrud = _fixture.Create<DeliveryCrud>();
          var expectedId = Guid.NewGuid();

          _mockMediator
               .Setup(m => m.Send(It.IsAny<AddDeliveryCommand>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(new Result<Guid>(expectedId));

          // Act
          var response = await _client.PostAsJsonAsync("/api/delivery", deliveryCrud, _cancellationToken);

          // Assert
          response.StatusCode.Should().Be(HttpStatusCode.Created);

          _mockMediator.Verify(
               m => m.Send(It.IsAny<AddDeliveryCommand>(), It.IsAny<CancellationToken>()),
               Times.Once);

          var responseBody = await response.Content.ReadAsStringAsync(_cancellationToken);
          var responseObject = JsonSerializer.Deserialize<JsonElement>(responseBody, _jsonSerializerOptions);
          var actualId = responseObject.GetProperty("id").GetGuid();

          actualId.Should().Be(expectedId);
     }
}
