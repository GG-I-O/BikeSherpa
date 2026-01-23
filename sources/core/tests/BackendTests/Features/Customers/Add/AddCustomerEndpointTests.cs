using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using Ardalis.Result;
using AutoFixture;
using AwesomeAssertions;
using BackendTests.Services;
using Ggio.BikeSherpa.Backend.Features.Customers.Add;
using Ggio.BikeSherpa.Backend.Features.Customers.Model;
using Mediator;
using Moq;

namespace BackendTests.Features.Customers.Add;

public class AddCustomerWebApplicationFactory() : TestWebApplicationFactory("write:customers", "write:customers") {}

public class AddCustomerEndpointTests(
     AddCustomerWebApplicationFactory factory,
     ITestContextAccessor testContextAccessor) : IClassFixture<AddCustomerWebApplicationFactory>
{
     private readonly HttpClient _client = factory.CreateClient();
     private readonly Mock<IMediator> _mockMediator = factory.MockMediator;
     private readonly Fixture _fixture = new();
     private readonly CancellationToken _cancellationToken = testContextAccessor.Current.CancellationToken;
     private readonly JsonSerializerOptions _jsonSerializerOptions = new()
     {
          PropertyNameCaseInsensitive = false,
          PropertyNamingPolicy = JsonNamingPolicy.CamelCase
     };


     [Fact]
     public async Task AddCustomer_ValidCustomer_ReturnsCreated()
     {
          // Arrange
          _mockMediator.Reset();
          var customerCrud = _fixture.Create<CustomerCrud>();
          var expectedId = Guid.NewGuid();

          _mockMediator
               .Setup(m => m.Send(It.IsAny<AddCustomerCommand>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(new Result<Guid>(expectedId));

          // Act
          var response = await _client.PostAsJsonAsync("/api/customer", customerCrud, _cancellationToken);

          // Assert
          response.StatusCode.Should().Be(HttpStatusCode.Created);

          _mockMediator.Verify(
               m => m.Send(It.IsAny<AddCustomerCommand>(), It.IsAny<CancellationToken>()),
               Times.Once);

          var responseBody = await response.Content.ReadAsStringAsync(_cancellationToken);
          var responseObject = JsonSerializer.Deserialize<JsonElement>(responseBody, _jsonSerializerOptions);
          var actualId = responseObject.GetProperty("id").GetGuid();

          actualId.Should().Be(expectedId);
     }
}