using System.Net;
using System.Text.Json;
using AutoFixture;
using AwesomeAssertions;
using BackendTests.Services;
using Ggio.BikeSherpa.Backend.Features.Reports.Delivery;
using Ggio.BikeSherpa.Backend.Features.Reports.Model;
using JetBrains.Annotations;
using Mediator;
using Moq;

namespace BackendTests.Features.Reports.Delivery;

[UsedImplicitly]
public class GetDeliveryReportWebApplicationFactory() : TestWebApplicationFactory("read:reports", "read:reports");

public class GetReportEndpointTests(
     GetDeliveryReportWebApplicationFactory factory,
     ITestContextAccessor testContextAccessor) : IClassFixture<GetDeliveryReportWebApplicationFactory>
{
     private readonly CancellationToken _cancellationToken = testContextAccessor.Current.CancellationToken;
     private readonly HttpClient _client = factory.CreateClient();
     private readonly Fixture _fixture = new();

     private readonly JsonSerializerOptions _jsonSerializerOptions = new()
     {
          PropertyNameCaseInsensitive = false,
          PropertyNamingPolicy = JsonNamingPolicy.CamelCase
     };

     private readonly Mock<IMediator> _mockMediator = factory.MockMediator;

     [Fact]
     public async Task HandleAsync_ShouldCallMediatorAndSendResponse_WhenRequestIsValid()
     {
          // Arrange
          _mockMediator.Reset();

          var deliveryId = Guid.NewGuid();
          var startDate = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);
          var endDate = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);

          var expectedReport = _fixture.Build<Report>()
               .With(r => r.CustomerName, "Test Customer")
               .With(r => r.StartDate, startDate)
               .With(r => r.EndDate, endDate)
               .With(r => r.TotalPrice, 42.50)
               .Create();

          _mockMediator
               .Setup(m => m.Send(
                    It.IsAny<GetReportQuery>(),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(expectedReport);

          // Act
          var response = await _client.GetAsync($"/api/reports/delivery/{deliveryId}", _cancellationToken);

          // Assert
          response.StatusCode.Should().Be(HttpStatusCode.OK);

          _mockMediator.Verify(
               m => m.Send(
                    It.Is<GetReportQuery>(q => q.DeliveryId == deliveryId),
                    It.IsAny<CancellationToken>()),
               Times.Once);

          var responseBody = await response.Content.ReadAsStringAsync(_cancellationToken);
          var actualReport = JsonSerializer.Deserialize<JsonElement>(responseBody, _jsonSerializerOptions);

          actualReport.GetProperty("customerName").GetString().Should().Be(expectedReport.CustomerName);
          actualReport.GetProperty("startDate").GetDateTimeOffset().Should().Be(expectedReport.StartDate);
          actualReport.GetProperty("endDate").GetDateTimeOffset().Should().Be(expectedReport.EndDate);
          actualReport.GetProperty("totalPrice").GetDouble().Should().Be(expectedReport.TotalPrice);
     }

     [Fact]
     public async Task HandleAsync_ShouldSendRouteDeliveryIdToMediator()
     {
          // Arrange
          _mockMediator.Reset();

          var deliveryId = Guid.NewGuid();

          _mockMediator
               .Setup(m => m.Send(
                    It.IsAny<GetReportQuery>(),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(_fixture.Create<Report>());

          // Act
          var response = await _client.GetAsync($"/api/reports/delivery/{deliveryId}", _cancellationToken);

          // Assert
          response.StatusCode.Should().Be(HttpStatusCode.OK);

          _mockMediator.Verify(
               m => m.Send(
                    It.Is<GetReportQuery>(q => q.DeliveryId == deliveryId),
                    It.IsAny<CancellationToken>()),
               Times.Once);
     }

     [Fact]
     public async Task HandleAsync_ShouldNotCallMediator_WhenDeliveryIdIsNotAGuid()
     {
          // Arrange
          _mockMediator.Reset();

          // Act
          var response = await _client.GetAsync("/api/reports/delivery/not-a-guid", _cancellationToken);

          // Assert
          response.StatusCode.Should().Be(HttpStatusCode.NotFound);

          _mockMediator.Verify(
               m => m.Send(
                    It.IsAny<GetReportQuery>(),
                    It.IsAny<CancellationToken>()),
               Times.Never);
     }
}