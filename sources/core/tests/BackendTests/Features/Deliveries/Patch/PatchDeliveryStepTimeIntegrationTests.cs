using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using AutoFixture;
using AwesomeAssertions;
using BackendTests.Services;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.SharedKernel;
using Ggio.BikeSherpa.Backend.Features.Deliveries.Patch;
using Ggio.BikeSherpa.Backend.Infrastructure;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson;
using Microsoft.AspNetCore.JsonPatch.SystemTextJson.Operations;

namespace BackendTests.Features.Deliveries.Patch;

[TestSubject(typeof(PatchDeliveryStepTimeEndpoint))]
[Trait("Category", "Integration")]
public class PatchDeliveryStepTimeIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
     private readonly WebApplicationFactory<Program> _factory;
     private readonly Fixture _fixture = new();
     private readonly JsonSerializerOptions _jsonSerializerOptions = new()
     {
          PropertyNameCaseInsensitive = false,
          PropertyNamingPolicy = JsonNamingPolicy.CamelCase
     };

     private const string Scope = "write:deliveries";

     public PatchDeliveryStepTimeIntegrationTests(WebApplicationFactory<Program> factory)
     {
          _factory = factory.WithWebHostBuilder(builder =>
          {
               builder.UseEnvironment("Development");

               builder.ConfigureServices(services =>
               {
                    services
                         .AddAuthentication("Test")
                         .AddScheme<TestAuthSchemeOptions, TestAuthHandler>("Test", options => options.Scope = Scope);

                    services.AddAuthorizationBuilder()
                         .AddPolicy(Scope, policy => policy.RequireClaim("scope", Scope));
               });
          });
     }

     [Fact]
     public async Task ShouldPatchDeliveryStepTime()
     {
          // Arrange
          await using var scope = _factory.Services.CreateAsyncScope();
          var dbContext = scope.ServiceProvider.GetRequiredService<BackendDbContext>();
          var client = _factory.CreateClient();

          var utcNow = DateTime.UtcNow;

          var delivery = _fixture.Build<Delivery>()
               .With(d => d.Steps, [])
               .With(d => d.ContractDate, utcNow)
               .With(d => d.StartDate, utcNow)
               .With(d => d.CreatedAt, utcNow)
               .With(d => d.UpdatedAt, utcNow)
               .Create();

          var address = _fixture
               .Build<Address>()
               .With(a => a.Postcode, "38000")
               .With(a => a.City, "Grenoble")
               .Create();

          var step = _fixture
               .Build<DeliveryStep>()
               .With(s => s.EstimatedDeliveryDate, utcNow.AddHours(1))
               .With(s => s.RealDeliveryDate, (DateTimeOffset?)null)
               .With(s => s.CreatedAt, utcNow)
               .With(s => s.UpdatedAt, utcNow)
               .With(s => s.StepAddress, address)
               .Create();

          delivery.Steps.Add(step);

          await dbContext.Deliveries.AddAsync(delivery, CancellationToken.None);
          await dbContext.SaveChangesAsync(CancellationToken.None);

          var newEstimatedDate = utcNow.AddHours(3);

          var patchDocument = new JsonPatchDocument<DeliveryStep>();
          patchDocument.Operations.Add(new Operation<DeliveryStep>()
          {
               op = "replace",
               path = "/estimatedDeliveryDate",
               value = newEstimatedDate.ToString("O")
          });

          var json = JsonSerializer.Serialize(patchDocument, _jsonSerializerOptions);
          using var content = new StringContent(json, Encoding.UTF8, "application/json-patch+json");

          try
          {
               // Act
               var response = await client.PatchAsync($"/api/delivery/{delivery.Id}/step/{step.Id}/time", content, CancellationToken.None);

               // Assert
               response.StatusCode.Should().Be(HttpStatusCode.OK);

               await using var verificationScope = _factory.Services.CreateAsyncScope();
               var verificationDbContext = verificationScope.ServiceProvider.GetRequiredService<BackendDbContext>();

               var dbDelivery = verificationDbContext.Deliveries
                    .AsNoTracking()
                    .Include(d => d.Steps)
                    .FirstOrDefault(d => d.Id == delivery.Id);

               dbDelivery.Should().NotBeNull();
               dbDelivery.Steps.Should().HaveCount(1);
               dbDelivery.Steps.First().EstimatedDeliveryDate.Should().BeCloseTo(newEstimatedDate, TimeSpan.FromSeconds(1));
          }
          finally
          {
               dbContext.Deliveries.Remove(delivery);
               await dbContext.SaveChangesAsync(CancellationToken.None);
          }
     }
}