using System.Net;
using AutoFixture;
using AwesomeAssertions;
using BackendTests.Services;
using Ggio.BikeSherpa.Backend.Domain.CourierAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Spi;
using Ggio.BikeSherpa.Backend.Domain.SharedKernel;
using Ggio.BikeSherpa.Backend.Features.Deliveries.Update;
using Ggio.BikeSherpa.Backend.Infrastructure;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;

namespace BackendTests.Features.Deliveries.Update;

[Collection("Database integration tests")]
[TestSubject(typeof(UpdateDeliveryStepCourierMyselfEndpoint))]
[TestSubject(typeof(UpdateDeliveryStepCourierMyselfHandler))]
[Trait("Category", "Integration")]
public class UpdateDeliveryStepCourierMyselfIntegrationTests : IClassFixture<IntegrationTestWebApplicationFactory>
{
     private const string Scope = "write:myDeliveries";
     private const string UserEmail = "user@example.com";

     private readonly Delivery _delivery;
     private readonly Courier _courier;
     private readonly WebApplicationFactory<Program> _factory;
     private readonly Fixture _fixture = TestFixtureFactory.Create();

     private readonly Mock<IItinerarySpi> _mockItineraryService = new();

     public UpdateDeliveryStepCourierMyselfIntegrationTests(IntegrationTestWebApplicationFactory factory)
     {
          var courierId = Guid.NewGuid();

          var address = _fixture
               .Build<Address>()
               .With(a => a.Postcode, "38000")
               .With(a => a.City, "Grenoble")
               .Create();

          _delivery = _fixture.Build<Delivery>()
               .With(d => d.Steps, [])
               .With(d => d.ContractDate, DateTime.UtcNow)
               .With(d => d.StartDate, DateTime.UtcNow)
               .With(s => s.CreatedAt, DateTime.UtcNow)
               .With(s => s.UpdatedAt, DateTime.UtcNow)
               .Create();

          var step = _fixture
               .Build<DeliveryStep>()
               .With(s => s.ParentDelivery, _delivery)
               .With(s => s.Order, 1)
               .With(s => s.CourierId, (Guid?)null)
               .With(s => s.EstimatedDeliveryDate, DateTime.UtcNow)
               .With(s => s.RealDeliveryDate, (DateTimeOffset?)null)
               .With(s => s.CreatedAt, DateTime.UtcNow)
               .With(s => s.UpdatedAt, DateTime.UtcNow)
               .With(s => s.StepAddress, address)
               .Create();

          _delivery.Steps.Add(step);

          _courier = _fixture
               .Build<Courier>()
               .With(c => c.Id, courierId)
               .With(c => c.Address, address)
               .With(c => c.Email, UserEmail)
               .Create();

          _mockItineraryService
               .Setup(x => x.GetItineraryInfoAsync(
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
                         .AddScheme<TestAuthSchemeOptions, TestAuthHandler>("Test", options =>
                         {
                              options.Scope = Scope;
                              options.Email = UserEmail;
                         });

                    services.AddAuthorizationBuilder()
                         .AddPolicy(Scope, policy => policy.RequireClaim("scope", Scope));

                    services.AddSingleton(_mockItineraryService.Object);
               });
          });
     }

     private async static Task ResetDatabaseAsync(BackendDbContext dbContext)
     {
          await dbContext.Database.EnsureDeletedAsync();
          await dbContext.Database.MigrateAsync();
     }

     [Fact]
     public async Task ShouldUpdateStepCourier_WhenDeliveryAndCourierExist()
     {
          // Arrange
          await using var scope = _factory.Services.CreateAsyncScope();
          var dbContext = scope.ServiceProvider.GetRequiredService<BackendDbContext>();
          await ResetDatabaseAsync(dbContext);

          var client = _factory.CreateClient();

          var step = _delivery.Steps[0];

          await dbContext.Deliveries.AddAsync(_delivery, CancellationToken.None);
          await dbContext.Couriers.AddAsync(_courier, CancellationToken.None);
          await dbContext.SaveChangesAsync(CancellationToken.None);

          try
          {
               // Act
               var response = await client.PutAsync(
                    $"/api/delivery/{_delivery.Id}/step/{step.Id}/courier/myself",
                    null,
                    CancellationToken.None);

               // Assert
               response.StatusCode.Should().Be(HttpStatusCode.OK);

               dbContext.ChangeTracker.Clear();

               var dbDelivery = await dbContext.Deliveries
                    .Include(d => d.Steps)
                    .FirstOrDefaultAsync(d => d.Id == _delivery.Id, CancellationToken.None);

               dbDelivery.Should().NotBeNull();

               var updatedStep = dbDelivery.Steps.First(s => s.Id == step.Id);
               updatedStep.CourierId.Should().Be(_courier.Id);
          }
          finally
          {
               // Clean
               await ResetDatabaseAsync(dbContext);
          }
     }

     [Fact]
     public async Task ShouldReturnNotFound_WhenDeliveryDoesNotExist()
     {
          // Arrange
          var client = _factory.CreateClient();

          var deliveryId = Guid.NewGuid();
          var stepId = Guid.NewGuid();

          // Act
          var response = await client.PutAsync(
               $"/api/delivery/{deliveryId}/step/{stepId}/courier/myself",
               null,
               CancellationToken.None);

          // Assert
          response.StatusCode.Should().Be(HttpStatusCode.NotFound);
     }

     [Fact]
     public async Task ShouldReturnNotFound_WhenCourierDoesNotExist()
     {
          // Arrange
          await using var scope = _factory.Services.CreateAsyncScope();
          var dbContext = scope.ServiceProvider.GetRequiredService<BackendDbContext>();
          await ResetDatabaseAsync(dbContext);

          var client = _factory.CreateClient();

          var step = _delivery.Steps[0];

          await dbContext.Deliveries.AddAsync(_delivery, CancellationToken.None);
          // Courier is intentionally not persisted, simulating no courier matching the current user's email.
          await dbContext.SaveChangesAsync(CancellationToken.None);

          try
          {
               // Act
               var response = await client.PutAsync(
                    $"/api/delivery/{_delivery.Id}/step/{step.Id}/courier/myself",
                    null,
                    CancellationToken.None);

               // Assert
               response.StatusCode.Should().Be(HttpStatusCode.NotFound);
          }
          finally
          {
               // Clean
               await ResetDatabaseAsync(dbContext);
          }
     }
}