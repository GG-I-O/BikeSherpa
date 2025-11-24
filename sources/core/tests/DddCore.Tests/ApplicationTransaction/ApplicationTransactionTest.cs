using AwesomeAssertions;
using Ggio.DddCore;
using Ggio.DddCore.Infrastructure.ApplicationTransaction;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace DddCore.Tests.ApplicationTransaction;

[TestSubject(typeof(ApplicationTransaction<>))]
public class ApplicationTransactionTest
{
     [Fact]
     public async Task CommitAsync_ShouldThrowInvalidOperationException_WhenTransactionIsNotActive()
     {
          // Arrange
          var sut = MakeSut(out var contextMock, out _, out _);
          contextMock.Setup(x => x.Status).Returns(IApplicationTransactionContext.TransactionScopeStatus.Completed);

          // Act
          var act = async () => await sut.CommitAsync(CancellationToken.None);

          // Assert
          await act.Should().ThrowAsync<InvalidOperationException>()
               .WithMessage("Cannot dispatch domain events when current application transaction is not active.");
     }

     [Fact]
     public async Task CommitAsync_ShouldProcessTransactionCorrectly_WhenTransactionIsActive()
     {
          // Arrange
          var sut = MakeSut(out var contextMock, out var dispatcherMock, out var dbContext);

          contextMock.Setup(x => x.Status).Returns(IApplicationTransactionContext.TransactionScopeStatus.Active);

          var entity = new TestEntity();
          entity.AddEvent();
          dbContext.TestEntities.Add(entity);

          // Act
          await sut.CommitAsync(CancellationToken.None);

          // Assert
          // 1. Dispatch pre-save events
          dispatcherMock.Verify(x => x.DispatchEventsAsync(
               It.Is<IEnumerable<HasDomainEventsBase>>(e => e.Contains(entity))), Times.Once);

          // 2. SaveChanges called (Implicitly verified if entity is saved, but difficult to verify call on real DbContext without side effects check)
          // We can check if ID is generated if it was 0, or state is unchanged/detached if we were clearing it.
          // Or purely rely on the flow not crashing.
          // Since we use a real DbContext, checking state is best.
          dbContext.ChangeTracker.HasChanges().Should().BeFalse("SaveChanges should have been called");

          // 3. Complete context
          contextMock.Verify(x => x.Complete(), Times.Once);

          // 4. Dispatch post-save events
          dispatcherMock.Verify(x => x.DispatchAndClearEventsAsync(
               It.Is<IEnumerable<HasDomainEventsBase>>(e => e.Contains(entity))), Times.Once);
     }

     private static ApplicationTransaction<TestDbContext> MakeSut(
          out Mock<IApplicationTransactionContext> applicationTransactionContextMock,
          out Mock<IDomainEventDispatcher> domainEventDispatcherMock,
          out TestDbContext dbContext)
     {
          applicationTransactionContextMock = new Mock<IApplicationTransactionContext>();
          domainEventDispatcherMock = new Mock<IDomainEventDispatcher>();

          var options = new DbContextOptionsBuilder<TestDbContext>()
               .UseInMemoryDatabase(Guid.NewGuid().ToString())
               .Options;

          dbContext = new TestDbContext(options);

          return new ApplicationTransaction<TestDbContext>(
               dbContext,
               applicationTransactionContextMock.Object,
               domainEventDispatcherMock.Object
          );
     }

     public class TestDbContext(DbContextOptions<TestDbContext> options) : DbContext(options)
     {
          public DbSet<TestEntity> TestEntities { get; set; }
     }

     public class TestEntity : EntityBase
     {
          public void AddEvent()
          {
               RegisterDomainEvent(new TestDomainEvent());
          }
     }

#pragma warning disable MSG0005
     public record TestDomainEvent : DomainEventBase;
#pragma warning restore MSG0005
}
