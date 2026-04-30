using System.Net;
using AutoFixture;
using AwesomeAssertions;
using BackendTests.Services;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.SharedKernel;
using Ggio.BikeSherpa.Backend.Features.Deliveries.Delete;
using Ggio.BikeSherpa.Backend.Infrastructure;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BackendTests.Features.Deliveries.Delete;

[Collection("Database integration tests")]
[TestSubject(typeof(DeleteDeliveryStepCourierEndpoint))]
[TestSubject(typeof(DeleteDeliveryStepCourierHandler))]
[Trait("Category", "Integration")]
public class DeleteDeliveryStepCourierIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
     private readonly WebApplicationFactory<Program> _factory;
     private readonly Fixture _fixture = new();

     private readonly Delivery _delivery;
     private readonly Guid _courierId = Guid.NewGuid();

     private const string Scope = "write:deliveries";

     public DeleteDeliveryStepCourierIntegrationTests(WebApplicationFactory<Program> factory)
     {
          var deliveryAddress = _fixture
               .Build<Address>()
               .With(a => a.Postcode, "38000")
               .With(a => a.City, "Grenoble")
               .Create();

          _delivery = _fixture.Build<Delivery>()
               .With(d => d.Steps, [])
               .With(d => d.ContractDate, DateTime.UtcNow)
               .With(d => d.StartDate, DateTime.UtcNow)
               .With(d => d.CreatedAt, DateTime.UtcNow)
               .With(d => d.UpdatedAt, DateTime.UtcNow)
               .Create();
          
          var step = _fixture
               .Build<DeliveryStep>()
               .With(s => s.ParentDelivery, _delivery)
               .With(s => s.Order, 1)
               .With(s => s.CourierId, _courierId)
               .With(s => s.StepAddress, deliveryAddress)
               .With(s => s.EstimatedDeliveryDate, DateTime.UtcNow)
               .With(s => s.RealDeliveryDate, (DateTimeOffset?)null)
               .With(s => s.CreatedAt, DateTime.UtcNow)
               .With(s => s.UpdatedAt, DateTime.UtcNow)
               .Create();
          _delivery.Steps.Add(step);

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
     public async Task ShouldDeleteCourierFromDeliveryStep()
     {
          // Arrange
          await using var scope = _factory.Services.CreateAsyncScope();
          var dbContext = scope.ServiceProvider.GetRequiredService<BackendDbContext>();
          await ResetDatabaseAsync(dbContext);
          
          var client = _factory.CreateClient();

          var step = _delivery.Steps[0];

          await dbContext.Deliveries.AddAsync(_delivery, CancellationToken.None);
          await dbContext.SaveChangesAsync(CancellationToken.None);

          try
          {
               // Act
               var response = await client.DeleteAsync(
                    $"/api/delivery/{_delivery.Id}/step/{step.Id}/courier",
                    CancellationToken.None);

               // Assert
               response.StatusCode.Should().Be(HttpStatusCode.OK);

               dbContext.ChangeTracker.Clear();

               var dbDelivery = await dbContext.Deliveries
                    .Include(d => d.Steps)
                    .FirstOrDefaultAsync(d => d.Id == _delivery.Id, CancellationToken.None);

               dbDelivery.Should().NotBeNull();

               var dbStep = dbDelivery.Steps.Single(s => s.Id == step.Id);
               dbStep.CourierId.Should().BeNull();
          }
          finally
          {
               // Clean
               await ResetDatabaseAsync(dbContext);
          }
     }

     [Fact]
     public async Task ShouldReturnNotFoundWhenDeliveryDoesNotExist()
     {
          // Arrange
          var client = _factory.CreateClient();

          // Act
          var response = await client.DeleteAsync(
               $"/api/delivery/{Guid.NewGuid()}/step/{Guid.NewGuid()}/courier",
               CancellationToken.None);

          // Assert
          response.StatusCode.Should().Be(HttpStatusCode.NotFound);
     }
}