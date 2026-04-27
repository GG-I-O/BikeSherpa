using System.Net;
using Ardalis.Result;
using AwesomeAssertions;
using BackendTests.Services;
using Ggio.BikeSherpa.Backend.Features.Deliveries.Delete;
using JetBrains.Annotations;
using Mediator;
using Moq;

namespace BackendTests.Features.Deliveries.Delete;

[UsedImplicitly]
public class DeleteDeliveryStepCourierWebApplicationFactory() : TestWebApplicationFactory("write:deliveries", "write:deliveries");

public class DeleteDeliveryStepCourierEndpointTests
(
     DeleteDeliveryStepCourierWebApplicationFactory factory,
     ITestContextAccessor testContextAccessor) : IClassFixture<DeleteDeliveryStepCourierWebApplicationFactory>
{
     private readonly HttpClient _client = factory.CreateClient();
     private readonly Mock<IMediator> _mockMediator = factory.MockMediator;
     private readonly CancellationToken _cancellationToken = testContextAccessor.Current.CancellationToken;

     [Fact]
     public async Task DeleteDeliveryStepCourier_ValidRequest_ReturnsOk()
     {
          // Arrange
          _mockMediator.Reset();

          var deliveryId = Guid.NewGuid();
          var stepId = Guid.NewGuid();

          _mockMediator
               .Setup(m => m.Send(It.IsAny<DeleteDeliveryStepCourierCommand>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(Result.Success());

          // Act
          var response = await _client.DeleteAsync(
               $"/api/delivery/{deliveryId}/step/{stepId}/courier",
               _cancellationToken);

          // Assert
          response.StatusCode.Should().Be(HttpStatusCode.OK);

          _mockMediator.Verify(
               m => m.Send(
                    It.Is<DeleteDeliveryStepCourierCommand>(c =>
                         c.DeliveryId == deliveryId &&
                         c.StepId == stepId),
                    It.IsAny<CancellationToken>()),
               Times.Once);
     }

     [Fact]
     public async Task DeleteDeliveryStepCourier_WhenHandlerReturnsNotFound_ReturnsNotFound()
     {
          // Arrange
          _mockMediator.Reset();

          var deliveryId = Guid.NewGuid();
          var stepId = Guid.NewGuid();

          _mockMediator
               .Setup(m => m.Send(It.IsAny<DeleteDeliveryStepCourierCommand>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(Result.NotFound());

          // Act
          var response = await _client.DeleteAsync(
               $"/api/delivery/{deliveryId}/step/{stepId}/courier",
               _cancellationToken);

          // Assert
          response.StatusCode.Should().Be(HttpStatusCode.NotFound);

          _mockMediator.Verify(
               m => m.Send(It.IsAny<DeleteDeliveryStepCourierCommand>(), It.IsAny<CancellationToken>()),
               Times.Once);
     }
}