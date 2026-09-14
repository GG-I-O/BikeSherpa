using System.Net;
using Ardalis.Result;
using AwesomeAssertions;
using BackendTests.Services;
using Ggio.BikeSherpa.Backend.Features.Deliveries.Update;
using JetBrains.Annotations;
using Mediator;
using Moq;

namespace BackendTests.Features.Deliveries.Update;

[UsedImplicitly]
public class UpdateDeliveryStepCourierMyselfWebApplicationFactory() : TestWebApplicationFactory("write:myDeliveries", "write:myDeliveries", "user@example.com");

public class UpdateDeliveryStepCourierMyselfEndpointTests
(
     UpdateDeliveryStepCourierMyselfWebApplicationFactory factory,
     ITestContextAccessor testContextAccessor) : IClassFixture<UpdateDeliveryStepCourierMyselfWebApplicationFactory>
{
     private readonly HttpClient _client = factory.CreateClient();
     private readonly Mock<IMediator> _mockMediator = factory.MockMediator;
     private readonly CancellationToken _cancellationToken = testContextAccessor.Current.CancellationToken;

     [Fact]
     public async Task UpdateDeliveryStepCourierMyself_ReturnsOk()
     {
          // Arrange
          _mockMediator.Reset();

          var deliveryId = Guid.NewGuid();
          var stepId = Guid.NewGuid();

          _mockMediator
               .Setup(m => m.Send(It.IsAny<UpdateDeliveryStepCourierMyselfCommand>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(Result.Success());

          // Act
          var response = await _client.PutAsync(
               $"/api/delivery/{deliveryId}/step/{stepId}/courier/myself",
               null,
               _cancellationToken);

          // Assert
          response.StatusCode.Should().Be(HttpStatusCode.OK);

          _mockMediator.Verify(
               m => m.Send(
                    It.Is<UpdateDeliveryStepCourierMyselfCommand>(command =>
                         command.DeliveryId == deliveryId &&
                         command.StepId == stepId &&
                         command.UserEmail == "user@example.com"),
                    It.IsAny<CancellationToken>()),
               Times.Once);
     }

     [Fact]
     public async Task UpdateDeliveryStepCourierMyself_ReturnsNotFound_WhenHandlerReturnsNotFound()
     {
          // Arrange
          _mockMediator.Reset();

          var deliveryId = Guid.NewGuid();
          var stepId = Guid.NewGuid();

          _mockMediator
               .Setup(m => m.Send(It.IsAny<UpdateDeliveryStepCourierMyselfCommand>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(Result.NotFound());

          // Act
          var response = await _client.PutAsync(
               $"/api/delivery/{deliveryId}/step/{stepId}/courier/myself",
               null,
               _cancellationToken);

          // Assert
          response.StatusCode.Should().Be(HttpStatusCode.NotFound);

          _mockMediator.Verify(
               m => m.Send(It.IsAny<UpdateDeliveryStepCourierMyselfCommand>(), It.IsAny<CancellationToken>()),
               Times.Once);
     }
}