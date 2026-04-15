using AwesomeAssertions;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.PricingStrategies;

namespace Backend.Domain.Tests.DeliveryAggregate.PricingStrategies;

public class TourDeliveryStrategyTests
{
    private readonly static PackingSize DefaultPackingSize = new("Standard", 10, 50, 0, 0);
    private readonly static DateTimeOffset BaseDate = new(2026, 1, 15, 10, 0, 0, TimeSpan.Zero);
    private static TourDeliveryStrategy MakeSut() => new();

    [Fact]
    public void Name_ShouldReturn_TourDelivery()
    {
        // Arrange & Act
        var sut = MakeSut();

        // Assert
        sut.ImplementedStrategy.Should().Be(PricingStrategy.TourDeliveryStrategy);
    }

    [Theory]
    [InlineData(1, 14.0)]
    [InlineData(2, 28.0)]
    [InlineData(3, 42.0)]
    public void CalculatePrice_PickupNumber_IsMultipliedByBasePrice(int pickupNumber, double expectedPickupCost)
    {
        // Arrange
        var sut = MakeSut();
        var contractDate = new DateTimeOffset(2026, 1, 14, 10, 0, 0, TimeSpan.Zero);
        var startDate = new DateTimeOffset(2026, 1, 14, 20, 0, 0, TimeSpan.Zero);

        // Act
        var result = sut.CalculateDeliveryPriceWithoutVat(startDate, contractDate, pickupNumber, 0, 0, 0, 0, DefaultPackingSize, 0, 0);

        // Assert
        result.Should().Be(2 + expectedPickupCost);
    }

    [Fact]
    public void CalculatePrice_WhenDeliveryIsOnSameDayAsContractDate_AddsExtraCost()
    {
        // Arrange
        var sut = MakeSut();
        var contractDate = BaseDate;
        var startDate = BaseDate.AddHours(5);

        // Act
        var result = sut.CalculateDeliveryPriceWithoutVat(startDate, contractDate, 0, 0, 0, 0, 0, DefaultPackingSize, 0, 0);

        // Assert
        result.Should().Be(2);
    }

    [Fact]
    public void CalculatePrice_WhenOrderIsAfter5PM_AndDeliveryIsNextDay_AddsSameDayCost()
    {
        // Arrange
        var sut = MakeSut();
        var contractDate = new DateTimeOffset(2026, 1, 15, 18, 0, 0, TimeSpan.Zero);
        var startDate = new DateTimeOffset(2026, 1, 16, 10, 0, 0, TimeSpan.Zero);

        // Act
        var result = sut.CalculateDeliveryPriceWithoutVat(startDate, contractDate, 0, 0, 0, 0, 0, DefaultPackingSize, 0, 0);

        // Assert
        result.Should().Be(2);
    }

    [Fact]
    public void CalculatePrice_WhenDeliveryIsOnDifferentDay_AppliesEarlyOrderDiscountAndNoSameDayCost()
    {
        // Arrange
        var sut = MakeSut();
        var contractDate = BaseDate;
        var startDate = BaseDate.AddHours(25);

        // Act
        var result = sut.CalculateDeliveryPriceWithoutVat(startDate, contractDate, 0, 0, 0, 0, 0, DefaultPackingSize, 0, 0);

        // Assert
        result.Should().Be(-2);
    }

    [Fact]
    public void CalculatePrice_WhenDelayIsAtMost2Hours_AppliesLastMinuteExtraCostAndSameDayCost()
    {
        // Arrange
        var sut = MakeSut();
        var contractDate = BaseDate;
        var startDate = BaseDate.AddHours(1);

        // Act
        var result = sut.CalculateDeliveryPriceWithoutVat(startDate, contractDate, 0, 0, 0, 0, 0, DefaultPackingSize, 0, 0);

        // Assert
        result.Should().Be(2 + 3);
    }

    [Fact]
    public void CalculatePrice_WhenDelayIsExactly2Hours_AppliesLastMinuteExtraCostAndSameDayCost()
    {
        // Arrange
        var sut = MakeSut();
        var contractDate = BaseDate;
        var startDate = BaseDate.AddHours(2);

        // Act
        var result = sut.CalculateDeliveryPriceWithoutVat(startDate, contractDate, 0, 0, 0, 0, 0, DefaultPackingSize, 0, 0);

        // Assert
        result.Should().Be(2 + 3);
    }

