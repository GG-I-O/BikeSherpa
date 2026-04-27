using System.Net;
using System.Text;
using System.Text.Json;
using Ardalis.Result;
using AwesomeAssertions;
using BackendTests.Services;
using Ggio.BikeSherpa.Backend.Features.Deliveries.Model;
using Ggio.BikeSherpa.Backend.Features.Deliveries.Patch;
using JetBrains.Annotations;
using Mediator;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson.Operations;
using Moq;

namespace BackendTests.Features.Deliveries.Patch;

[UsedImplicitly]
public class PatchDeliveryStepWebApplicationFactory() : TestWebApplicationFactory("write:deliveries", "write:deliveries");

public class PatchDeliveryStepEndpointTests
(
     PatchDeliveryStepWebApplicationFactory factory,
     ITestContextAccessor testContextAccessor) : IClassFixture<PatchDeliveryStepWebApplicationFactory>
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
     public async Task PatchDeliveryStep_OrderPatch_ReturnsOk()
     {
          // Arrange
          _mockMediator.Reset();

          var deliveryId = Guid.NewGuid();
          var stepId = Guid.NewGuid();

          var patchDocument = new JsonPatchDocument<PatchDeliveryRequest>();
          patchDocument.Operations.Add(new Operation<PatchDeliveryRequest>()
          {
               op = "replace",
               path = "/Order",
               value = 3
          });

          _mockMediator
               .Setup(m => m.Send(It.IsAny<PatchDeliveryStepOrderCommand>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(Result.Success());

          var json = JsonSerializer.Serialize(patchDocument, _jsonSerializerOptions);
          using var content = new StringContent(json, Encoding.UTF8, "application/json-patch+json");

          // Act
          var response = await _client.PatchAsync($"/api/delivery/{deliveryId}/step/{stepId}", content, _cancellationToken);

          // Assert
          response.StatusCode.Should().Be(HttpStatusCode.OK);

          _mockMediator.Verify(
               m => m.Send(It.IsAny<PatchDeliveryStepOrderCommand>(), It.IsAny<CancellationToken>()),
               Times.Once);

          _mockMediator.Verify(
               m => m.Send(It.IsAny<PatchDeliveryStepTimeCommand>(), It.IsAny<CancellationToken>()),
               Times.Never);
     }

     [Fact]
     public async Task PatchDeliveryStep_EstimatedDeliveryDatePatch_ReturnsOk()
     {
          // Arrange
          _mockMediator.Reset();

          var deliveryId = Guid.NewGuid();
          var stepId = Guid.NewGuid();

          var patchDocument = new JsonPatchDocument<PatchDeliveryRequest>();
          patchDocument.Operations.Add(new Operation<PatchDeliveryRequest>()
          {
               op = "replace",
               path = "/EstimatedDeliveryDate",
               value = DateTime.UtcNow.AddDays(1).ToString("O")
          });

          _mockMediator
               .Setup(m => m.Send(It.IsAny<PatchDeliveryStepTimeCommand>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(Result.Success());

          var json = JsonSerializer.Serialize(patchDocument, _jsonSerializerOptions);
          using var content = new StringContent(json, Encoding.UTF8, "application/json-patch+json");

          // Act
          var response = await _client.PatchAsync($"/api/delivery/{deliveryId}/step/{stepId}", content, _cancellationToken);

          // Assert
          response.StatusCode.Should().Be(HttpStatusCode.OK);

          _mockMediator.Verify(
               m => m.Send(It.IsAny<PatchDeliveryStepTimeCommand>(), It.IsAny<CancellationToken>()),
               Times.Once);

          _mockMediator.Verify(
               m => m.Send(It.IsAny<PatchDeliveryStepOrderCommand>(), It.IsAny<CancellationToken>()),
               Times.Never);
     }

     [Fact]
     public async Task PatchDeliveryStep_InvalidPatchPath_ReturnsBadRequest()
     {
          // Arrange
          _mockMediator.Reset();

          var deliveryId = Guid.NewGuid();
          var stepId = Guid.NewGuid();

          var patchDocument = new JsonPatchDocument<PatchDeliveryRequest>();
          patchDocument.Operations.Add(new Operation<PatchDeliveryRequest>()
          {
               op = "replace",
               path = "/InvalidPath",
               value = "invalid"
          });

          var json = JsonSerializer.Serialize(patchDocument, _jsonSerializerOptions);
          using var content = new StringContent(json, Encoding.UTF8, "application/json-patch+json");

          // Act
          var response = await _client.PatchAsync($"/api/delivery/{deliveryId}/step/{stepId}", content, _cancellationToken);

          // Assert
          response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

          _mockMediator.Verify(
               m => m.Send(It.IsAny<PatchDeliveryStepOrderCommand>(), It.IsAny<CancellationToken>()),
               Times.Never);

          _mockMediator.Verify(
               m => m.Send(It.IsAny<PatchDeliveryStepTimeCommand>(), It.IsAny<CancellationToken>()),
               Times.Never);
     }
}