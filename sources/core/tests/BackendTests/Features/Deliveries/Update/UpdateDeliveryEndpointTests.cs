using System;
using Ardalis.Result;
using AutoFixture;
using AwesomeAssertions;
using Ggio.BikeSherpa.Backend.Features.Deliveries.Model;
using Ggio.BikeSherpa.Backend.Features.Deliveries.Update;
using Mediator;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;
using BackendTests.Services;
using JetBrains.Annotations;
using Moq;
using Xunit;

namespace BackendTests.Features.Deliveries.Update;

[UsedImplicitly]
public class UpdateDeliveryWebApplicationFactory() : TestWebApplicationFactory("write:deliveries", "write:deliveries");

public class UpdateDeliveryEndpointTests(
     UpdateDeliveryWebApplicationFactory factory,
     ITestContextAccessor testContextAccessor) : IClassFixture<UpdateDeliveryWebApplicationFactory>
{
     private readonly HttpClient _client = factory.CreateClient();
     private readonly Mock<IMediator> _mockMediator = factory.MockMediator;
     private readonly Fixture _fixture = new();
     private readonly CancellationToken _cancellationToken = testContextAccessor.Current.CancellationToken;

     [Fact]
     public async Task UpdateDelivery_ValidDelivery_ReturnsOk()
     {
          // Arrange
          var deliveryCrud = _fixture.Create<DeliveryCrud>();
          var expectedId = Guid.NewGuid();

          _mockMediator
               .Setup(m => m.Send(
                    It.IsAny<UpdateDeliveryCommand>(),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(Result.Success());

          // Act
          var response = await _client.PutAsJsonAsync($"/api/delivery/{expectedId}", deliveryCrud, _cancellationToken);

          // Assert
          response.StatusCode.Should().Be(HttpStatusCode.OK);

          _mockMediator.Verify(
               m => m.Send(
                    It.IsAny<UpdateDeliveryCommand>(),
                    It.IsAny<CancellationToken>()),
               Times.Once);
     }
}
