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

[TestSubject(typeof(UpdateDeliveryStepTimeEndpoint))]
[TestSubject(typeof(UpdateDeliveryStepTimeHandler))]
[Trait("Category", "Integration")]
public class UpdateDeliveryStepTimeIntegrationTests : IClassFixture<WebApplicationFactory<Program>>
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

     public UpdateDeliveryStepTimeIntegrationTests(WebApplicationFactory<Program> factory)
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

          _delivery = _fixture.Build<Delivery>()
               .With(d => d.Steps, [])
               .With(d => d.ContractDate, DateTime.UtcNow)
               .With(d => d.StartDate, DateTime.UtcNow)
               .With(s => s.CreatedAt, DateTime.UtcNow)
               .With(s => s.UpdatedAt, DateTime.UtcNow)
               .Create();
          
          var firstStep = _fixture
               .Build<DeliveryStep>()
               .With(s => s.ParentDelivery, _delivery)
               .With(s => s.Order, 1)
               .With(s => s.CourierId, courierId)
               .With(s => s.EstimatedDeliveryDate, DateTimeOffset.UtcNow.AddHours(1))
               .With(s => s.RealDeliveryDate, (DateTimeOffset?)null)
               .With(s => s.CreatedAt, DateTime.UtcNow)
               .With(s => s.UpdatedAt, DateTime.UtcNow)
               .With(s => s.StepAddress, firstAddress)
               .Create();
          _delivery.Steps.Add(firstStep);

          var secondStep = _fixture
               .Build<DeliveryStep>()
               .With(s => s.ParentDelivery, _delivery)
               .With(s => s.Order, 2)
               .With(s => s.CourierId, courierId)
               .With(s => s.EstimatedDeliveryDate, DateTimeOffset.UtcNow.AddHours(2))
               .With(s => s.RealDeliveryDate, (DateTimeOffset?)null)
               .With(s => s.CreatedAt, DateTime.UtcNow)
               .With(s => s.UpdatedAt, DateTime.UtcNow)
               .With(s => s.StepAddress, secondAddress)
               .Create();
          _delivery.Steps.Add(secondStep);
          
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
     public async Task ShouldUpdateDeliveryStepTime()
     {
          // Arrange
          var dbContext = _factory.Services.CreateAsyncScope().ServiceProvider.GetService<BackendDbContext>();
          var client = _factory.CreateClient();

          var firstStep = _delivery.Steps[0];
          var secondStep = _delivery.Steps[1];
          var newEstimatedDeliveryDate = firstStep.EstimatedDeliveryDate.AddMinutes(30);

          await dbContext!.Deliveries.AddAsync(_delivery, CancellationToken.None);
          await dbContext.SaveChangesAsync(CancellationToken.None);

          var request = new UpdateDeliveryStepTimeRequest(newEstimatedDeliveryDate);
          var json = JsonSerializer.Serialize(request, _jsonSerializerOptions);
          using var content = new StringContent(json, Encoding.UTF8, "application/json");

          try
          {
               // Act
               var response = await client.PutAsync(
                    $"/api/delivery/{_delivery.Id}/step/{firstStep.Id}/changeTime",
                    content,
                    CancellationToken.None);

               // Assert
               response.StatusCode.Should().Be(HttpStatusCode.OK);

               dbContext.ChangeTracker.Clear();

               var dbDelivery = await dbContext.Deliveries
                    .Include(d => d.Steps)
                    .FirstOrDefaultAsync(d => d.Id == _delivery.Id, CancellationToken.None);

               dbDelivery.Should().NotBeNull();

               var dbFirstStep = dbDelivery.Steps.Single(s => s.Id == firstStep.Id);
               var dbSecondStep = dbDelivery.Steps.Single(s => s.Id == secondStep.Id);

               dbFirstStep.EstimatedDeliveryDate.Should().BeCloseTo(newEstimatedDeliveryDate, TimeSpan.FromSeconds(1));
               dbSecondStep.EstimatedDeliveryDate.Should().BeCloseTo(secondStep.EstimatedDeliveryDate.AddMinutes(30), TimeSpan.FromSeconds(1));
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

          var request = new UpdateDeliveryStepTimeRequest(DateTimeOffset.UtcNow.AddHours(1));
          var json = JsonSerializer.Serialize(request, _jsonSerializerOptions);
          using var content = new StringContent(json, Encoding.UTF8, "application/json");

          // Act
          var response = await client.PutAsync(
               $"/api/delivery/{deliveryId}/step/{stepId}/changeTime",
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

          var request = new UpdateDeliveryStepTimeRequest(DateTimeOffset.UtcNow.AddHours(1));
          var json = JsonSerializer.Serialize(request, _jsonSerializerOptions);
          using var content = new StringContent(json, Encoding.UTF8, "application/json");

          try
          {
               // Act
               var response = await client.PutAsync(
                    $"/api/delivery/{_delivery.Id}/step/{Guid.NewGuid()}/changeTime",
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
     public async Task ShouldReturnBadRequest_WhenDateIsMissing()
     {
          // Arrange
          var dbContext = _factory.Services.CreateAsyncScope().ServiceProvider.GetService<BackendDbContext>();
          var client = _factory.CreateClient();

          var firstStep = _delivery.Steps[0];

          await dbContext!.Deliveries.AddAsync(_delivery, CancellationToken.None);
          await dbContext.SaveChangesAsync(CancellationToken.None);

          using var content = new StringContent("{}", Encoding.UTF8, "application/json");

          try
          {
               // Act
               var response = await client.PutAsync(
                    $"/api/delivery/{_delivery.Id}/step/{firstStep.Id}/changeTime",
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