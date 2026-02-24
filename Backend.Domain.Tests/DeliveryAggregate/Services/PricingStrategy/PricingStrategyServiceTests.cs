using AwesomeAssertions;
using Ggio.BikeSherpa.Backend.Domain;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.PricingStrategies;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Services.PricingStrategy;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Services.Repositories;
using Mediator;
using Moq;

namespace Backend.Domain.Tests.DeliveryAggregate.Services.PricingStrategy;

public class PricingStrategyServiceTests
{
    private readonly static DateTimeOffset StartDate = new(2026, 1, 14, 10, 0, 0, TimeSpan.Zero);
    private readonly static DateTimeOffset ContractDate = new(2026, 1, 14, 20, 0, 0, TimeSpan.Zero);
    private readonly static PackingSize DefaultPackingSize = new(1, "Standard", 10, 50, 0, 0);
    private readonly static Urgency DefaultUrgency = new(1, "Normal", PriceCoefficient: 0);
    private readonly static DeliveryZone GrenobleZone = new(1, "Grenoble", []);
    private readonly static DeliveryZone BorderZone = new(2, "Limitrophe", []);
    private readonly static DeliveryZone PeripheryZone = new(3, "Périphérie", []);
    private readonly static DeliveryZone OutsideZone = new(4, "Extérieur", []);
    private readonly static Address DefaultAddress = new()
    { Name = "Test", StreetInfo = "1 rue Test", Postcode = "38000", City = "Grenoble" };

    private record StrategyArgs(
        DateTimeOffset ArgStartDate, DateTimeOffset ArgContractDate,
        int Pickups, int Grenoble, int Border, int Periphery, int Outside,
        PackingSize PackingSize, double Coefficient, double Distance);

    private readonly Mock<IUrgencyRepository> _urgenciesMock;
    private readonly Mock<IPackingSizeRepository> _packingSizesMock;
    private readonly PricingStrategyService _sut;
    private StrategyArgs? _capturedArgs;

    public PricingStrategyServiceTests()
    {
        var strategyMock = new Mock<IPricingStrategy>();
        strategyMock.Setup(s => s.Name).Returns("SimpleDeliveryStrategy");
        strategyMock
            .Setup(s => s.CalculateDeliveryPriceWithoutVat(
                It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(),
                It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(),
                It.IsAny<PackingSize>(), It.IsAny<double>(), It.IsAny<double>()))
            .Callback((DateTimeOffset sd, DateTimeOffset cd,
                       int pickups, int g, int b, int p, int o,
                       PackingSize ps, double coeff, double dist) =>
                _capturedArgs = new(sd, cd, pickups, g, b, p, o, ps, coeff, dist))
            .Returns(0);

        _urgenciesMock = new Mock<IUrgencyRepository>();
        _urgenciesMock.Setup(r => r.GetUrgency(It.IsAny<string>())).Returns(DefaultUrgency);

        _packingSizesMock = new Mock<IPackingSizeRepository>();
        _packingSizesMock.Setup(r => r.FromName(It.IsAny<string>())).Returns(DefaultPackingSize);

        _sut = new PricingStrategyService(
            [strategyMock.Object], _urgenciesMock.Object, _packingSizesMock.Object);
    }

    private Delivery MakeDelivery(
        PricingStrategyEnum pricingStrategy = PricingStrategyEnum.SimpleDeliveryStrategy,
        List<DeliveryStep>? steps = null,
        double? totalPrice = null,
        string packingSize = "Standard",
        string urgency = "Normal")
    {
        var mediatorMock = new Mock<IMediator>();
        return new Delivery(mediatorMock.Object)
        {
            PricingStrategy = pricingStrategy,
            Code = "TEST-001",
            CustomerId = Guid.NewGuid(),
            Urgency = urgency,
            PackingSize = packingSize,
            InsulatedBox = false,
            ContractDate = ContractDate,
            StartDate = StartDate,
            Steps = steps ?? [],
            TotalPrice = totalPrice
        };
    }

    private static DeliveryStep MakeStep(StepTypeEnum type, DeliveryZone zone, double distance = 0) =>
        new(type, 1, DefaultAddress, zone, distance, ContractDate);

    private PricingStrategyService MakeSutWith(
        IUrgencyRepository? urgencies = null,
        IPackingSizeRepository? packingSizes = null,
        params IPricingStrategy[] strategies) =>
        new(strategies, urgencies ?? _urgenciesMock.Object, packingSizes ?? _packingSizesMock.Object);

