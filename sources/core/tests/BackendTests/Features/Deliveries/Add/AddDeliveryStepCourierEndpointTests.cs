using System.Net;
using Ardalis.Result;
using AwesomeAssertions;
using BackendTests.Services;
using Ggio.BikeSherpa.Backend.Features.Deliveries.Add;
using JetBrains.Annotations;
using Mediator;
using Moq;

namespace BackendTests.Features.Deliveries.Add;

[UsedImplicitly]
public class AddDeliveryStepCourierWebApplicationFactory() : TestWebApplicationFactory("write:deliveries", "write:deliveries");

public class AddDeliveryStepCourierEndpointTests
(
     AddDeliveryStepCourierWebApplicationFactory factory,
     ITestContextAccessor testContextAccessor) : IClassFixture<AddDeliveryStepCourierWebApplicationFactory>
{
     private readonly HttpClient _client = factory.CreateClient();
     private readonly Mock<IMediator> _mockMediator = factory.MockMediator;
     private readonly CancellationToken _cancellationToken = testContextAccessor.Current.CancellationToken;

     [Fact]
     public async Task AddDeliveryStepCourier_ValidRequest_ReturnsOk()
     {
          // Arrange
          _mockMediator.Reset();

          var deliveryId = Guid.NewGuid();
          var stepId = Guid.NewGuid();
          var courierId = Guid.NewGuid();

          _mockMediator
               .Setup(m => m.Send(It.IsAny<AddDeliveryStepCourierCommand>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(Result.Success());

          // Act
          var response = await _client.PostAsync(
               $"/api/delivery/{deliveryId}/step/{stepId}/courier/{courierId}",
               null,
               _cancellationToken);

          // Assert
          response.StatusCode.Should().Be(HttpStatusCode.OK);

          _mockMediator.Verify(
               m => m.Send(
                    It.Is<AddDeliveryStepCourierCommand>(c =>
                         c.DeliveryId == deliveryId &&
                         c.StepId == stepId &&
                         c.CourierId == courierId),
                    It.IsAny<CancellationToken>()),
               Times.Once);
     }

     [Fact]
     public async Task AddDeliveryStepCourier_WhenHandlerReturnsNotFound_ReturnsNotFound()
     {
          // Arrange
          _mockMediator.Reset();

          var deliveryId = Guid.NewGuid();
          var stepId = Guid.NewGuid();
          var courierId = Guid.NewGuid();

          _mockMediator
               .Setup(m => m.Send(It.IsAny<AddDeliveryStepCourierCommand>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(Result.NotFound());

          // Act
          var response = await _client.PostAsync(
               $"/api/delivery/{deliveryId}/step/{stepId}/courier/{courierId}",
               null,
               _cancellationToken);

          // Assert
          response.StatusCode.Should().Be(HttpStatusCode.NotFound);

          _mockMediator.Verify(
               m => m.Send(It.IsAny<AddDeliveryStepCourierCommand>(), It.IsAny<CancellationToken>()),
               Times.Once);
     }
}