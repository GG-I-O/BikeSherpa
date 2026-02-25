using AutoFixture;
using AutoFixture.AutoMoq;
using AwesomeAssertions;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.PricingStrategies;
using PricingStrategyEnum = Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations.PricingStrategy;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Services.PricingStrategy;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Services.Repositories;
using Ggio.BikeSherpa.Backend.Domain.SharedKernel;
using Moq;

namespace Backend.Domain.Tests.DeliveryAggregate.Services.PricingStrategy;

public class PricingStrategyServiceTests
{
    private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());
    private static DateTimeOffset _startDate;
    private static DateTimeOffset _contractDate;
    private readonly static DeliveryZone CoreZone = new("Centre");
    private readonly static DeliveryZone BorderZone = new("Limitrophe");
    private readonly static DeliveryZone PeripheryZone = new("Périphérie");
    private readonly static DeliveryZone OutsideZone = new("Extérieur");
    private static Address? _defaultAddress;

    private readonly Mock<IPricingStrategy> _mockPricingStrategy;
    private readonly Mock<IUrgencyRepository> _mockUrgencyRepository;
    private readonly Mock<IPackingSizeRepository> _mockPackingSizeRepository;
    private readonly PricingStrategyService _sut;

    public PricingStrategyServiceTests()
    {
        _mockPricingStrategy = new Mock<IPricingStrategy>();
        _startDate = _fixture.Create<DateTimeOffset>();
        _contractDate = _fixture.Create<DateTimeOffset>().AddHours(10);
        var defaultPackingSize = _fixture.Create<PackingSize>();
        var defaultUrgency = _fixture.Create<Urgency>();
        _defaultAddress = _fixture.Create<Address>();
        _mockPricingStrategy.Setup(s => s.Name).Returns("SimpleDeliveryStrategy");
        _mockPricingStrategy
            .Setup(s => s.CalculateDeliveryPriceWithoutVat(
                It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(),
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(),
                It.IsAny<PackingSize>(), It.IsAny<double>(), It.IsAny<double>()))
            .Returns(0);

        _mockUrgencyRepository = new Mock<IUrgencyRepository>();
        _mockUrgencyRepository
        .Setup(r => r.GetByName(It.IsAny<string>()))
        .Returns(defaultUrgency);

        _mockPackingSizeRepository = new Mock<IPackingSizeRepository>();
        _mockPackingSizeRepository
        .Setup(r => r.GetByName(It.IsAny<string>()))
        .Returns(defaultPackingSize);

        _sut = new PricingStrategyService(
            [_mockPricingStrategy.Object],
            _mockUrgencyRepository.Object,
            _mockPackingSizeRepository.Object);
    }

    private Delivery MakeDelivery(
        Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations.PricingStrategy pricingStrategy = Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations.PricingStrategy.SimpleDeliveryStrategy,
        List<DeliveryStep>? steps = null,
        double? totalPrice = null,
        string packingSize = "Standard",
        string urgency = "Normal") =>
        new()
        {
            PricingStrategy = pricingStrategy,
            Code = _fixture.Create<string>(),
            CustomerId = _fixture.Create<Guid>(),
            Urgency = urgency,
            PackingSize = packingSize,
            InsulatedBox = false,
            ContractDate = _contractDate,
            StartDate = _startDate,
            Steps = steps ?? [],
            TotalPrice = totalPrice
        };

    private static DeliveryStep MakeStep(StepType type, DeliveryZone zone, double distance = 0)
    {
        return new DeliveryStep(type, 1, _defaultAddress!, distance)
        {
            Id = Guid.NewGuid(),
            StepAddress = _defaultAddress!,
            StepZone = zone
        };
    }

    private PricingStrategyService MakeSutWith(
        IUrgencyRepository? urgencies = null,
        IPackingSizeRepository? packingSizes = null,
        params IPricingStrategy[] strategies) =>
        new(strategies, urgencies ?? _mockUrgencyRepository.Object, packingSizes ?? _mockPackingSizeRepository.Object);

    [Fact]
    public void CalculatePrice_DelegatesToMatchingStrategy_AndNotToOthers()
    {
        // Arrange
        var matchingMock = new Mock<IPricingStrategy>();
        matchingMock.Setup(s => s.Name).Returns("SimpleDeliveryStrategy");
        matchingMock.Setup(s => s.CalculateDeliveryPriceWithoutVat(
            It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(),
            It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(),
            It.IsAny<PackingSize>(), It.IsAny<double>(), It.IsAny<double>())).Returns(42.0);

        var otherMock = new Mock<IPricingStrategy>();
        otherMock.Setup(s => s.Name).Returns("TourDeliveryStrategy");
        otherMock.Setup(s => s.CalculateDeliveryPriceWithoutVat(
            It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(),
            It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(),
            It.IsAny<PackingSize>(), It.IsAny<double>(), It.IsAny<double>())).Returns(99.0);

        // Act
        var sut = MakeSutWith(strategies: [matchingMock.Object, otherMock.Object]);
        var result = sut.CalculateDeliveryPriceWithoutVat(MakeDelivery());

        // Assert
        result.Should().Be(42.0);
        matchingMock.Verify(s => s.CalculateDeliveryPriceWithoutVat(
            It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(),
            It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(),
            It.IsAny<PackingSize>(), It.IsAny<double>(), It.IsAny<double>()), Times.Once);
        otherMock.Verify(s => s.CalculateDeliveryPriceWithoutVat(
            It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(),
            It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(),
            It.IsAny<PackingSize>(), It.IsAny<double>(), It.IsAny<double>()), Times.Never);
    }

    [Fact]
    public void CalculatePrice_WhenCustomStrategy_ReturnsTotalPriceWithoutCallingStrategy()
    {
        // Arrange
        var customMock = new Mock<IPricingStrategy>();
        customMock.Setup(s => s.Name).Returns("CustomStrategy");
        var sut = MakeSutWith(strategies: [customMock.Object]);

        // Act
        var result = sut.CalculateDeliveryPriceWithoutVat(
            MakeDelivery(Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations.PricingStrategy.CustomStrategy, totalPrice: 99.5));

        // Assert
        result.Should().Be(99.5);
        customMock.Verify(s => s.CalculateDeliveryPriceWithoutVat(
            It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(),
            It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(),
            It.IsAny<PackingSize>(), It.IsAny<double>(), It.IsAny<double>()), Times.Never);
    }

    [Fact]
    public void CalculatePrice_WhenNoStrategyMatches_Throws()
    {
        // Arrange
        var tourMock = new Mock<IPricingStrategy>();
        tourMock.Setup(s => s.Name).Returns("TourDeliveryStrategy");
        var sut = MakeSutWith(strategies: [tourMock.Object]);

        // Act
        var act = () => sut.CalculateDeliveryPriceWithoutVat(MakeDelivery());

        // Assert
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void CalculatePrice_CountsOnlyPickupStepsAsPickups()
    {
        // Arrange & Act
        var steps = new List<DeliveryStep>
        {
            MakeStep(StepType.Pickup, CoreZone),
            MakeStep(StepType.Pickup, CoreZone),
            MakeStep(StepType.Dropoff, CoreZone),
        };

        _sut.CalculateDeliveryPriceWithoutVat(MakeDelivery(steps: steps));

        // Assert
        _mockPricingStrategy.Verify(s => s.CalculateDeliveryPriceWithoutVat(
            It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(),
            2, It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(),
            It.IsAny<PackingSize>(), It.IsAny<double>(), It.IsAny<double>()), Times.Once);
    }

    [Fact]
    public void CalculatePrice_CountsDropoffStepsPerZone()
    {
        // Arrange
        var steps = new List<DeliveryStep>
        {
            MakeStep(StepType.Dropoff, CoreZone),
            MakeStep(StepType.Dropoff, CoreZone),
            MakeStep(StepType.Dropoff, BorderZone),
            MakeStep(StepType.Dropoff, PeripheryZone),
            MakeStep(StepType.Dropoff, OutsideZone),
            MakeStep(StepType.Dropoff, OutsideZone),
        };

        // Act
        _sut.CalculateDeliveryPriceWithoutVat(MakeDelivery(steps: steps));

        // Assert
        _mockPricingStrategy.Verify(s => s.CalculateDeliveryPriceWithoutVat(
            It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(),
            It.IsAny<int>(), 2, 1, 1, 2,
            It.IsAny<PackingSize>(), It.IsAny<double>(), It.IsAny<double>()), Times.Once);
    }

    [Fact]
    public void CalculatePrice_SumsDistanceAcrossAllSteps()
    {
        // Arrange
        var distance1 = _fixture.Create<double>();
        var distance2 = _fixture.Create<double>();
        var steps = new List<DeliveryStep>
        {
            MakeStep(StepType.Pickup, CoreZone, distance: distance1),
            MakeStep(StepType.Dropoff, CoreZone, distance: distance2),
        };

        // Act
        _sut.CalculateDeliveryPriceWithoutVat(MakeDelivery(steps: steps));

        // Assert
        _mockPricingStrategy.Verify(s => s.CalculateDeliveryPriceWithoutVat(
            It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(),
            It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(),
            It.IsAny<PackingSize>(), It.IsAny<double>(), distance1 + distance2), Times.Once);
    }

    [Fact]
    public void CalculatePrice_PassesUrgencyCoefficientFromRepository()
    {
        // Arrange & Act
        var express = _fixture.Create<Urgency>() with { PriceCoefficient = 1.5 };
        _mockUrgencyRepository.Setup(r => r.GetByName(express.Name)).Returns(express);

        _sut.CalculateDeliveryPriceWithoutVat(MakeDelivery(urgency: express.Name));

        // Assert
        _mockPricingStrategy.Verify(s => s.CalculateDeliveryPriceWithoutVat(
            It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(),
            It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(),
            It.IsAny<PackingSize>(), 1.5, It.IsAny<double>()), Times.Once);
    }

    [Fact]
    public void CalculatePrice_PassesPackingSizeFromRepository()
    {
        // Arrange
        var large = _fixture.Create<PackingSize>();
        _mockPackingSizeRepository.Setup(r => r.GetByName(large.Name)).Returns(large);

        // Act
        _sut.CalculateDeliveryPriceWithoutVat(MakeDelivery(packingSize: large.Name));

        // Assert
        _mockPricingStrategy.Verify(s => s.CalculateDeliveryPriceWithoutVat(
            It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(),
            It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(),
            large, It.IsAny<double>(), It.IsAny<double>()), Times.Once);
    }
}
