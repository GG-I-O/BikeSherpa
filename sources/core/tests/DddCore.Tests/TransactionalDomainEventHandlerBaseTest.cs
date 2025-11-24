using AwesomeAssertions;
using Ggio.DddCore;
using JetBrains.Annotations;
using Moq;

namespace DddCore.Tests;

[TestSubject(typeof(TransactionalDomainEventHandlerBase<>))]
public class TransactionalDomainEventHandlerBaseTest
{
     [Fact]
     public async Task Handle_ShouldNotCallHandleInternal_WhenTransactionIsNotActive()
     {
          // Arrange
          var sut = MakeSut(out var contextMock);
          contextMock.Setup(x => x.Status).Returns(IApplicationTransactionContext.TransactionScopeStatus.Completed);

          // Act
          await sut.Handle(new TestDomainEvent(), CancellationToken.None);

          // Assert
          sut.WasCalled.Should().BeFalse();
     }

     [Fact]
     public async Task Handle_ShouldCallHandleInternal_WhenTransactionIsActive()
     {
          // Arrange
          var sut = MakeSut(out var contextMock);
          contextMock.Setup(x => x.Status).Returns(IApplicationTransactionContext.TransactionScopeStatus.Active);

          // Act
          await sut.Handle(new TestDomainEvent(), CancellationToken.None);

          // Assert
          sut.WasCalled.Should().BeTrue();
     }

     private static TestTransactionalHandler MakeSut(out Mock<IApplicationTransactionContext> contextMock)
     {
          contextMock = new Mock<IApplicationTransactionContext>();
          return new TestTransactionalHandler(contextMock.Object);
     }

     public record TestDomainEvent : DomainEventBase;

     public class TestTransactionalHandler(IApplicationTransactionContext context)
          : TransactionalDomainEventHandlerBase<TestDomainEvent>(context)
     {
          public bool WasCalled { get; private set; }

          override protected ValueTask HandleInternal(TestDomainEvent notification, CancellationToken cancellationToken)
          {
               WasCalled = true;
               return ValueTask.CompletedTask;
          }
     }
}
