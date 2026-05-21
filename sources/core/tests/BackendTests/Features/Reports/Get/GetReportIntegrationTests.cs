using System.Net;
using System.Text.Json;
using AutoFixture;
using AwesomeAssertions;
using BackendTests.Services;
using Ggio.BikeSherpa.Backend.Domain.CustomerAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.SharedKernel;
using Ggio.BikeSherpa.Backend.Features.Reports.Get;
using Ggio.BikeSherpa.Backend.Infrastructure;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PricingStrategyEnum = Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations.PricingStrategy;

namespace BackendTests.Features.Reports.Get;

[Collection("Database integration tests")]
[TestSubject(typeof(GetReportEndpoint))]
[Trait("Category", "Integration")]
public class GetReportIntegrationTests : IClassFixture<IntegrationTestWebApplicationFactory>
{
     private readonly WebApplicationFactory<Program> _factory;
     private readonly Fixture _fixture = TestFixtureFactory.Create();

     private const string Scope = "read:reports";
     private const string UserEmail = "user@example.com";

     private readonly JsonSerializerOptions _jsonSerializerOptions = new()
     {
          PropertyNameCaseInsensitive = false,
          PropertyNamingPolicy = JsonNamingPolicy.CamelCase
     };

     public GetReportIntegrationTests(IntegrationTestWebApplicationFactory factory)
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
     public async Task ShouldReturnReport_ForCustomerAndDateRange()
     {
          // Arrange
          await using var scope = _factory.Services.CreateAsyncScope();
          var dbContext = scope.ServiceProvider.GetRequiredService<BackendDbContext>();
          await ResetDatabaseAsync(dbContext);

          var client = _factory.CreateClient();

          var startDate = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);
          var endDate = new DateTimeOffset(2026, 1, 31, 23, 59, 59, TimeSpan.Zero);

          var customer = _fixture.Build<Customer>()
               .With(c => c.Name, "Report Customer")
               .With(c => c.Code, "C01")
               .With(c => c.Address, _fixture.Build<Address>()
                    .With(a => a.Postcode, "38000")
                    .With(a => a.City, "Grenoble")
                    .With(a => a.Complement, (string?)null)
                    .Create())
               .Create();

          var delivery = _fixture.Build<Delivery>()
               .With(d => d.CustomerId, customer.Id)
               .With(d => d.Steps, [])
               .With(d => d.PricingStrategy, PricingStrategyEnum.CustomStrategy)
               .With(d => d.TotalPrice, 42.50)
               .With(d => d.ContractDate, startDate)
               .With(d => d.StartDate, startDate.AddDays(1))
               .With(d => d.CreatedAt, DateTime.UtcNow)
               .With(d => d.UpdatedAt, DateTime.UtcNow)
               .Create();

          await dbContext.Customers.AddAsync(customer, CancellationToken.None);
          await dbContext.Deliveries.AddAsync(delivery, CancellationToken.None);
          await dbContext.SaveChangesAsync(CancellationToken.None);

          try
          {
               // Act
               var response = await client.GetAsync(
                    $"/api/reports?customerId={customer.Id}&startDate={Uri.EscapeDataString(startDate.ToString("O"))}&endDate={Uri.EscapeDataString(endDate.ToString("O"))}",
                    CancellationToken.None);

               // Assert
               response.StatusCode.Should().Be(HttpStatusCode.OK);

               var responseBody = await response.Content.ReadAsStringAsync(CancellationToken.None);
               var report = JsonSerializer.Deserialize<JsonElement>(responseBody, _jsonSerializerOptions);

               report.GetProperty("customerName").GetString().Should().Be(customer.Name);
               report.GetProperty("startDate").GetDateTimeOffset().Should().Be(startDate);
               report.GetProperty("endDate").GetDateTimeOffset().Should().Be(endDate);
               report.GetProperty("totalPrice").GetDouble().Should().Be(42.50);

               var deliveries = report.GetProperty("deliveries");
               deliveries.GetArrayLength().Should().Be(1);
               deliveries[0].GetProperty("deliveryCode").GetString().Should().Be(delivery.Code);
               deliveries[0].GetProperty("deliveryPrice").GetDouble().Should().Be(42.50);
          }
          finally
          {
               // Clean
               await ResetDatabaseAsync(dbContext);
          }
     }
}
