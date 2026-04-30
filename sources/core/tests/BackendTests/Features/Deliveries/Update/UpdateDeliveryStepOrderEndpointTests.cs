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
public class UpdateDeliveryStepOrderWebApplicationFactory() : TestWebApplicationFactory("write:deliveries", "write:deliveries");

public class UpdateDeliveryStepOrderEndpointTests
(
     UpdateDeliveryStepOrderWebApplicationFactory factory,
     ITestContextAccessor testContextAccessor) : IClassFixture<UpdateDeliveryStepOrderWebApplicationFactory>
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
     public async Task UpdateDeliveryStepOrder_ReturnsOk()
     {
          // Arrange
          _mockMediator.Reset();

          var deliveryId = Guid.NewGuid();
          var stepId = Guid.NewGuid();
          var request = new UpdateDeliveryStepOrderRequest(1);

          _mockMediator
               .Setup(m => m.Send(It.IsAny<UpdateDeliveryStepOrderCommand>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(Result.Success());

          var json = JsonSerializer.Serialize(request, _jsonSerializerOptions);
          using var content = new StringContent(json, Encoding.UTF8, "application/json");

          // Act
          var response = await _client.PutAsync(
               $"/api/delivery/{deliveryId}/step/{stepId}/changeOrder",
               content,
               _cancellationToken);

          // Assert
          response.StatusCode.Should().Be(HttpStatusCode.OK);

          _mockMediator.Verify(
               m => m.Send(
                    It.Is<UpdateDeliveryStepOrderCommand>(command =>
                         command.DeliveryId == deliveryId &&
                         command.StepId == stepId &&
                         command.Increment == request.Increment),
                    It.IsAny<CancellationToken>()),
               Times.Once);
     }

     [Fact]
     public async Task UpdateDeliveryStepOrder_ReturnsNotFound_WhenHandlerReturnsNotFound()
     {
          // Arrange
          _mockMediator.Reset();

          var deliveryId = Guid.NewGuid();
          var stepId = Guid.NewGuid();
          var request = new UpdateDeliveryStepOrderRequest(1);

          _mockMediator
               .Setup(m => m.Send(It.IsAny<UpdateDeliveryStepOrderCommand>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(Result.NotFound());

          var json = JsonSerializer.Serialize(request, _jsonSerializerOptions);
          using var content = new StringContent(json, Encoding.UTF8, "application/json");

          // Act
          var response = await _client.PutAsync(
               $"/api/delivery/{deliveryId}/step/{stepId}/changeOrder",
               content,
               _cancellationToken);

          // Assert
          response.StatusCode.Should().Be(HttpStatusCode.NotFound);

          _mockMediator.Verify(
               m => m.Send(It.IsAny<UpdateDeliveryStepOrderCommand>(), It.IsAny<CancellationToken>()),
               Times.Once);
     }
}