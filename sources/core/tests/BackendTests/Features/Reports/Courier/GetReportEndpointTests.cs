using System.Net;
using AutoFixture;
using AwesomeAssertions;
using BackendTests.Services;
using Ggio.BikeSherpa.Backend.Features.Reports.Courier;
using Ggio.BikeSherpa.Backend.Infrastructure.Storage;
using JetBrains.Annotations;
using Mediator;
using Moq;

namespace BackendTests.Features.Reports.Courier;

[UsedImplicitly]
public class GetReportWebApplicationFactory() : TestWebApplicationFactory("read:reports", "read:reports");

public class GetReportEndpointTests(
     GetReportWebApplicationFactory factory,
     ITestContextAccessor testContextAccessor) : IClassFixture<GetReportWebApplicationFactory>
{
     private readonly CancellationToken _cancellationToken = testContextAccessor.Current.CancellationToken;
     private readonly HttpClient _client = factory.CreateClient();
     private readonly Fixture _fixture = new();

     private readonly Mock<IMediator> _mockMediator = factory.MockMediator;
     private readonly Mock<IExportSaveService> _mockExportSaveService = factory.MockExportSaveService;

     [Fact]
     public async Task HandleAsync_ShouldCallMediatorAndSendResponse_WhenRequestIsValid()
     {
          // Arrange
          _mockMediator.Reset();

          var courierId = Guid.NewGuid();
          var startDate = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);
          var endDate = new DateTimeOffset(2026, 1, 31, 0, 0, 0, TimeSpan.Zero);

          var expectedReport = _fixture.Build<ReportFile>()
               .Create();

          _mockMediator
               .Setup(m => m.Send(
                    It.IsAny<GetReportQuery>(),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(expectedReport);
          
          _mockExportSaveService.Setup(x=>x.SaveCourierReportAsync(It.IsAny<string>(),
                    It.IsAny<byte[]>(), 
                    It.IsAny<string>()
                    , It.IsAny<CancellationToken>()))
               .ReturnsAsync("The super path !");

          // Act
          var response = await _client.GetAsync(
               $"/api/reports/courier/{courierId}?startDate={Uri.EscapeDataString(startDate.ToString("O"))}&endDate={Uri.EscapeDataString(endDate.ToString("O"))}",
               _cancellationToken);

          // Assert
          response.StatusCode.Should().Be(HttpStatusCode.OK);

          _mockMediator.Verify(
               m => m.Send(
                    It.Is<GetReportQuery>(q =>
                         q.CourierId == courierId &&
                         q.From == startDate &&
                         q.To == endDate),
                    It.IsAny<CancellationToken>()),
               Times.Once);

          var responseBody = await response.Content.ReadAsStringAsync(_cancellationToken);
          responseBody.Should().Contain("The super path !");
     }

     [Fact]
     public async Task HandleAsync_ShouldSendRequestValuesToMediator()
     {
          // Arrange
          _mockMediator.Reset();

          var courrierId = Guid.NewGuid();
          var startDate = new DateTimeOffset(2026, 2, 1, 8, 30, 0, TimeSpan.Zero);
          var endDate = new DateTimeOffset(2026, 2, 28, 18, 0, 0, TimeSpan.Zero);

          _mockMediator
               .Setup(m => m.Send(
                    It.IsAny<GetReportQuery>(),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(_fixture.Create<ReportFile>());

          // Act
          var response = await _client.GetAsync(
               $"/api/reports/courier/{courrierId}?startDate={Uri.EscapeDataString(startDate.ToString("O"))}&endDate={Uri.EscapeDataString(endDate.ToString("O"))}",
               _cancellationToken);

          // Assert
          response.StatusCode.Should().Be(HttpStatusCode.OK);

          _mockMediator.Verify(
               m => m.Send(
                    It.Is<GetReportQuery>(q =>
                         q.CourierId == courrierId &&
                         q.From == startDate &&
                         q.To == endDate),
                    It.IsAny<CancellationToken>()),
               Times.Once);
     }
}
