using AwesomeAssertions;
using Ggio.DddCore.Infrastructure;
using Mediator;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace DddCore.Tests.Integration;

public class AddDomainEntityTests(MediatorTestFixture fixture) : IClassFixture<MediatorTestFixture>
{
     [Fact]
     public async Task GivenAggregateRoot_WhenAddDomainEntity_ThenDomainEventIsPublishedAndEntityAddedToDbContext()
     {
          // Arrange
          var serviceCollection = fixture.GetServiceCollection();
          serviceCollection.AddDddDbContext<TestDbContext>((_, option) => option.UseInMemoryDatabase(Guid.NewGuid().ToString()));
          serviceCollection.AddDddInfrastructureServices();
          serviceCollection.AddLogging();
         
          var serviceScope = serviceCollection.BuildServiceProvider().CreateScope();
          var dbContext = serviceScope.ServiceProvider.GetRequiredService<TestDbContext>();
          
          var factory = new MyAggregateRootFactory(serviceScope.ServiceProvider.GetRequiredService<IMediator>());
          
          // Act
          var entity = await factory.CreateAsync("test");
          var dbEntity = await dbContext.MyAggregateRoots.FindAsync([entity.Name],cancellationToken: TestContext.Current.CancellationToken);
          
          // Assert
          dbEntity.Should().NotBeNull();
          dbEntity.Name.Should().Be(entity.Name);

     }
}
