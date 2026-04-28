using System.Net;
using System.Text;
using System.Text.Json;
using Ardalis.Result;
using AwesomeAssertions;
using BackendTests.Services;
using Ggio.BikeSherpa.Backend.Features.Deliveries.Update;
using JetBrains.Annotations;
using Mediator;
using Moq;

namespace BackendTests.Features.Deliveries.Update;

[UsedImplicitly]
public class UpdateDeliveryStepTimeWebApplicationFactory() : TestWebApplicationFactory("write:deliveries", "write:deliveries");

public class UpdateDeliveryStepTimeEndpointTests
(
     UpdateDeliveryStepTimeWebApplicationFactory factory,
     ITestContextAccessor testContextAccessor) : IClassFixture<UpdateDeliveryStepTimeWebApplicationFactory>
{
     private readonly HttpClient _client = factory.CreateClient();
     private readonly Mock<IMediator> _mockMediator = factory.MockMediator;
     private readonly CancellationToken _cancellationToken = testContextAccessor.Current.CancellationToken;

     private readonly JsonSerializerOptions _jsonSerializerOptions = new()
     {
          PropertyNameCaseInsensitive = false,
          PropertyNamingPolicy = JsonNamingPolicy.CamelCase
     };

     [Fact]
     public async Task UpdateDeliveryStepTime_ReturnsOk()
     {
          // Arrange
          _mockMediator.Reset();

          var deliveryId = Guid.NewGuid();
          var stepId = Guid.NewGuid();
          var request = new UpdateDeliveryStepTimeRequest(DateTimeOffset.UtcNow.AddHours(1));

          _mockMediator
               .Setup(m => m.Send(It.IsAny<UpdateDeliveryStepTimeCommand>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(Result.Success());

          var json = JsonSerializer.Serialize(request, _jsonSerializerOptions);
          using var content = new StringContent(json, Encoding.UTF8, "application/json");

          // Act
          var response = await _client.PutAsync(
               $"/api/delivery/{deliveryId}/step/{stepId}/changeTime",
               content,
               _cancellationToken);

          // Assert
          response.StatusCode.Should().Be(HttpStatusCode.OK);

          _mockMediator.Verify(
               m => m.Send(
                    It.Is<UpdateDeliveryStepTimeCommand>(command =>
                         command.DeliveryId == deliveryId &&
                         command.StepId == stepId &&
                         command.Date == request.Date),
                    It.IsAny<CancellationToken>()),
               Times.Once);
     }

     [Fact]
     public async Task UpdateDeliveryStepTime_ReturnsNotFound_WhenHandlerReturnsNotFound()
     {
          // Arrange
          _mockMediator.Reset();

          var deliveryId = Guid.NewGuid();
          var stepId = Guid.NewGuid();
          var request = new UpdateDeliveryStepTimeRequest(DateTimeOffset.UtcNow.AddHours(1));

          _mockMediator
               .Setup(m => m.Send(It.IsAny<UpdateDeliveryStepTimeCommand>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(Result.NotFound());

          var json = JsonSerializer.Serialize(request, _jsonSerializerOptions);
          using var content = new StringContent(json, Encoding.UTF8, "application/json");

          // Act
          var response = await _client.PutAsync(
               $"/api/delivery/{deliveryId}/step/{stepId}/changeTime",
               content,
               _cancellationToken);

          // Assert
          response.StatusCode.Should().Be(HttpStatusCode.NotFound);

          _mockMediator.Verify(
               m => m.Send(It.IsAny<UpdateDeliveryStepTimeCommand>(), It.IsAny<CancellationToken>()),
               Times.Once);
     }
}