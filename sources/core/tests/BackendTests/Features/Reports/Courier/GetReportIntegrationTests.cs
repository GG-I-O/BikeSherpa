using AutoFixture;
using AwesomeAssertions;
using BackendTests.Services;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;
using Ggio.BikeSherpa.Backend.Domain.SharedKernel;
using Ggio.BikeSherpa.Backend.Features.Reports.Customer;
using Ggio.BikeSherpa.Backend.Infrastructure;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PricingStrategyEnum = Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations.PricingStrategy;

namespace BackendTests.Features.Reports.Courier;

[Collection("Database integration tests")]
[TestSubject(typeof(GetReportEndpoint))]
[Trait("Category", "Integration")]
public class GetReportIntegrationTests : IClassFixture<IntegrationTestWebApplicationFactory>
{
     private const string Scope = "read:reports";
     private const string UserEmail = "user@example.com";
     private readonly WebApplicationFactory<Program> _factory;
     private readonly Fixture _fixture = TestFixtureFactory.Create();

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

          var startDate = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);
          var endDate = new DateTimeOffset(2026, 1, 31, 23, 59, 59, TimeSpan.Zero);

          var urgency = new Urgency("Normal", 1, "Normal", 1.0, null, null, 12);
          var packingSize = new PackingSize("X", 1, "X", 0, 0);
          var zone = new DeliveryZone("Z1");
          await dbContext.Urgencies.AddAsync(urgency, TestContext.Current.CancellationToken);
          await dbContext.PackingSizes.AddAsync(packingSize, TestContext.Current.CancellationToken);
          await dbContext.DeliveryZones.AddAsync(zone, TestContext.Current.CancellationToken);
          await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

          var customer = _fixture.Build<Ggio.BikeSherpa.Backend.Domain.CustomerAggregate.Customer>()
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
               .With(d => d.Steps, new List<DeliveryStep>())
               .With(d => d.PricingStrategy, PricingStrategyEnum.SimpleDeliveryStrategy)
               .With(d => d.Urgency, urgency)
               .With(d => d.Code, "D01")
               .With(d => d.TotalPrice, 42.50)
               .With(d => d.ContractDate, startDate)
               .With(d => d.StartDate, startDate.AddDays(1))
               .With(d => d.CreatedAt, DateTimeOffset.UtcNow)
               .With(d => d.UpdatedAt, DateTimeOffset.UtcNow)
               .Create();

          var address = _fixture.Build<Address>()
               .With(a => a.Postcode, "38000")
               .With(a => a.City, "Grenoble")
               .With(a => a.Complement, (string?)null)
               .Create();

          var step = new DeliveryStep(StepType.Pickup, 1, address)
          {
               PackingSize = packingSize,
               StepZone = zone,
               ParentDelivery = delivery,
               StepAddress = address
          };

          delivery.Steps = [step];

          await dbContext.Customers.AddAsync(customer, CancellationToken.None);
          await dbContext.Deliveries.AddAsync(delivery, CancellationToken.None);
          await dbContext.SaveChangesAsync(CancellationToken.None);

          try
          {
               // Act
               var handler = scope.ServiceProvider.GetRequiredService<GetReportHandler>();
               var query = new GetReportQuery(customer.Id, startDate, endDate);
               var report = await handler.Handle(query, CancellationToken.None);

               // Assert
               report.Deliveries.Should().HaveCount(1);
               report.Deliveries[0].DeliveryCode.Should().Be(delivery.Code);
               report.Deliveries[0].DeliveryPrice.Should().Be(42.50);
          }
          finally
          {
               // Clean
               await ResetDatabaseAsync(dbContext);
          }
     }
}
