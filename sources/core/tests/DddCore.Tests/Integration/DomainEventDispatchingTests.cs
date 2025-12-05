using AwesomeAssertions;
using Ggio.DddCore;
using Ggio.DddCore.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DddCore.Tests.Integration;

public class DomainEventDispatchingTests(MediatorTestFixture fixture) : IClassFixture<MediatorTestFixture>
{
     [Fact]
     public async Task GivenAggregateRootWithEventsAndTransactionIsComplete_WhenApplicationTransactionIsCommites_ThenTransactionalDomainEventHandlersAreCalled()
     {
          // Arrange
          var serviceCollection = fixture.GetServiceCollection();
          
          serviceCollection.AddDddDbContext<TestDbContext>((_, option) => option.UseInMemoryDatabase("test"));
          serviceCollection.AddInfrastructureServices();
          
          serviceCollection.AddLogging();


          var serviceScope = serviceCollection.BuildServiceProvider().CreateScope();
          var dbContext = serviceScope.ServiceProvider.GetRequiredService<TestDbContext>();

          var applicationTransaction = serviceScope.ServiceProvider.GetRequiredService<IApplicationTransaction>();


          await dbContext.MyAggregateRoots.AddAsync(new MyAggregateRoot { Name = "test" }, TestContext.Current.CancellationToken);
          await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);

          // Act
          var testEntity = await dbContext.MyAggregateRoots.FirstAsync(TestContext.Current.CancellationToken);
          testEntity.MakeWonderful();
          await applicationTransaction.CommitAsync(TestContext.Current.CancellationToken);

          // Assert
          var testPostTransactionHandler = serviceScope.ServiceProvider.GetRequiredService<TestPostTransactionHandlers>();
          testPostTransactionHandler.WasCalled.Should().BeTrue();
          testPostTransactionHandler.HandleCount.Should().Be(1);
          var testTransactionalDispatcher = serviceScope.ServiceProvider.GetRequiredService<TestTransactionalHandlers>();
          testTransactionalDispatcher.WasCalled.Should().BeTrue();
          testTransactionalDispatcher.HandleCount.Should().Be(1);
          var entity = await dbContext.MyAggregateRoots.FirstAsync(TestContext.Current.CancellationToken);
          entity.Name.Should().Be("Wonderful");
     }
}
