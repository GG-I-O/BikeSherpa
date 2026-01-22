using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;
using Ardalis.Result;
using AutoFixture;
using AwesomeAssertions;
using FastEndpoints;
using Ggio.BikeSherpa.Backend.Features.Customers.Add;
using Ggio.BikeSherpa.Backend.Features.Customers.Model;
using Mediator;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace BackendTests.Features.Customers.Add;

public class AddCustomerEndpointTests : IClassFixture<WebApplicationFactory<Program>>
{
     private readonly CancellationToken _cancellationToken;
     private readonly HttpClient _client;
     private readonly Fixture _fixture = new();

     private readonly JsonSerializerOptions _jsonSerializerOptions = new()
     {
          PropertyNameCaseInsensitive = false,
          PropertyNamingPolicy = JsonNamingPolicy.CamelCase
     };

     private readonly Mock<IMediator> _mockMediator = new();

     public AddCustomerEndpointTests(WebApplicationFactory<Program> webApplicationFactory, ITestContextAccessor testContextAccessor)
     {
          _cancellationToken = testContextAccessor.Current.CancellationToken;

          var webHostBuilder = webApplicationFactory.WithWebHostBuilder(builder =>
          {
               builder.UseEnvironment("IntegrationTest");

               builder.ConfigureServices(services =>
               {
                    // FastEndpoints is not added by Program in IntegrationTest env, so add it for this test host.
                    services.AddFastEndpoints();

                    // Replace mediator with our mock
                    services.AddSingleton(_mockMediator.Object);

                    // Add a test auth scheme + policy so endpoint Policies("write:customers") passes
                    services
                         .AddAuthentication("Test")
                         .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>("Test", _ => { });

                    services.AddAuthorization(options =>
                    {
                         options.AddPolicy("write:customers",
                              policy => policy.RequireClaim("scope", "write:customers"));
                    });
               });

               builder.Configure(app =>
               {
                    app.UsePathBase("/api");

                    app.UseAuthentication();
                    app.UseAuthorization();

                    app.UseFastEndpoints();
               });
          });

          _client = webHostBuilder.CreateClient();
     }

     [Fact]
     public async Task AddCustomer_ValidCustomer_ReturnsCreated()
     {
          // Arrange
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

     private sealed class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
     {
          public TestAuthHandler(
               IOptionsMonitor<AuthenticationSchemeOptions> options,
               ILoggerFactory logger,
               UrlEncoder encoder)
               : base(options, logger, encoder)
          {
          }

          override protected Task<AuthenticateResult> HandleAuthenticateAsync()
          {
               var identity = new ClaimsIdentity("Test");
               identity.AddClaim(new Claim("scope", "write:customers"));

               var principal = new ClaimsPrincipal(identity);
               var ticket = new AuthenticationTicket(principal, "Test");

               return Task.FromResult(AuthenticateResult.Success(ticket));
          }
     }
}
