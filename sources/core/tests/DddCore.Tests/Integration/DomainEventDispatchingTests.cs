using AwesomeAssertions;
using Ggio.DddCore;
using Ggio.DddCore.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DddCore.Tests.Integration;

public class DomainEventDispatchingTests
{
     [Fact]
     public async Task GivenAggregateRootWithEventsAndTransactionIsComplete_WhenApplicationTransactionIsCommites_ThenTransactionalDomainEventHandlersAreCalled()
     {
          // Arrange
          var serviceCollection = new ServiceCollection();
          serviceCollection.AddDddDbContext<TestDbContext>((_, option) => option.UseInMemoryDatabase("test"));
          serviceCollection.AddInfrastructureServices();
          serviceCollection.AddMediator(options =>
          {
               options.Assemblies = [typeof(MyAggregateRoot).Assembly, typeof(IDomainEvent)];
               options.ServiceLifetime = ServiceLifetime.Scoped;
          });

          serviceCollection.AddLogging();


          var serviceScope = serviceCollection.BuildServiceProvider().CreateScope();
          var dbContext = serviceScope.ServiceProvider.GetRequiredService<TestDbContext>();

          var applicationTransaction = serviceScope.ServiceProvider.GetRequiredService<IApplicationTransaction>();


          await dbContext.MyAggregateRoots.AddAsync(new MyAggregateRoot { Name = "test" });
          await dbContext.SaveChangesAsync();

          // Act
          var testEntity = dbContext.MyAggregateRoots.First();
          testEntity.MakeWonderful();
          await applicationTransaction.CommitAsync();

          // Assert
          var testPostTransactionHandler = serviceScope.ServiceProvider.GetRequiredService<TestPostTransactionHandlers>();
          testPostTransactionHandler.WasCalled.Should().BeTrue();
          testPostTransactionHandler.HandleCount.Should().Be(1);
          var testTransactionalDispatcher = serviceScope.ServiceProvider.GetRequiredService<TestTransactionalHandlers>();
          testTransactionalDispatcher.WasCalled.Should().BeTrue();
          testTransactionalDispatcher.HandleCount.Should().Be(1);
          var entity = dbContext.MyAggregateRoots.First();
          entity.Name.Should().Be("Wonderful");
     }
}
