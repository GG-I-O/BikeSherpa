using Ardalis.Result;
using AwesomeAssertions;
using Ggio.BikeSherpa.Backend.Features.Deliveries.Delete;
using Mediator;
using System.Net;
using BackendTests.Services;
using JetBrains.Annotations;
using Moq;

namespace BackendTests.Features.Deliveries.Delete;

[UsedImplicitly]
public class DeleteDeliveryWebApplicationFactory() : TestWebApplicationFactory("write:deliveries", "write:deliveries");

public class DeleteDeliveryEndpointTests(
     DeleteDeliveryWebApplicationFactory factory,
     ITestContextAccessor testContextAccessor) : IClassFixture<DeleteDeliveryWebApplicationFactory>
{
     private readonly HttpClient _client = factory.CreateClient();
     private readonly Mock<IMediator> _mockMediator = factory.MockMediator;
     private readonly CancellationToken _cancellationToken = testContextAccessor.Current.CancellationToken;

     [Fact]
     public async Task DeleteDelivery_ReturnsOkOnSuccessfulDelete()
     {
          // Arrange
          _mockMediator.Reset();
          var expectedId = Guid.NewGuid();

          _mockMediator
               .Setup(m => m.Send(
                    It.IsAny<DeleteDeliveryCommand>(),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(new Result());

          // Act
          var response = await _client.DeleteAsync($"/api/delivery/{expectedId}", _cancellationToken);

          // Assert
          response.StatusCode.Should().Be(HttpStatusCode.OK);

          _mockMediator.Verify(
               m => m.Send(
                    It.IsAny<DeleteDeliveryCommand>(),
                    It.IsAny<CancellationToken>()),
               Times.Once);
     }
}
