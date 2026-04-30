using System.Net;
using AutoFixture;
using AwesomeAssertions;
using BackendTests.Services;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.SharedKernel;
using Ggio.BikeSherpa.Backend.Features.Deliveries.Get;
using Ggio.BikeSherpa.Backend.Infrastructure;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BackendTests.Features.Deliveries.Get;

[Collection("Database integration tests")]
[TestSubject(typeof(GetDeliveryEndpoint))]
[Trait("Category", "Integration")]
public class GetDeliveryIntegrationTests : IClassFixture<IntegrationTestWebApplicationFactory>
{
     private readonly WebApplicationFactory<Program> _factory;
     private readonly Fixture _fixture = new();

     private const string Scope = "read:deliveries";

     public GetDeliveryIntegrationTests(IntegrationTestWebApplicationFactory factory)
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
               }
          );
     }
     
     private async static Task ResetDatabaseAsync(BackendDbContext dbContext)
     {
          await dbContext.Database.EnsureDeletedAsync();
          await dbContext.Database.MigrateAsync();
     }

     [Fact]
     public async Task ShouldReturnDelivery_WithSteps()
     {
          // Arrange
          await using var scope = _factory.Services.CreateAsyncScope();
          var dbContext = scope.ServiceProvider.GetRequiredService<BackendDbContext>();
          await ResetDatabaseAsync(dbContext);
          
          var client = _factory.CreateClient();
          
          var address = _fixture
               .Build<Address>()
               .With(a => a.StreetInfo, "2 Cours Berriat")
               .With(a => a.Postcode, "38000")
               .With(a => a.City, "Grenoble")
               .Create();

          var delivery = _fixture.Build<Delivery>()
               .With(d => d.Steps, [])
               .With(d => d.ContractDate, DateTime.UtcNow)
               .With(d => d.StartDate, DateTime.UtcNow)
               .With(d => d.CreatedAt, DateTime.UtcNow)
               .With(d => d.UpdatedAt, DateTime.UtcNow)
               .Create();
          
          var step = _fixture
               .Build<DeliveryStep>()
               .With(s => s.ParentDelivery, delivery)
               .With(s => s.StepAddress, address)
               .With(s => s.EstimatedDeliveryDate, DateTime.UtcNow)
               .With(s => s.RealDeliveryDate, DateTime.UtcNow)
               .With(s => s.CreatedAt, DateTime.UtcNow)
               .With(s => s.UpdatedAt, DateTime.UtcNow)
               .Create();
          delivery.Steps.Add(step);

          await dbContext.Deliveries.AddAsync(delivery, CancellationToken.None);
          await dbContext.SaveChangesAsync(CancellationToken.None);

          try
          {
               // Act
               var response = await client.GetAsync($"/api/deliveries", CancellationToken.None);

               // Assert
               response.StatusCode.Should().Be(HttpStatusCode.OK);

               var dbDelivery = dbContext.Deliveries
                    .FirstOrDefault(d => d.Id == delivery.Id);

               dbDelivery.Should().NotBeNull();
               dbDelivery.Steps.Should().HaveCount(1);
               dbDelivery.Steps.First().StepAddress.Should().BeEquivalentTo(address);
               dbDelivery.Steps.First().StepZone.Should().NotBeNull();
          }
          finally
          {
               // Clean
               await ResetDatabaseAsync(dbContext);
          }
     }
}