    [Fact]
    public void CalculatePrice_WhenDelayIsBetween2And18Hours_AppliesOnlySameDayCost()
    {
        // Arrange
        var sut = MakeSut();
        var contractDate = BaseDate;
        var startDate = BaseDate.AddHours(6);

        // Act
        var result = sut.CalculateDeliveryPriceWithoutVat(startDate, contractDate, 0, 0, 0, 0, 0, DefaultPackingSize, 0, 0);

        // Assert
        result.Should().Be(2);
    }

    [Theory]
    [InlineData(3, 0, 0, 0, 3 * 5.0)]
    [InlineData(0, 3, 0, 0, 3 * 8.0)]
    [InlineData(0, 0, 3, 0, 3 * 0.0)]
    [InlineData(0, 0, 0, 3, 3 * 0.0)]
    public void CalculatePrice_ZoneDropoffSteps_AreMultipliedByCorrectStepPrice(
        int core, int border, int periphery, int outside, double expectedZoneCost)
    {
        // Arrange
        var sut = MakeSut();
        var contractDate = new DateTimeOffset(2026, 1, 14, 10, 0, 0, TimeSpan.Zero);
        var startDate = new DateTimeOffset(2026, 1, 14, 16, 0, 0, TimeSpan.Zero);

        // Act
        var result = sut.CalculateDeliveryPriceWithoutVat(startDate, contractDate, 0, core, border, periphery, outside, DefaultPackingSize, 0, 0);

        // Assert
        result.Should().Be(2 + expectedZoneCost);
    }

    [Fact]
    public void CalculatePrice_IncludesPackingSizeTourPrice()
    {
        // Arrange
        var sut = MakeSut();
        var packingSize = DefaultPackingSize with { TourPrice = 7.0 };
        var contractDate = new DateTimeOffset(2026, 1, 14, 10, 0, 0, TimeSpan.Zero);
        var startDate = new DateTimeOffset(2026, 1, 14, 16, 0, 0, TimeSpan.Zero);

        // Act
        var result = sut.CalculateDeliveryPriceWithoutVat(startDate, contractDate, 0, 0, 0, 0, 0, packingSize, 0, 0);

        // Assert
        result.Should().Be(2 + 7.0);
    }

    [Fact]
    public void CalculatePrice_UrgencyCoefficientAndDistance_HaveNoEffect()
    {
        // Arrange
        var sut = MakeSut();
        var contractDate = new DateTimeOffset(2026, 1, 14, 10, 0, 0, TimeSpan.Zero);
        var startDate = new DateTimeOffset(2026, 1, 14, 20, 0, 0, TimeSpan.Zero);

        // Act
        var resultWithoutUrgency = sut.CalculateDeliveryPriceWithoutVat(startDate, contractDate, 0, 0, 0, 0, 0, DefaultPackingSize, 0, 0);
        var resultWithUrgency = sut.CalculateDeliveryPriceWithoutVat(startDate, contractDate, 0, 0, 0, 0, 0, DefaultPackingSize, 10.0, 100.0);

        // Assert
        resultWithUrgency.Should().Be(resultWithoutUrgency);
    }

    [Fact]
    public void CalculatePrice_WithAllFactors_ReturnsCorrectTotal()
    {
        // Arrange
        var sut = MakeSut();
        var packingSize = DefaultPackingSize with { TourPrice = 4.0 };
        var contractDate = new DateTimeOffset(2026, 1, 14, 10, 0, 0, TimeSpan.Zero);
        var startDate = new DateTimeOffset(2026, 1, 14, 16, 0, 0, TimeSpan.Zero);

        // Act
        var result = sut.CalculateDeliveryPriceWithoutVat(startDate, contractDate, 2, 1, 2, 1, 1, packingSize, 0, 0);

        // Assert
        result.Should().Be(14 * 2 + 2 + 4 + 1 * 5 + 2 * 8 + 1 * 0 + 1 * 0, "Coût PEC * nb PEC +\nCoût PEC le jour même +\ncoût du colisage +\nnb dépôts dans la zone centrale x coût de la zone +\nnb dépôts en bordure x coût de la zone +\nnb dépôts périphérie x coût de la zone +\nnb dépôts en extérieur x coût de la zone");
    }
}
