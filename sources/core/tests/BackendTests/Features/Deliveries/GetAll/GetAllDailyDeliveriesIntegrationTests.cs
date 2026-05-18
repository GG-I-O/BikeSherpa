using System.Net;
using System.Text.Json;
using AutoFixture;
using AwesomeAssertions;
using BackendTests.Services;
using Ggio.BikeSherpa.Backend.Domain.CourierAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.SharedKernel;
using Ggio.BikeSherpa.Backend.Features.Deliveries.GetAll;
using Ggio.BikeSherpa.Backend.Infrastructure;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace BackendTests.Features.Deliveries.GetAll;

[Collection("Database integration tests")]
[TestSubject(typeof(GetAllDailyDeliveriesEndpoint))]
[Trait("Category", "Integration")]
public class GetAllDailyDeliveriesIntegrationTests : IClassFixture<IntegrationTestWebApplicationFactory>
{
     private readonly WebApplicationFactory<Program> _factory;
     private readonly Fixture _fixture = new();

     private const string Scope = "read:myDeliveries";
     private const string UserEmail = "courier@example.com";

     public GetAllDailyDeliveriesIntegrationTests(IntegrationTestWebApplicationFactory factory)
     {
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
     public async Task ShouldReturnDailySteps_ForAuthenticatedCourierAndDate()
     {
          // Arrange
          await using var scope = _factory.Services.CreateAsyncScope();
          var dbContext = scope.ServiceProvider.GetRequiredService<BackendDbContext>();
          await ResetDatabaseAsync(dbContext);

          var client = _factory.CreateClient();
          var date = new DateTimeOffset(2026, 5, 12, 0, 0, 0, TimeSpan.Zero);
          var now = DateTimeOffset.UtcNow;

          var courierAddress = _fixture
               .Build<Address>()
               .With(a => a.StreetInfo, "1 Rue de la Paix")
               .With(a => a.Postcode, "38000")
               .With(a => a.City, "Grenoble")
               .Create();

          var courier = _fixture.Build<Courier>()
               .With(c => c.Email, UserEmail)
               .With(c => c.Address, courierAddress)
               .With(c => c.CreatedAt, now)
               .With(c => c.UpdatedAt, now)
               .Create();

          var delivery = _fixture.Build<Delivery>()
               .With(d => d.Steps, [])
               .With(d => d.ContractDate, date)
               .With(d => d.StartDate, date)
               .With(d => d.CreatedAt, now)
               .With(d => d.UpdatedAt, now)
               .Create();

          var address1 = _fixture
               .Build<Address>()
               .With(a => a.StreetInfo, "1 Cours Berriat")
               .With(a => a.Postcode, "38000")
               .With(a => a.City, "Grenoble")
               .Create();

          var expectedStep = _fixture.Build<DeliveryStep>()
               .With(s => s.ParentDelivery, delivery)
               .With(s => s.StepAddress, address1)
               .With(s => s.CourierId, courier.Id)
               .With(s => s.EstimatedDeliveryDate, date.AddHours(10))
               .With(s => s.RealDeliveryDate, date.AddHours(11))
               .With(s => s.CreatedAt, now)
               .With(s => s.UpdatedAt, now)
               .Create();
          
          var address2 = _fixture
               .Build<Address>()
               .With(a => a.StreetInfo, "2 Cours Berriat")
               .With(a => a.Postcode, "38000")
               .With(a => a.City, "Grenoble")
               .Create();

          var stepForAnotherDate = _fixture.Build<DeliveryStep>()
               .With(s => s.ParentDelivery, delivery)
               .With(s => s.StepAddress, address2)
               .With(s => s.CourierId, courier.Id)
               .With(s => s.EstimatedDeliveryDate, date.AddDays(1))
               .With(s => s.RealDeliveryDate, (DateTimeOffset?)null)
               .With(s => s.CreatedAt, now)
               .With(s => s.UpdatedAt, now)
               .Create();

          delivery.Steps.Add(expectedStep);
          delivery.Steps.Add(stepForAnotherDate);

          await dbContext.Couriers.AddAsync(courier, CancellationToken.None);
          await dbContext.Deliveries.AddAsync(delivery, CancellationToken.None);
          await dbContext.SaveChangesAsync(CancellationToken.None);

          try
          {
               // Act
               var response = await client.GetAsync($"/api/deliveries/dailyDeliveries/{date:O}", CancellationToken.None);

               // Assert
               response.StatusCode.Should().Be(HttpStatusCode.OK);

               var responseBody = await response.Content.ReadAsStringAsync(CancellationToken.None);
               var responseArray = JsonSerializer.Deserialize<JsonElement>(
                    responseBody,
                    new JsonSerializerOptions
                    {
                         PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    });

               responseArray.GetArrayLength().Should().Be(1);

               var deliveryObject = responseArray[0].GetProperty("data");
               deliveryObject.GetProperty("id").GetGuid().Should().Be(delivery.Id);

               var steps = deliveryObject.GetProperty("steps");
               steps.GetArrayLength().Should().Be(1);

               var stepObject = steps[0].GetProperty("data");
               stepObject.GetProperty("id").GetGuid().Should().Be(expectedStep.Id);
               stepObject.GetProperty("courierId").GetGuid().Should().Be(courier.Id);
          }
          finally
          {
               // Clean
               await ResetDatabaseAsync(dbContext);
          }
     }

     [Fact]
     public async Task ShouldReturnUnauthorized_WhenCourierDoesNotExist()
     {
          // Arrange
          await using var scope = _factory.Services.CreateAsyncScope();
          var dbContext = scope.ServiceProvider.GetRequiredService<BackendDbContext>();
          await ResetDatabaseAsync(dbContext);

          var client = _factory.CreateClient();
          var date = new DateTimeOffset(2026, 5, 12, 0, 0, 0, TimeSpan.Zero);

          try
          {
               // Act
               var act = async () => await client.GetAsync($"/api/deliveries/dailyDeliveries/{date:O}", CancellationToken.None);

               // Assert
               await act.Should().ThrowAsync<UnauthorizedAccessException>()
                    .WithMessage("User unauthorized");
          }
          finally
          {
               // Clean
               await ResetDatabaseAsync(dbContext);
          }
     }
}