    [Fact]
    public void CalculatePrice_DelegatesToMatchingStrategy_AndNotToOthers()
    {
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

        var sut = MakeSutWith(strategies: [matchingMock.Object, otherMock.Object]);
        var result = sut.CalculateDeliveryPriceWithoutVat(MakeDelivery());

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
        var customMock = new Mock<IPricingStrategy>();
        customMock.Setup(s => s.Name).Returns("CustomStrategy");
        var sut = MakeSutWith(strategies: [customMock.Object]);

        var result = sut.CalculateDeliveryPriceWithoutVat(
            MakeDelivery(PricingStrategyEnum.CustomStrategy, totalPrice: 99.5));

        result.Should().Be(99.5);
        customMock.Verify(s => s.CalculateDeliveryPriceWithoutVat(
            It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(),
            It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(),
            It.IsAny<PackingSize>(), It.IsAny<double>(), It.IsAny<double>()), Times.Never);
    }

    [Fact]
    public void CalculatePrice_WhenNoStrategyMatches_Throws()
    {
        var tourMock = new Mock<IPricingStrategy>();
        tourMock.Setup(s => s.Name).Returns("TourDeliveryStrategy");
        var sut = MakeSutWith(strategies: [tourMock.Object]);

        var act = () => sut.CalculateDeliveryPriceWithoutVat(MakeDelivery());

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void CalculatePrice_CountsOnlyPickupStepsAsPickups()
    {
        var steps = new List<DeliveryStep>
        {
            MakeStep(StepTypeEnum.Pickup, GrenobleZone),
            MakeStep(StepTypeEnum.Pickup, GrenobleZone),
            MakeStep(StepTypeEnum.Dropoff, GrenobleZone),
        };

        _sut.CalculateDeliveryPriceWithoutVat(MakeDelivery(steps: steps));

        _capturedArgs!.Pickups.Should().Be(2);
    }

    [Fact]
    public void CalculatePrice_CountsDropoffStepsPerZone()
    {
        var steps = new List<DeliveryStep>
        {
            MakeStep(StepTypeEnum.Dropoff, GrenobleZone),
            MakeStep(StepTypeEnum.Dropoff, GrenobleZone),
            MakeStep(StepTypeEnum.Dropoff, BorderZone),
            MakeStep(StepTypeEnum.Dropoff, PeripheryZone),
            MakeStep(StepTypeEnum.Dropoff, OutsideZone),
            MakeStep(StepTypeEnum.Dropoff, OutsideZone),
        };

        _sut.CalculateDeliveryPriceWithoutVat(MakeDelivery(steps: steps));

        _capturedArgs!.Grenoble.Should().Be(2);
        _capturedArgs.Border.Should().Be(1);
        _capturedArgs.Periphery.Should().Be(1);
        _capturedArgs.Outside.Should().Be(2);
    }

    [Fact]
    public void CalculatePrice_SumsDistanceAcrossAllSteps()
    {
        var steps = new List<DeliveryStep>
        {
            MakeStep(StepTypeEnum.Pickup, GrenobleZone, distance: 3.5),
            MakeStep(StepTypeEnum.Dropoff, GrenobleZone, distance: 6.0),
        };

        _sut.CalculateDeliveryPriceWithoutVat(MakeDelivery(steps: steps));

        _capturedArgs!.Distance.Should().Be(9.5);
    }

    [Fact]
    public void CalculatePrice_PassesUrgencyCoefficientFromRepository()
    {
        var express = new Urgency(2, "Express", PriceCoefficient: 1.5);
        _urgenciesMock.Setup(r => r.GetUrgency("Express")).Returns(express);

        _sut.CalculateDeliveryPriceWithoutVat(MakeDelivery(urgency: "Express"));

        _capturedArgs!.Coefficient.Should().Be(1.5);
    }

    [Fact]
    public void CalculatePrice_PassesPackingSizeFromRepository()
    {
        var large = new PackingSize(2, "Large", 30, 80, 5, 8);
        _packingSizesMock.Setup(r => r.FromName("Large")).Returns(large);

        _sut.CalculateDeliveryPriceWithoutVat(MakeDelivery(packingSize: "Large"));

        _capturedArgs!.PackingSize.Should().Be(large);
    }
}

