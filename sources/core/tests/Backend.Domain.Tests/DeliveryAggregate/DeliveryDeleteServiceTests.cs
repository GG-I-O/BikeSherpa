using AwesomeAssertions;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;
using Ggio.DddCore;
using Mediator;
using Moq;

namespace Backend.Domain.Tests.DeliveryAggregate;

public class DeliveryDeleteServiceTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly DeliveryDeleteService _sut;
    private readonly Delivery _delivery;
    private DomainEntityDeletedEvent? _capturedEvent;
    private CancellationToken _capturedToken;

    public DeliveryDeleteServiceTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _mediatorMock
            .Setup(m => m.Publish(It.IsAny<DomainEntityDeletedEvent>(), It.IsAny<CancellationToken>()))
            .Callback((DomainEntityDeletedEvent evt, CancellationToken ct) =>
            {
                _capturedEvent = evt;
                _capturedToken = ct;
            })
            .Returns(ValueTask.CompletedTask);

        _delivery = new(_mediatorMock.Object)
        {
            PricingStrategy = PricingStrategy.SimpleDeliveryStrategy,
            Code = "TEST-001",
            CustomerId = Guid.NewGuid(),
            Urgency = "Normal",
            PackingSize = "Standard",
            InsulatedBox = false,
            ContractDate = DateTimeOffset.UtcNow,
            StartDate = DateTimeOffset.UtcNow,
            Steps = []
        };
        _sut = new DeliveryDeleteService(_mediatorMock.Object);
    }

    [Fact]
    public async Task DeleteDeliveryAsync_PublishesDomainEntityDeletedEvent()
    {
        await _sut.DeleteDeliveryAsync(_delivery, TestContext.Current.CancellationToken);

        _mediatorMock.Verify(
            m => m.Publish(It.IsAny<DomainEntityDeletedEvent>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task DeleteDeliveryAsync_PublishesEventContainingTheCorrectDelivery()
    {
        await _sut.DeleteDeliveryAsync(_delivery, TestContext.Current.CancellationToken);

        _capturedEvent.Should().NotBeNull();
        _capturedEvent!.NewEntity.Should().BeSameAs(_delivery);
    }

    [Fact]
    public async Task DeleteDeliveryAsync_ForwardsCancellationToken()
    {
        using var cts = new CancellationTokenSource();

        await _sut.DeleteDeliveryAsync(_delivery, cts.Token);

        _capturedToken.Should().Be(cts.Token);
    }
}

