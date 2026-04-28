using System.Net;
using System.Text;
using System.Text.Json;
using AutoFixture;
using AwesomeAssertions;
using BackendTests.Services;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.SPI;
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

[TestSubject(typeof(UpdateDeliveryStepOrderEndpoint))]
[TestSubject(typeof(UpdateDeliveryStepOrderHandler))]
[Trait("Category", "Integration")]
public class UpdateDeliveryStepOrderIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
{
     private readonly WebApplicationFactory<Program> _factory;
     private readonly Fixture _fixture = new();
     private readonly Mock<IItinerarySpi> _mockItineraryService = new();
     private readonly Delivery _delivery;

     private const string Scope = "write:deliveries";

     private readonly JsonSerializerOptions _jsonSerializerOptions = new()
     {
          PropertyNameCaseInsensitive = false,
          PropertyNamingPolicy = JsonNamingPolicy.CamelCase
     };

     public UpdateDeliveryStepOrderIntegrationTests(WebApplicationFactory<Program> factory)
     {
          var courierId = Guid.NewGuid();
          
          var firstAddress = _fixture
               .Build<Address>()
               .With(a => a.Postcode, "38000")
               .With(a => a.City, "Grenoble")
               .Create();

          var secondAddress = _fixture
               .Build<Address>()
               .With(a => a.Postcode, "69000")
               .With(a => a.City, "Lyon")
               .Create();

          var firstStep = _fixture
               .Build<DeliveryStep>()
               .With(s => s.Order, 1)
               .With(s => s.CourierId, courierId)
               .With(s => s.EstimatedDeliveryDate, DateTime.UtcNow)
               .With(s => s.RealDeliveryDate, (DateTimeOffset?)null)
               .With(s => s.CreatedAt, DateTime.UtcNow)
               .With(s => s.UpdatedAt, DateTime.UtcNow)
               .With(s => s.StepAddress, firstAddress)
               .Create();

          var secondStep = _fixture
               .Build<DeliveryStep>()
               .With(s => s.Order, 2)
               .With(s => s.CourierId, courierId)
               .With(s => s.EstimatedDeliveryDate, DateTime.UtcNow.AddHours(1))
               .With(s => s.RealDeliveryDate, (DateTimeOffset?)null)
               .With(s => s.CreatedAt, DateTime.UtcNow)
               .With(s => s.UpdatedAt, DateTime.UtcNow)
               .With(s => s.StepAddress, secondAddress)
               .Create();

          _delivery = _fixture.Build<Delivery>()
               .With(d => d.Steps, [firstStep, secondStep])
               .With(d => d.ContractDate, DateTime.UtcNow)
               .With(d => d.StartDate, DateTime.UtcNow)
               .With(s => s.CreatedAt, DateTime.UtcNow)
               .With(s => s.UpdatedAt, DateTime.UtcNow)
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
                         .AddScheme<TestAuthSchemeOptions, TestAuthHandler>("Test", options => options.Scope = Scope);

                    services.AddAuthorizationBuilder()
                         .AddPolicy(Scope, policy => policy.RequireClaim("scope", Scope));

                    services.AddSingleton(_mockItineraryService.Object);
               });
          });
     }

     private async Task ClearDatabaseAsync(BackendDbContext dbContext)
     {
          await dbContext.Deliveries
               .Where(d => d.Id == _delivery.Id)
               .ExecuteDeleteAsync(CancellationToken.None);
     }

     [Fact]
     public async Task ShouldUpdateDeliveryStepOrder_WhenIncrementIsPositive()
     {
          // Arrange
          var dbContext = _factory.Services.CreateAsyncScope().ServiceProvider.GetService<BackendDbContext>();
          var client = _factory.CreateClient();

          var firstStep = _delivery.Steps[0];
          var secondStep = _delivery.Steps[1];

          await dbContext!.Deliveries.AddAsync(_delivery, CancellationToken.None);
          await dbContext.SaveChangesAsync(CancellationToken.None);

          var request = new UpdateDeliveryStepOrderRequest(1);
          var json = JsonSerializer.Serialize(request, _jsonSerializerOptions);
          using var content = new StringContent(json, Encoding.UTF8, "application/json");

          try
          {
               // Act
               var response = await client.PutAsync(
                    $"/api/delivery/{_delivery.Id}/step/{firstStep.Id}/changeOrder",
                    content,
                    CancellationToken.None);

               // Assert
               response.StatusCode.Should().Be(HttpStatusCode.OK);

               dbContext.ChangeTracker.Clear();

               var dbDelivery = await dbContext.Deliveries
                    .Include(d => d.Steps)
                    .FirstOrDefaultAsync(d => d.Id == _delivery.Id, CancellationToken.None);

               dbDelivery.Should().NotBeNull();

               var orderedSteps = dbDelivery.Steps
                    .OrderBy(s => s.Order)
                    .ToList();

               orderedSteps.Should().HaveCount(2);
               orderedSteps.Select(s => s.Order).Should().BeEquivalentTo([1, 2]);
               orderedSteps[0].Id.Should().Be(secondStep.Id);
               orderedSteps[1].Id.Should().Be(firstStep.Id);
          }
          finally
          {
               // Clean
               await ClearDatabaseAsync(dbContext);
          }
     }

     [Fact]
     public async Task ShouldReturnNotFound_WhenDeliveryDoesNotExist()
     {
          // Arrange
          var client = _factory.CreateClient();

          var deliveryId = Guid.NewGuid();
          var stepId = Guid.NewGuid();

          var request = new UpdateDeliveryStepOrderRequest(1);
          var json = JsonSerializer.Serialize(request, _jsonSerializerOptions);
          using var content = new StringContent(json, Encoding.UTF8, "application/json");

          // Act
          var response = await client.PutAsync(
               $"/api/delivery/{deliveryId}/step/{stepId}/changeOrder",
               content,
               CancellationToken.None);

          // Assert
          response.StatusCode.Should().Be(HttpStatusCode.NotFound);
     }

     [Fact]
     public async Task ShouldReturnNotFound_WhenStepDoesNotExist()
     {
          // Arrange
          var dbContext = _factory.Services.CreateAsyncScope().ServiceProvider.GetService<BackendDbContext>();
          var client = _factory.CreateClient();

          await dbContext!.Deliveries.AddAsync(_delivery, CancellationToken.None);
          await dbContext.SaveChangesAsync(CancellationToken.None);

          var request = new UpdateDeliveryStepOrderRequest(1);
          var json = JsonSerializer.Serialize(request, _jsonSerializerOptions);
          using var content = new StringContent(json, Encoding.UTF8, "application/json");

          try
          {
               // Act
               var response = await client.PutAsync(
                    $"/api/delivery/{_delivery.Id}/step/{Guid.NewGuid()}/changeOrder",
                    content,
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
     public async Task ShouldReturnBadRequest_WhenIncrementIsZero()
     {
          // Arrange
          var dbContext = _factory.Services.CreateAsyncScope().ServiceProvider.GetService<BackendDbContext>();
          var client = _factory.CreateClient();

          var firstStep = _delivery.Steps[0];

          await dbContext!.Deliveries.AddAsync(_delivery, CancellationToken.None);
          await dbContext.SaveChangesAsync(CancellationToken.None);

          var request = new UpdateDeliveryStepOrderRequest(0);
          var json = JsonSerializer.Serialize(request, _jsonSerializerOptions);
          using var content = new StringContent(json, Encoding.UTF8, "application/json");

          try
          {
               // Act
               var response = await client.PutAsync(
                    $"/api/delivery/{_delivery.Id}/step/{firstStep.Id}/changeOrder",
                    content,
                    CancellationToken.None);

               // Assert
               response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
          }
          finally
          {
               // Clean
               await ClearDatabaseAsync(dbContext);
          }
     }
}