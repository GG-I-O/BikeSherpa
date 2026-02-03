using Ardalis.Result;
using AwesomeAssertions;
using Ggio.BikeSherpa.Backend.Features.Customers.Delete;
using Mediator;
using System.Net;
using BackendTests.Services;
using JetBrains.Annotations;
using Moq;

namespace BackendTests.Features.Customers.Delete;

[UsedImplicitly]
public class DeleteCustomerWebApplicationFactory() : TestWebApplicationFactory("write:customers", "write:customers");

public class DeleteCustomerEndpointTests(
     DeleteCustomerWebApplicationFactory factory,
     ITestContextAccessor testContextAccessor) : IClassFixture<DeleteCustomerWebApplicationFactory>
{
     private readonly HttpClient _client = factory.CreateClient();
     private readonly Mock<IMediator> _mockMediator = factory.MockMediator;
     private readonly CancellationToken _cancellationToken = testContextAccessor.Current.CancellationToken;

     [Fact]
     public async Task DeleteCustomer_ReturnsOkOnSuccessfulDelete()
     {
          // Arrange
          _mockMediator.Reset();
          var expectedId = Guid.NewGuid();

          _mockMediator
               .Setup(m => m.Send(
                    It.IsAny<DeleteCustomerCommand>(),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(new Result());

          // Act
          var response = await _client.DeleteAsync($"/api/customer/{expectedId}", _cancellationToken);

          // Assert
          response.StatusCode.Should().Be(HttpStatusCode.OK);

          _mockMediator.Verify(
               m => m.Send(
                    It.IsAny<DeleteCustomerCommand>(),
                    It.IsAny<CancellationToken>()),
               Times.Once);
     }
}
