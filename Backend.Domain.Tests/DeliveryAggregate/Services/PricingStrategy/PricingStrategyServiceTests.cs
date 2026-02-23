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
    {
        Name = "Test",
        StreetInfo = "1 rue Test",
        Postcode = "38000",
        City = "Grenoble"
    };

    private static Delivery MakeDelivery(
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
            ExactTime = false,
            ContractDate = ContractDate,
            StartDate = StartDate,
            Steps = steps ?? [],
            TotalPrice = totalPrice
        };
    }

    private static DeliveryStep MakeStep(StepTypeEnum type, DeliveryZone zone, double distance = 0) =>
        new(type, 1, DefaultAddress, zone, distance, ContractDate);

    [Fact]
    public void CalculatePrice_DelegatesToMatchingStrategy_AndNotToOthers()
    {
        // Arrange
        var matchingStrategyMock = new Mock<IPricingStrategy>();
        matchingStrategyMock.Setup(s => s.Name).Returns("SimpleDeliveryStrategy");
        matchingStrategyMock.Setup(s => s.CalculateDeliveryPriceWithoutVat(
            It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(),
            It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(),
            It.IsAny<PackingSize>(), It.IsAny<double>(), It.IsAny<double>()
        )).Returns(42.0);

        var otherStrategyMock = new Mock<IPricingStrategy>();
        otherStrategyMock.Setup(s => s.Name).Returns("TourDeliveryStrategy");
        otherStrategyMock.Setup(s => s.CalculateDeliveryPriceWithoutVat(
            It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(),
            It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(),
            It.IsAny<PackingSize>(), It.IsAny<double>(), It.IsAny<double>()
        )).Returns(99.0); // deliberately different — ensures we're not returning this

        var urgenciesMock = new Mock<IUrgencyRepository>();
        urgenciesMock.Setup(r => r.GetUrgency(It.IsAny<string>())).Returns(DefaultUrgency);
        var packingSizesMock = new Mock<IPackingSizeRepository>();
        packingSizesMock.Setup(r => r.FromName(It.IsAny<string>())).Returns(DefaultPackingSize);

        var sut = new PricingStrategyService(
            [matchingStrategyMock.Object, otherStrategyMock.Object],
            urgenciesMock.Object,
            packingSizesMock.Object);
        var delivery = MakeDelivery(PricingStrategyEnum.SimpleDeliveryStrategy);

        // Act
        var result = sut.CalculateDeliveryPriceWithoutVat(delivery);

        // Assert
        result.Should().Be(42.0); // returns the matching strategy's result, not the other's (99.0)
        matchingStrategyMock.Verify(s => s.CalculateDeliveryPriceWithoutVat(
            It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(),
            It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(),
            It.IsAny<PackingSize>(), It.IsAny<double>(), It.IsAny<double>()
        ), Times.Once);
        otherStrategyMock.Verify(s => s.CalculateDeliveryPriceWithoutVat(
            It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(),
            It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(),
            It.IsAny<PackingSize>(), It.IsAny<double>(), It.IsAny<double>()
        ), Times.Never);
    }

    [Fact]
    public void CalculatePrice_WhenCustomStrategy_ReturnsTotalPrice()
    {
        // Arrange
        var strategyMock = new Mock<IPricingStrategy>();
        strategyMock.Setup(s => s.Name).Returns("CustomStrategy");

        var urgenciesMock = new Mock<IUrgencyRepository>();
        urgenciesMock.Setup(r => r.GetUrgency(It.IsAny<string>())).Returns(DefaultUrgency);
        var packingSizesMock = new Mock<IPackingSizeRepository>();
        packingSizesMock.Setup(r => r.FromName(It.IsAny<string>())).Returns(DefaultPackingSize);

        var sut = new PricingStrategyService([strategyMock.Object], urgenciesMock.Object, packingSizesMock.Object);
        var delivery = MakeDelivery(PricingStrategyEnum.CustomStrategy, totalPrice: 99.5);

        // Act
        var result = sut.CalculateDeliveryPriceWithoutVat(delivery);

        // Assert
        result.Should().Be(99.5);
        strategyMock.Verify(s => s.CalculateDeliveryPriceWithoutVat(
            It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(),
            It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(),
            It.IsAny<PackingSize>(), It.IsAny<double>(), It.IsAny<double>()
        ), Times.Never);
    }

    [Fact]
    public void CalculatePrice_CountsOnlyPickupStepsAsPickups()
    {
        // Arrange
        int capturedPickupCount = -1;
        var strategyMock = new Mock<IPricingStrategy>();
        strategyMock.Setup(s => s.Name).Returns("SimpleDeliveryStrategy");
        strategyMock.Setup(s => s.CalculateDeliveryPriceWithoutVat(
            It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(),
            It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(),
            It.IsAny<PackingSize>(), It.IsAny<double>(), It.IsAny<double>()
        )).Callback((DateTimeOffset _, DateTimeOffset _, int pickups, int _, int _, int _, int _, PackingSize _, double _, double _) =>
        {
            capturedPickupCount = pickups;
        }).Returns(0);

        var urgenciesMock = new Mock<IUrgencyRepository>();
        urgenciesMock.Setup(r => r.GetUrgency(It.IsAny<string>())).Returns(DefaultUrgency);
        var packingSizesMock = new Mock<IPackingSizeRepository>();
        packingSizesMock.Setup(r => r.FromName(It.IsAny<string>())).Returns(DefaultPackingSize);

        var steps = new List<DeliveryStep>
        {
            MakeStep(StepTypeEnum.Pickup, GrenobleZone),
            MakeStep(StepTypeEnum.Pickup, GrenobleZone),
            MakeStep(StepTypeEnum.Dropoff, GrenobleZone),
        };
        var sut = new PricingStrategyService([strategyMock.Object], urgenciesMock.Object, packingSizesMock.Object);
        var delivery = MakeDelivery(steps: steps);

        // Act
        sut.CalculateDeliveryPriceWithoutVat(delivery);

        // Assert
        capturedPickupCount.Should().Be(2);
    }

    [Fact]
    public void CalculatePrice_CountsDropoffStepsPerZone()
    {
        // Arrange
        int capturedGrenoble = -1, capturedBorder = -1, capturedPeriphery = -1, capturedOutside = -1;
        var strategyMock = new Mock<IPricingStrategy>();
        strategyMock.Setup(s => s.Name).Returns("SimpleDeliveryStrategy");
        strategyMock.Setup(s => s.CalculateDeliveryPriceWithoutVat(
            It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(),
            It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(),
            It.IsAny<PackingSize>(), It.IsAny<double>(), It.IsAny<double>()
        )).Callback((DateTimeOffset _, DateTimeOffset _, int _, int g, int b, int p, int o, PackingSize _, double _, double _) =>
        {
            capturedGrenoble = g;
            capturedBorder = b;
            capturedPeriphery = p;
            capturedOutside = o;
        }).Returns(0);

        var urgenciesMock = new Mock<IUrgencyRepository>();
        urgenciesMock.Setup(r => r.GetUrgency(It.IsAny<string>())).Returns(DefaultUrgency);
        var packingSizesMock = new Mock<IPackingSizeRepository>();
        packingSizesMock.Setup(r => r.FromName(It.IsAny<string>())).Returns(DefaultPackingSize);

        var steps = new List<DeliveryStep>
        {
            MakeStep(StepTypeEnum.Dropoff, GrenobleZone),
            MakeStep(StepTypeEnum.Dropoff, GrenobleZone),
            MakeStep(StepTypeEnum.Dropoff, BorderZone),
            MakeStep(StepTypeEnum.Dropoff, PeripheryZone),
            MakeStep(StepTypeEnum.Dropoff, OutsideZone),
            MakeStep(StepTypeEnum.Dropoff, OutsideZone),
        };
        var sut = new PricingStrategyService([strategyMock.Object], urgenciesMock.Object, packingSizesMock.Object);
        var delivery = MakeDelivery(steps: steps);

        // Act
        sut.CalculateDeliveryPriceWithoutVat(delivery);

        // Assert
        capturedGrenoble.Should().Be(2);
        capturedBorder.Should().Be(1);
        capturedPeriphery.Should().Be(1);
        capturedOutside.Should().Be(2);
    }

    [Fact]
    public void CalculatePrice_SumsDistanceAcrossAllSteps()
    {
        // Arrange
        double capturedDistance = -1;
        var strategyMock = new Mock<IPricingStrategy>();
        strategyMock.Setup(s => s.Name).Returns("SimpleDeliveryStrategy");
        strategyMock.Setup(s => s.CalculateDeliveryPriceWithoutVat(
            It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(),
            It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(),
            It.IsAny<PackingSize>(), It.IsAny<double>(), It.IsAny<double>()
        )).Callback((DateTimeOffset _, DateTimeOffset _, int _, int _, int _, int _, int _, PackingSize _, double _, double d) =>
        {
            capturedDistance = d;
        }).Returns(0);

        var urgenciesMock = new Mock<IUrgencyRepository>();
        urgenciesMock.Setup(r => r.GetUrgency(It.IsAny<string>())).Returns(DefaultUrgency);
        var packingSizesMock = new Mock<IPackingSizeRepository>();
        packingSizesMock.Setup(r => r.FromName(It.IsAny<string>())).Returns(DefaultPackingSize);

        var steps = new List<DeliveryStep>
        {
            MakeStep(StepTypeEnum.Pickup, GrenobleZone, distance: 3.5),
            MakeStep(StepTypeEnum.Dropoff, GrenobleZone, distance: 6.0),
        };
        var sut = new PricingStrategyService([strategyMock.Object], urgenciesMock.Object, packingSizesMock.Object);
        var delivery = MakeDelivery(steps: steps);

        // Act
        sut.CalculateDeliveryPriceWithoutVat(delivery);

        // Assert
        capturedDistance.Should().Be(9.5);
    }

    [Fact]
    public void CalculatePrice_PassesUrgencyCoefficientFromRepository()
    {
        // Arrange
        double capturedCoefficient = -1;
        var urgency = new Urgency(2, "Express", PriceCoefficient: 1.5);
        var strategyMock = new Mock<IPricingStrategy>();
        strategyMock.Setup(s => s.Name).Returns("SimpleDeliveryStrategy");
        strategyMock.Setup(s => s.CalculateDeliveryPriceWithoutVat(
            It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(),
            It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(),
            It.IsAny<PackingSize>(), It.IsAny<double>(), It.IsAny<double>()
        )).Callback((DateTimeOffset _, DateTimeOffset _, int _, int _, int _, int _, int _, PackingSize _, double coeff, double _) =>
        {
            capturedCoefficient = coeff;
        }).Returns(0);

        var urgenciesMock = new Mock<IUrgencyRepository>();
        urgenciesMock.Setup(r => r.GetUrgency("Express")).Returns(urgency);
        var packingSizesMock = new Mock<IPackingSizeRepository>();
        packingSizesMock.Setup(r => r.FromName(It.IsAny<string>())).Returns(DefaultPackingSize);

        var sut = new PricingStrategyService([strategyMock.Object], urgenciesMock.Object, packingSizesMock.Object);
        var delivery = MakeDelivery(urgency: "Express");

        // Act
        sut.CalculateDeliveryPriceWithoutVat(delivery);

        // Assert
        capturedCoefficient.Should().Be(1.5);
    }

    [Fact]
    public void CalculatePrice_PassesPackingSizeFromRepository()
    {
        // Arrange
        PackingSize? capturedPackingSize = null;
        var packingSize = new PackingSize(2, "Large", 30, 80, 5, 8);
        var strategyMock = new Mock<IPricingStrategy>();
        strategyMock.Setup(s => s.Name).Returns("SimpleDeliveryStrategy");
        strategyMock.Setup(s => s.CalculateDeliveryPriceWithoutVat(
            It.IsAny<DateTimeOffset>(), It.IsAny<DateTimeOffset>(),
            It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(), It.IsAny<int>(),
            It.IsAny<PackingSize>(), It.IsAny<double>(), It.IsAny<double>()
        )).Callback((DateTimeOffset _, DateTimeOffset _, int _, int _, int _, int _, int _, PackingSize ps, double _, double _) =>
        {
            capturedPackingSize = ps;
        }).Returns(0);

        var urgenciesMock = new Mock<IUrgencyRepository>();
        urgenciesMock.Setup(r => r.GetUrgency(It.IsAny<string>())).Returns(DefaultUrgency);
        var packingSizesMock = new Mock<IPackingSizeRepository>();
        packingSizesMock.Setup(r => r.FromName("Large")).Returns(packingSize);

        var sut = new PricingStrategyService([strategyMock.Object], urgenciesMock.Object, packingSizesMock.Object);
        var delivery = MakeDelivery(packingSize: "Large");

        // Act
        sut.CalculateDeliveryPriceWithoutVat(delivery);

        // Assert
        capturedPackingSize.Should().Be(packingSize);
    }

    [Fact]
    public void CalculatePrice_WhenNoStrategyMatches_Throws()
    {
        // Arrange
        var strategyMock = new Mock<IPricingStrategy>();
        strategyMock.Setup(s => s.Name).Returns("TourDeliveryStrategy");

        var urgenciesMock = new Mock<IUrgencyRepository>();
        urgenciesMock.Setup(r => r.GetUrgency(It.IsAny<string>())).Returns(DefaultUrgency);
        var packingSizesMock = new Mock<IPackingSizeRepository>();
        packingSizesMock.Setup(r => r.FromName(It.IsAny<string>())).Returns(DefaultPackingSize);

        var sut = new PricingStrategyService([strategyMock.Object], urgenciesMock.Object, packingSizesMock.Object);
        var delivery = MakeDelivery(PricingStrategyEnum.SimpleDeliveryStrategy);

        // Act
        var act = () => sut.CalculateDeliveryPriceWithoutVat(delivery);

        // Assert
        act.Should().Throw<InvalidOperationException>();
    }
}
