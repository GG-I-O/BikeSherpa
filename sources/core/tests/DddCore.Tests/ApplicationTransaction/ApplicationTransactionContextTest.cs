using AwesomeAssertions;
using Ggio.DddCore;
using Ggio.DddCore.Infrastructure.ApplicationTransaction;
using JetBrains.Annotations;

namespace DddCore.Tests.ApplicationTransaction;

[TestSubject(typeof(ApplicationTransactionContext))]
public class ApplicationTransactionContextTest
{
     [Fact]
     public void Constructor_ShouldInitializeStatusToActive()
     {
          // Arrange
          var sut = MakeSut();

          // Act & Assert
          sut.Status.Should().Be(IApplicationTransactionContext.TransactionScopeStatus.Active);
          sut.EntitiesWithEvents.Should().BeEmpty();
     }

     [Fact]
     public void Complete_ShouldSetStatusToCompleted()
     {
          // Arrange
          var sut = MakeSut();

          // Act
          sut.Complete();

          // Assert
          sut.Status.Should().Be(IApplicationTransactionContext.TransactionScopeStatus.Completed);
     }

     private ApplicationTransactionContext MakeSut() =>
          // This class has no dependencies, so no mocks are needed.
          new();
}
