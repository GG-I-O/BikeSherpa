using AutoFixture;
using AutoFixture.AutoMoq;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.DddCore;
using Mediator;
using Moq;

namespace Backend.Domain.Tests.DeliveryAggregate;

public class DeliveryDeleteServiceTests
{
     private readonly Delivery _delivery;
     private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());
     private readonly Mock<IMediator> _mediatorMock;
     private readonly DeliveryDeleteService _sut;

     public DeliveryDeleteServiceTests()
     {
          _mediatorMock = new Mock<IMediator>();
          _mediatorMock
               .Setup(m => m.Publish(It.IsAny<AggregateRootDeletedEvent>(), It.IsAny<CancellationToken>()))
               .Returns(ValueTask.CompletedTask);

          _delivery = _fixture.Build<Delivery>()
               .With(d => d.Steps, [])
               .Create();
          _sut = new DeliveryDeleteService(_mediatorMock.Object);
     }

     [Fact]
     public async Task DeleteDeliveryAsync_PublishesPublishesDomainEntityDeletedEventContainingTheCorrectDelivery()
     {
          // Arrange & Act
          await _sut.DeleteDeliveryAsync(_delivery, TestContext.Current.CancellationToken);

          // Assert
          _mediatorMock.Verify(
               m => m.Publish(
                    It.Is<AggregateRootDeletedEvent>(e => e.DeletedAggregate == _delivery),
                    It.IsAny<CancellationToken>()),
               Times.Once);
     }

     [Fact]
     public async Task DeleteDeliveryAsync_ForwardsCancellationToken()
     {
          // Arrange
          using var cts = new CancellationTokenSource();
          var token = cts.Token;

          // Act
          await _sut.DeleteDeliveryAsync(_delivery, token);

          // Assert
          _mediatorMock.Verify(
               m => m.Publish(It.IsAny<AggregateRootDeletedEvent>(), token),
               Times.Once);
     }
}
