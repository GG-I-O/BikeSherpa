using AwesomeAssertions;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;
using Ggio.DddCore;
using Mediator;
using Moq;

namespace Backend.Domain.Tests.DeliveryAggregate;

public class DeliveryFactoryTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly DeliveryFactory _sut;
    private DomainEntityAddedEvent? _capturedEvent;
    private readonly static Guid CustomerId = Guid.NewGuid();
    private readonly static DateTimeOffset ContractDate = new(2026, 1, 15, 10, 0, 0, TimeSpan.Zero);
    private readonly static DateTimeOffset StartDate = new(2026, 1, 14, 10, 0, 0, TimeSpan.Zero);

    public DeliveryFactoryTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _mediatorMock
            .Setup(m => m.Publish(It.IsAny<DomainEntityAddedEvent>(), It.IsAny<CancellationToken>()))
            .Callback((DomainEntityAddedEvent evt, CancellationToken _) => _capturedEvent = evt)
            .Returns(ValueTask.CompletedTask);

        _sut = new DeliveryFactory(_mediatorMock.Object);
    }

    private Task<Delivery> CreateDefault(
        PricingStrategyEnum strategy = PricingStrategyEnum.SimpleDeliveryStrategy,
        string code = "TEST-001",
        Guid? customerId = null,
        string urgency = "Normal",
        double? totalPrice = null,
        double? discount = null,
        string[]? details = null,
        string packingSize = "Standard",
        bool insulatedBox = false,
        DateTimeOffset? contractDate = null,
        DateTimeOffset? startDate = null) =>
        _sut.CreateDeliveryAsync(
            strategy, code, customerId ?? CustomerId, urgency,
            totalPrice, discount, details ?? [], packingSize,
            insulatedBox,
            contractDate ?? ContractDate, startDate ?? StartDate);

    [Fact]
    public async Task CreateDelivery_MapsAllPropertiesOntoTheReturnedDelivery()
    {
        var details = new[] { "fragile", "urgent" };
        var delivery = await CreateDefault(
            strategy: PricingStrategyEnum.TourDeliveryStrategy,
            code: "DEL-42",
            customerId: CustomerId,
            urgency: "Express",
            totalPrice: 29.99,
            discount: 5.0,
            details: details,
            packingSize: "Large",
            insulatedBox: true,
            contractDate: ContractDate,
            startDate: StartDate);

        delivery.PricingStrategy.Should().Be(PricingStrategyEnum.TourDeliveryStrategy);
        delivery.Code.Should().Be("DEL-42");
        delivery.CustomerId.Should().Be(CustomerId);
        delivery.Urgency.Should().Be("Express");
        delivery.TotalPrice.Should().Be(29.99);
        delivery.Discount.Should().Be(5.0);
        delivery.Details.Should().BeEquivalentTo(details);
        delivery.PackingSize.Should().Be("Large");
        delivery.InsulatedBox.Should().BeTrue();
        delivery.ContractDate.Should().Be(ContractDate);
        delivery.StartDate.Should().Be(StartDate);
    }

    [Fact]
    public async Task CreateDelivery_InitialisesStepsAsEmptyList()
    {
        var delivery = await CreateDefault();

        delivery.Steps.Should().BeEmpty();
    }

    [Fact]
    public async Task CreateDelivery_WhenOptionalFieldsAreNull_LeavesThemNull()
    {
        var delivery = await CreateDefault(totalPrice: null, discount: null);

        delivery.TotalPrice.Should().BeNull();
        delivery.Discount.Should().BeNull();
    }

    [Fact]
    public async Task CreateDelivery_PublishesDomainEntityAddedEvent()
    {
        await CreateDefault();

        _mediatorMock.Verify(
            m => m.Publish(It.IsAny<DomainEntityAddedEvent>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task CreateDelivery_PublishesEventContainingTheCreatedDelivery()
    {
        var delivery = await CreateDefault();

        _capturedEvent.Should().NotBeNull();
        _capturedEvent!.NewEntity.Should().BeSameAs(delivery);
    }
}

