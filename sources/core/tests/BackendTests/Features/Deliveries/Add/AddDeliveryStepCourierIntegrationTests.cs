using System.Net;
using AutoFixture;
using AwesomeAssertions;
using BackendTests.Services;
using Ggio.BikeSherpa.Backend.Domain.CourierAggregate;
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

[TestSubject(typeof(AddDeliveryStepCourierEndpoint))]
[TestSubject(typeof(AddDeliveryStepCourierHandler))]
[Trait("Category", "Integration")]
public class AddDeliveryStepCourierIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
     private readonly WebApplicationFactory<Program> _factory;
     private readonly Fixture _fixture = new();

     private readonly Delivery _delivery;
     private readonly Courier _courier;

     private const string Scope = "write:deliveries";

     public AddDeliveryStepCourierIntegrationTests(WebApplicationFactory<Program> factory)
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
               .With(s => s.StepAddress, deliveryAddress)
               .With(s => s.EstimatedDeliveryDate, DateTime.UtcNow)
               .With(s => s.RealDeliveryDate, (DateTimeOffset?)null)
               .With(s => s.CreatedAt, DateTime.UtcNow)
               .With(s => s.UpdatedAt, DateTime.UtcNow)
               .Create();
          _delivery.Steps.Add(step);

          var courierAddress = _fixture
               .Build<Address>()
               .With(a => a.Postcode, "38000")
               .With(a => a.City, "Grenoble")
               .Create();

          _courier = _fixture.Build<Courier>()
               .With(c => c.Address, courierAddress)
               .With(c => c.CreatedAt, DateTime.UtcNow)
               .With(c => c.UpdatedAt, DateTime.UtcNow)
               .Create();

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

     private async Task ClearDatabaseAsync(BackendDbContext dbContext)
     {
          await dbContext.Deliveries
               .Where(d => d.Id == _delivery.Id)
               .ExecuteDeleteAsync(CancellationToken.None);

          await dbContext.Couriers
               .Where(c => c.Id == _courier.Id)
               .ExecuteDeleteAsync(CancellationToken.None);
     }

     [Fact]
     public async Task ShouldAddCourierToDeliveryStep()
     {
          // Arrange
          var dbContext = _factory.Services.CreateAsyncScope().ServiceProvider.GetService<BackendDbContext>();
          var client = _factory.CreateClient();

          var step = _delivery.Steps[0];

          await dbContext!.Deliveries.AddAsync(_delivery, CancellationToken.None);
          await dbContext.Couriers.AddAsync(_courier, CancellationToken.None);
          await dbContext.SaveChangesAsync(CancellationToken.None);

          try
          {
               // Act
               var response = await client.PostAsync(
                    $"/api/delivery/{_delivery.Id}/step/{step.Id}/courier/{_courier.Id}",
                    null,
                    CancellationToken.None);

               // Assert
               response.StatusCode.Should().Be(HttpStatusCode.OK);

               dbContext.ChangeTracker.Clear();

               var dbDelivery = await dbContext.Deliveries
                    .Include(d => d.Steps)
                    .FirstOrDefaultAsync(d => d.Id == _delivery.Id, CancellationToken.None);

               dbDelivery.Should().NotBeNull();

               var dbStep = dbDelivery!.Steps.Single(s => s.Id == step.Id);
               dbStep.CourierId.Should().Be(_courier.Id);
          }
          finally
          {
               // Clean
               await ClearDatabaseAsync(dbContext);
          }
     }

     [Fact]
     public async Task ShouldReturnNotFoundWhenDeliveryDoesNotExist()
     {
          // Arrange
          var dbContext = _factory.Services.CreateAsyncScope().ServiceProvider.GetService<BackendDbContext>();
          var client = _factory.CreateClient();

          await dbContext!.Couriers.AddAsync(_courier, CancellationToken.None);
          await dbContext.SaveChangesAsync(CancellationToken.None);

          try
          {
               // Act
               var response = await client.PostAsync(
                    $"/api/delivery/{Guid.NewGuid()}/step/{Guid.NewGuid()}/courier/{_courier.Id}",
                    null,
                    CancellationToken.None);

               // Assert
               response.StatusCode.Should().Be(HttpStatusCode.NotFound);
          }
          finally
          {
               // Clean
               await ClearDatabaseAsync(dbContext);
          }
     }

     [Fact]
     public async Task ShouldReturnNotFoundWhenCourierDoesNotExist()
     {
          // Arrange
          var dbContext = _factory.Services.CreateAsyncScope().ServiceProvider.GetService<BackendDbContext>();
          var client = _factory.CreateClient();

          var step = _delivery.Steps[0];

          await dbContext!.Deliveries.AddAsync(_delivery, CancellationToken.None);
          await dbContext.SaveChangesAsync(CancellationToken.None);

          try
          {
               // Act
               var response = await client.PostAsync(
                    $"/api/delivery/{_delivery.Id}/step/{step.Id}/courier/{Guid.NewGuid()}",
                    null,
                    CancellationToken.None);

               // Assert
               response.StatusCode.Should().Be(HttpStatusCode.NotFound);
          }
          finally
          {
               // Clean
               await ClearDatabaseAsync(dbContext);
          }
     }
}