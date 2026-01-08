using System.Net;
using System.Text.Json;
using AwesomeAssertions;
using FluentValidation;
using FluentValidation.Results;
using Ggio.BikeSherpa.Backend.Model;
using Ggio.BikeSherpa.Backend.Services.Middleware;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Testing;

namespace BackendTests.Services.Middleware;

[TestSubject(typeof(ValidationExceptionMiddleware))]
public class ValidationExceptionMiddlewareTests : IClassFixture<WebApplicationFactory<Program>>
{
     private readonly HttpClient _client;
     private readonly JsonSerializerOptions _jsonSerializerOptions = new()
     {
          PropertyNameCaseInsensitive = false,
          PropertyNamingPolicy = JsonNamingPolicy.CamelCase
     };
     private CancellationToken _cancellationToken;

     public ValidationExceptionMiddlewareTests(WebApplicationFactory<Program> webApplicationFactory, ITestContextAccessor testContextAccessor)
     {
          _cancellationToken = testContextAccessor.Current.CancellationToken;
          var webHostBuilder = webApplicationFactory.WithWebHostBuilder(builder =>
          {
               builder.Configure(app =>
               {
                    app.UseMiddleware<ValidationExceptionMiddleware>();

                    app.Map("/success", appMap => appMap.Run(async context => { await context.Response.WriteAsync("Success"); }));

                    app.Map("/exception", appMap => appMap.Run(_ => throw new ValidationException([
                         new ValidationFailure("code", "code non unique"),
                         new ValidationFailure("name", "nom non unique"),
                    ])));
               });
               builder.UseEnvironment("IntegrationTest");
          });

          _client = webHostBuilder.CreateClient();
     }

     [Fact]
     public async Task InvokeAsync_ShouldSucceed_WhenNoExceptionOccurs()
     {
          // Act
          var response = await _client.GetAsync("/success", _cancellationToken);

          // Assert
          response.StatusCode.Should().Be(HttpStatusCode.OK);
          var content = await response.Content.ReadAsStringAsync(_cancellationToken);
          content.Should().Be("Success");
     }

     [Fact]
     public async Task InvokeAsync_ShouldFail_WhenExceptionOccurs()
     {
          // Act
          var response = await _client.GetAsync("/exception", _cancellationToken);

          // Assert
          response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
          var content = await response.Content.ReadAsStringAsync(_cancellationToken);
          response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
          var errors = JsonSerializer.Deserialize<List<ThrownValidationError>>(content, _jsonSerializerOptions);
          errors.Should().NotBeNull();
          errors[0].Origin.Should().Be("code");
          errors[0].Message.Should().Be("code non unique");
          errors[1].Origin.Should().Be("name");
          errors[1].Message.Should().Be("nom non unique");
     }
}
