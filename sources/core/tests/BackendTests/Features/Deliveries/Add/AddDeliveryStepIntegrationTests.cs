using System.Net;
using System.Net.Http.Json;
using AutoFixture;
using AwesomeAssertions;
using BackendTests.Services;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.SharedKernel;
using Ggio.BikeSherpa.Backend.Features.Deliveries.Add;
using Ggio.BikeSherpa.Backend.Infrastructure;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BackendTests.Features.Deliveries.Add;

[Collection("Database integration tests")]
[TestSubject(typeof(AddDeliveryStepEndpoint))]
[Trait("Category", "Integration")]
public class AddDeliveryStepIntegrationTests : IClassFixture<IntegrationTestWebApplicationFactory>
{
     private readonly WebApplicationFactory<Program> _factory;
     private readonly Fixture _fixture = new();

     private const string Scope = "write:deliveries";

     public AddDeliveryStepIntegrationTests(IntegrationTestWebApplicationFactory factory)
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
     
     private async static Task ResetDatabaseAsync(BackendDbContext dbContext)
     {
          await dbContext.Database.EnsureDeletedAsync();
          await dbContext.Database.MigrateAsync();
     }

     [Fact]
     public async Task ShouldAddDeliveryStep()
     {
          // Arrange
          await using var scope = _factory.Services.CreateAsyncScope();
          var dbContext = scope.ServiceProvider.GetRequiredService<BackendDbContext>();
          await ResetDatabaseAsync(dbContext);
          
          var client = _factory.CreateClient();

          var delivery = _fixture.Build<Delivery>()
               .With(d => d.Steps, [])
               .With(d => d.ContractDate, DateTime.UtcNow)
               .With(d => d.StartDate, DateTime.UtcNow)
               .Create();

          await dbContext.Deliveries.AddAsync(delivery, CancellationToken.None);
          await dbContext.SaveChangesAsync(CancellationToken.None);

          var address = _fixture
               .Build<Address>()
               .With(a => a.Postcode, "38000")
               .With(a => a.City, "Grenoble")
               .Create();

          var step = _fixture
               .Build<DeliveryStep>()
               .Without(s => s.ParentDelivery)
               .With(s => s.EstimatedDeliveryDate, DateTime.UtcNow)
               .With(s => s.StepAddress, address)
               .Create();

          try
          {
               // Act
               var response = await client.PostAsJsonAsync($"/api/delivery/{delivery.Id}/step/", step, CancellationToken.None);

               // Assert
               response.StatusCode.Should().Be(HttpStatusCode.Created);

               var dbDelivery = dbContext.Deliveries
                    .Include(d => d.Steps)
                    .FirstOrDefault(d => d.Id == delivery.Id);

               dbDelivery.Should().NotBeNull();
               dbDelivery.Steps.Should().HaveCount(1);
               dbDelivery.Steps.First().StepAddress.Should().BeEquivalentTo(address);
          }
          finally
          {
               // Clean
               await ResetDatabaseAsync(dbContext);
          }
     }
}
