using AwesomeAssertions;
using Ggio.DddCore;
using JetBrains.Annotations;
using Moq;

namespace DddCore.Tests;

[TestSubject(typeof(PostTransactionDomainEventHandlerBase<>))]
public class PostTransactionDomainEventHandlerBaseTest
{
     [Fact]
     public async Task Handle_ShouldNotCallHandleInternal_WhenTransactionIsNotCompleted()
     {
          // Arrange
          var sut = MakeSut(out var contextMock);
          contextMock.Setup(x => x.Status).Returns(IApplicationTransactionContext.TransactionScopeStatus.Active);

          // Act
          await sut.Handle(new TestDomainEvent(), CancellationToken.None);

          // Assert
          sut.WasCalled.Should().BeFalse();
     }

     [Fact]
     public async Task Handle_ShouldCallHandleInternal_WhenTransactionIsCompleted()
     {
          // Arrange
          var sut = MakeSut(out var contextMock);
          contextMock.Setup(x => x.Status).Returns(IApplicationTransactionContext.TransactionScopeStatus.Completed);

          // Act
          await sut.Handle(new TestDomainEvent(), CancellationToken.None);

          // Assert
          sut.WasCalled.Should().BeTrue();
     }

     private TestPostTransactionHandler MakeSut(out Mock<IApplicationTransactionContext> contextMock)
     {
          contextMock = new Mock<IApplicationTransactionContext>();
          return new TestPostTransactionHandler(contextMock.Object);
     }

     public record TestDomainEvent : DomainEventBase;

     public class TestPostTransactionHandler(IApplicationTransactionContext context)
          : PostTransactionDomainEventHandlerBase<TestDomainEvent>(context)
     {
          public bool WasCalled { get; private set; }

          override protected ValueTask HandleInternal(TestDomainEvent notification, CancellationToken cancellationToken)
          {
               WasCalled = true;
               return ValueTask.CompletedTask;
          }
     }
}
