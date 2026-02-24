using AwesomeAssertions;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.PricingStrategies;

namespace Backend.Domain.Tests.DeliveryAggregate.PricingStrategies;

public class TourDeliveryStrategyTests
{
    private static readonly PackingSize DefaultPackingSize = new(1, "Standard", 10, 50, 0, 0);
    private static readonly DateTimeOffset BaseDate = new(2026, 1, 15, 10, 0, 0, TimeSpan.Zero);
    private static TourDeliveryStrategy MakeSut() => new();

    [Fact]
    public void Name_ShouldReturn_TourDelivery()
    {
        var sut = MakeSut();

        sut.Name.Should().Be("TourDelivery");
    }

    [Theory]
    [InlineData(1, 14.0)]
    [InlineData(2, 28.0)]
    [InlineData(3, 42.0)]
    public void CalculatePrice_PickupNumber_IsMultipliedByBasePrice(int pickupNumber, double expectedPickupCost)
    {
        var sut = MakeSut();
        var contractDate = new DateTimeOffset(2026, 1, 14, 10, 0, 0, TimeSpan.Zero);
        var startDate = new DateTimeOffset(2026, 1, 14, 20, 0, 0, TimeSpan.Zero);

        var result = sut.CalculateDeliveryPriceWithoutVat(startDate, contractDate, pickupNumber, 0, 0, 0, 0, DefaultPackingSize, 0, 0);

        result.Should().Be(2 + expectedPickupCost);
    }

    [Fact]
    public void CalculatePrice_WhenDeliveryIsOnSameDayAsContractDate_AddsExtraCost()
    {
        var sut = MakeSut();
        var contractDate = BaseDate;
        var startDate = BaseDate.AddHours(5);

        var result = sut.CalculateDeliveryPriceWithoutVat(startDate, contractDate, 0, 0, 0, 0, 0, DefaultPackingSize, 0, 0);

        result.Should().Be(2);
    }

    [Fact]
    public void CalculatePrice_WhenOrderIsAfter5PM_AndDeliveryIsNextDay_AddsSameDayCost()
    {
        var sut = MakeSut();
        var contractDate = new DateTimeOffset(2026, 1, 15, 18, 0, 0, TimeSpan.Zero);
        var startDate = new DateTimeOffset(2026, 1, 16, 10, 0, 0, TimeSpan.Zero);

        var result = sut.CalculateDeliveryPriceWithoutVat(startDate, contractDate, 0, 0, 0, 0, 0, DefaultPackingSize, 0, 0);

        result.Should().Be(2);
    }

    [Fact]
    public void CalculatePrice_WhenDeliveryIsOnDifferentDay_AppliesEarlyOrderDiscountAndNoSameDayCost()
    {
        var sut = MakeSut();
        var contractDate = BaseDate;
        var startDate = BaseDate.AddHours(25);

        var result = sut.CalculateDeliveryPriceWithoutVat(startDate, contractDate, 0, 0, 0, 0, 0, DefaultPackingSize, 0, 0);

        result.Should().Be(-2);
    }

    [Fact]
    public void CalculatePrice_WhenDelayIsAtMost2Hours_AppliesLastMinuteExtraCostAndSameDayCost()
    {
        var sut = MakeSut();
        var contractDate = BaseDate;
        var startDate = BaseDate.AddHours(1);

        var result = sut.CalculateDeliveryPriceWithoutVat(startDate, contractDate, 0, 0, 0, 0, 0, DefaultPackingSize, 0, 0);

        result.Should().Be(2 + 3);
    }

    [Fact]
    public void CalculatePrice_WhenDelayIsExactly2Hours_AppliesLastMinuteExtraCostAndSameDayCost()
    {
        var sut = MakeSut();
        var contractDate = BaseDate;
        var startDate = BaseDate.AddHours(2);

        var result = sut.CalculateDeliveryPriceWithoutVat(startDate, contractDate, 0, 0, 0, 0, 0, DefaultPackingSize, 0, 0);

        result.Should().Be(2 + 3);
    }

    [Fact]
    public void CalculatePrice_WhenDelayIsBetween2And18Hours_AppliesOnlySameDayCost()
    {
        var sut = MakeSut();
        var contractDate = BaseDate;
        var startDate = BaseDate.AddHours(6);

        var result = sut.CalculateDeliveryPriceWithoutVat(startDate, contractDate, 0, 0, 0, 0, 0, DefaultPackingSize, 0, 0);

        result.Should().Be(2);
    }

    [Theory]
    [InlineData(3, 0, 0, 0, 3 * 5.0)]
    [InlineData(0, 3, 0, 0, 3 * 8.0)]
    [InlineData(0, 0, 3, 0, 3 * 0.0)]
    [InlineData(0, 0, 0, 3, 3 * 0.0)]
    public void CalculatePrice_ZoneDropoffSteps_AreMultipliedByCorrectStepPrice(
        int grenoble, int border, int periphery, int outside, double expectedZoneCost)
    {
        var sut = MakeSut();
        var contractDate = new DateTimeOffset(2026, 1, 14, 10, 0, 0, TimeSpan.Zero);
        var startDate = new DateTimeOffset(2026, 1, 14, 16, 0, 0, TimeSpan.Zero);

        var result = sut.CalculateDeliveryPriceWithoutVat(startDate, contractDate, 0, grenoble, border, periphery, outside, DefaultPackingSize, 0, 0);

        result.Should().Be(2 + expectedZoneCost);
    }

    [Fact]
    public void CalculatePrice_IncludesPackingSizeTourPrice()
    {
        var sut = MakeSut();
        var packingSize = DefaultPackingSize with { TourPrice = 7.0 };
        var contractDate = new DateTimeOffset(2026, 1, 14, 10, 0, 0, TimeSpan.Zero);
        var startDate = new DateTimeOffset(2026, 1, 14, 16, 0, 0, TimeSpan.Zero);

        var result = sut.CalculateDeliveryPriceWithoutVat(startDate, contractDate, 0, 0, 0, 0, 0, packingSize, 0, 0);

        result.Should().Be(2 + 7.0);
    }

    [Fact]
    public void CalculatePrice_UrgencyCoefficientAndDistance_HaveNoEffect()
    {
        var sut = MakeSut();
        var contractDate = new DateTimeOffset(2026, 1, 14, 10, 0, 0, TimeSpan.Zero);
        var startDate = new DateTimeOffset(2026, 1, 14, 20, 0, 0, TimeSpan.Zero);

        var resultWithoutUrgency = sut.CalculateDeliveryPriceWithoutVat(startDate, contractDate, 0, 0, 0, 0, 0, DefaultPackingSize, 0, 0);
        var resultWithUrgency = sut.CalculateDeliveryPriceWithoutVat(startDate, contractDate, 0, 0, 0, 0, 0, DefaultPackingSize, 10.0, 100.0);

        resultWithUrgency.Should().Be(resultWithoutUrgency);
    }

    [Fact]
    public void CalculatePrice_WithAllFactors_ReturnsCorrectTotal()
    {
        var sut = MakeSut();
        var packingSize = DefaultPackingSize with { TourPrice = 4.0 };
        var contractDate = new DateTimeOffset(2026, 1, 14, 10, 0, 0, TimeSpan.Zero);
        var startDate = new DateTimeOffset(2026, 1, 14, 16, 0, 0, TimeSpan.Zero);

        var result = sut.CalculateDeliveryPriceWithoutVat(startDate, contractDate, 2, 1, 2, 1, 1, packingSize, 0, 0);

        result.Should().Be(55);
    }
}
