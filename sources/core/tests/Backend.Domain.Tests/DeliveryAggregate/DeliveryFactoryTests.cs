using System;
using System.Threading;
using System.Threading.Tasks;
using Ardalis.Specification;
using AutoFixture;
using AwesomeAssertions;
using Ggio.BikeSherpa.Backend.Domain.CustomerAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Services.PricingStrategy;
using Ggio.BikeSherpa.Backend.Domain.SharedKernel;
using Ggio.DddCore;
using Mediator;
using Moq;
using Xunit;

namespace Backend.Domain.Tests.DeliveryAggregate;

public class DeliveryFactoryTests
{
    private readonly Mock<IMediator> _mediatorMock;
    private readonly Mock<IReadRepository<Customer>> _customerRepositoryMock = new();
    private readonly Mock<IPricingStrategyService> _pricingStrategyServiceMock = new();
    private readonly Fixture _fixture = new();
    private readonly DeliveryFactory _sut;
    private readonly static Guid CustomerId = Guid.NewGuid();
    private readonly static DateTimeOffset ContractDate = new(2026, 1, 15, 10, 0, 0, TimeSpan.Zero);
    private readonly static DateTimeOffset StartDate = new(2026, 1, 14, 10, 0, 0, TimeSpan.Zero);

    public DeliveryFactoryTests()
    {
        _mediatorMock = new Mock<IMediator>();
        _mediatorMock
            .Setup(m => m.Publish(It.IsAny<DomainEntityAddedEvent>(), It.IsAny<CancellationToken>()))
            .Returns(ValueTask.CompletedTask);

        var fakeCustomer = _fixture.Build<Customer>()
            .With(c => c.Address, _fixture.Build<Address>()
                .With(a => a.Complement, (string?)null)
                .Create())
            .Create();
        _customerRepositoryMock
            .Setup(r => r.FirstOrDefaultAsync(It.IsAny<ISpecification<Customer>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(fakeCustomer);
        _pricingStrategyServiceMock
            .Setup(s => s.CalculateDeliveryPriceWithoutVat(It.IsAny<Delivery>()))
            .Returns(55);

        _sut = new DeliveryFactory(_mediatorMock.Object, _customerRepositoryMock.Object, _pricingStrategyServiceMock.Object);
    }

    private Task<Delivery> CreateDefault(
        PricingStrategy strategy = PricingStrategy.SimpleDeliveryStrategy,
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
        // Arrange
        var details = new[] { "fragile", "urgent" };

        // Act
        var delivery = await CreateDefault(
            strategy: PricingStrategy.TourDeliveryStrategy,
            code: "DEL-42",
            customerId: CustomerId,
            urgency: "Express",
            discount: 5.0,
            details: details,
            packingSize: "Large",
            insulatedBox: true,
            contractDate: ContractDate,
            startDate: StartDate);

        // Assert
        delivery.PricingStrategy.Should().Be(PricingStrategy.TourDeliveryStrategy);
        delivery.Code.Should().Be("DEL-42");
        delivery.CustomerId.Should().Be(CustomerId);
        delivery.Urgency.Should().Be("Express");
        delivery.TotalPrice.Should().Be(55);
        delivery.Discount.Should().Be(5.0);
        delivery.Details.Should().BeEquivalentTo(details);
        delivery.PackingSize.Should().Be("Large");
        delivery.InsulatedBox.Should().BeTrue();
        delivery.ContractDate.Should().Be(ContractDate);
        delivery.StartDate.Should().Be(StartDate);
        delivery.Status.Should().Be(DeliveryStatus.Pending);
        delivery.ReportId.Should().NotBeNull();
        delivery.TotalPrice.Should().Be(55);
    }

    [Fact]
    public async Task CreateDelivery_InitialisesStepsAsEmptyList()
    {
        // Arrange & Act
        var delivery = await CreateDefault();

        // Assert
        delivery.Steps.Should().BeEmpty();
    }

    [Fact]
    public async Task CreateDelivery_WhenOptionalFieldsAreNull_LeavesThemNull()
    {
        // Arrange & Act
        var delivery = await CreateDefault(discount: null);

        // Assert
        delivery.Discount.Should().BeNull();
    }

    [Fact]
    public async Task CreateDelivery_PublishesDomainEntityAddedEventContainingTheCreatedDelivery()
    {
        // Arrange & Act
        var delivery = await CreateDefault();

        // Assert
        _mediatorMock.Verify(
            m => m.Publish(It.Is<DomainEntityAddedEvent>(e => ReferenceEquals(e.NewEntity, delivery)), It.IsAny<CancellationToken>()),
            Times.Once);
    }
}

