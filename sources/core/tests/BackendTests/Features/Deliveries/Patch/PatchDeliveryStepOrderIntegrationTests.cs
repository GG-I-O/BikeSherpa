using System.Net;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using AutoFixture;
using AwesomeAssertions;
using BackendTests.Services;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.SPI;
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
using Moq;

namespace BackendTests.Features.Deliveries.Patch;

[TestSubject(typeof(PatchDeliveryStepOrderEndpoint))]
[Trait("Category", "Integration")]
public class PatchDeliveryStepOrderIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
     private readonly WebApplicationFactory<Program> _factory;
     private readonly Fixture _fixture = new();


     private readonly JsonSerializerOptions _jsonSerializerOptions = new()
     {
          PropertyNameCaseInsensitive = false,
          PropertyNamingPolicy = JsonNamingPolicy.CamelCase
     };

     private const string Scope = "write:deliveries";

     public PatchDeliveryStepOrderIntegrationTests(WebApplicationFactory<Program> factory)
     {
          Mock<IItinerarySpi> mockItineraryService = new();
          mockItineraryService.Setup(x => x.GetItineraryInfoAsync(
                    It.IsAny<GeoPoint>(),
                    It.IsAny<GeoPoint>(),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(new ItineraryResult(12.3, 45));

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

                    services.AddSingleton(mockItineraryService.Object);
               });
          });
     }

     [Fact]
     public async Task ShouldPatchDeliveryStepOrder()
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

          var address1 = _fixture
               .Build<Address>()
               .With(a => a.Name, "MockName")
               .With(a => a.Postcode, "38000")
               .With(a => a.City, "Grenoble")
               .Create();

          var step1 = _fixture
               .Build<DeliveryStep>()
               .With(s => s.Order, 1)
               .With(s => s.EstimatedDeliveryDate, utcNow)
               .With(s => s.RealDeliveryDate, (DateTimeOffset?)null)
               .With(s => s.CreatedAt, utcNow)
               .With(s => s.UpdatedAt, utcNow)
               .With(s => s.StepAddress, address1)
               .Create();

          var address2 = _fixture
               .Build<Address>()
               .With(a => a.Name, "MockName2")
               .With(a => a.Postcode, "38000")
               .With(a => a.City, "Grenoble")
               .Create();

          var step2 = _fixture
               .Build<DeliveryStep>()
               .With(s => s.Order, 2)
               .With(s => s.EstimatedDeliveryDate, utcNow)
               .With(s => s.RealDeliveryDate, (DateTimeOffset?)null)
               .With(s => s.CreatedAt, utcNow)
               .With(s => s.UpdatedAt, utcNow)
               .With(s => s.StepAddress, address2)
               .Create();

          delivery.Steps.Add(step1);
          delivery.Steps.Add(step2);

          await dbContext.Deliveries.AddAsync(delivery, CancellationToken.None);
          await dbContext.SaveChangesAsync(CancellationToken.None);

          const int newOrder = 2;

          var patchDocument = new JsonPatchDocument<DeliveryStep>();
          patchDocument.Operations.Add(new Operation<DeliveryStep>
          {
               op = "replace",
               path = "/order",
               value = newOrder
          });

          var json = JsonSerializer.Serialize(patchDocument, _jsonSerializerOptions);
          using var content = new StringContent(json, Encoding.UTF8, "application/json-patch+json");

          try
          {
               // Act
               var response = await client.PatchAsync($"/api/delivery/{delivery.Id}/step/{step1.Id}/order", content, CancellationToken.None);

               // Assert
               response.StatusCode.Should().Be(HttpStatusCode.OK);

               await using var verificationScope = _factory.Services.CreateAsyncScope();
               var verificationDbContext = verificationScope.ServiceProvider.GetRequiredService<BackendDbContext>();

               var dbDelivery = await verificationDbContext.Deliveries
                    .AsNoTracking()
                    .Include(d => d.Steps)
                    .FirstOrDefaultAsync(d => d.Id == delivery.Id, CancellationToken.None);

               dbDelivery.Should().NotBeNull();
               dbDelivery.Steps.Should().HaveCount(2);
               dbDelivery.Steps.First(s => s.Id == step1.Id).Order.Should().Be(newOrder);
          }
          finally
          {
               await using var cleanupScope = _factory.Services.CreateAsyncScope();
               var cleanupDbContext = cleanupScope.ServiceProvider.GetRequiredService<BackendDbContext>();

               await cleanupDbContext.Deliveries
                    .Where(d => d.Id == delivery.Id)
                    .ExecuteDeleteAsync(CancellationToken.None);
          }
     }
}
