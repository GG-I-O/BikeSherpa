using AwesomeAssertions;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.PricingStrategies;

namespace Backend.Domain.Tests.DeliveryAggregate.PricingStrategies;

public class SimpleDeliveryStrategyTests
{
    private readonly static PackingSize DefaultPackingSize = new(1, "Standard", 10, 50, 0, 0);
    private readonly static DateTimeOffset BaseDate = new(2026, 1, 15, 10, 0, 0, TimeSpan.Zero);
    private static SimpleDeliveryStrategy MakeSut() => new();

    [Fact]
    public void Name_ShouldReturn_SimpleDelivery()
    {
        var sut = MakeSut();

        sut.Name.Should().Be("SimpleDelivery");
    }

    [Fact]
    public void CalculatePrice_WhenDeliveryIsOnSameDayAsContractDate_AddsExtraCost()
    {
        var sut = MakeSut();
        var contractDate = BaseDate;
        var startDate = BaseDate.AddHours(5);

        var result = sut.CalculateDeliveryPriceWithoutVat(startDate, contractDate, 1, 0, 0, 0, 0, DefaultPackingSize, 0, 0);

        result.Should().Be(2);
    }

    [Fact]
    public void CalculatePrice_WhenOrderIsAfter5PM_AndDeliveryIsNextDay_AddsSameDayCost()
    {
        var sut = MakeSut();
        var contractDate = new DateTimeOffset(2026, 1, 15, 18, 0, 0, TimeSpan.Zero);
        var startDate = new DateTimeOffset(2026, 1, 16, 10, 0, 0, TimeSpan.Zero);

        var result = sut.CalculateDeliveryPriceWithoutVat(startDate, contractDate, 1, 0, 0, 0, 0, DefaultPackingSize, 0, 0);

        result.Should().Be(2);
    }

    [Fact]
    public void CalculatePrice_WhenDeliveryIsOnDifferentDay_AppliesEarlyOrderDiscountAndNoSameDayCost()
    {
        var sut = MakeSut();
        var contractDate = BaseDate;
        var startDate = BaseDate.AddHours(25);

        var result = sut.CalculateDeliveryPriceWithoutVat(startDate, contractDate, 1, 0, 0, 0, 0, DefaultPackingSize, 0, 0);

        result.Should().Be(-2);
    }

    [Fact]
    public void CalculatePrice_WhenDelayIsAtMost2Hours_AppliesLastMinuteExtraCostAndSameDayCost()
    {
        var sut = MakeSut();
        var contractDate = BaseDate;
        var startDate = BaseDate.AddHours(1);

        var result = sut.CalculateDeliveryPriceWithoutVat(startDate, contractDate, 1, 0, 0, 0, 0, DefaultPackingSize, 0, 0);

        result.Should().Be(2 + 3);
    }

    [Fact]
    public void CalculatePrice_WhenDelayIsExactly2Hours_AppliesLastMinuteExtraCostAndSameDayCost()
    {
        var sut = MakeSut();
        var contractDate = BaseDate;
        var startDate = BaseDate.AddHours(2);

        var result = sut.CalculateDeliveryPriceWithoutVat(startDate, contractDate, 1, 0, 0, 0, 0, DefaultPackingSize, 0, 0);

        result.Should().Be(2 + 3);
    }

    [Fact]
    public void CalculatePrice_WhenDelayIsBetween2And18Hours_AppliesOnlySameDayCost()
    {
        var sut = MakeSut();
        var contractDate = BaseDate;
        var startDate = BaseDate.AddHours(6);

        var result = sut.CalculateDeliveryPriceWithoutVat(startDate, contractDate, 1, 0, 0, 0, 0, DefaultPackingSize, 0, 0);

        result.Should().Be(2);
    }

    [Theory]
    [InlineData(3, 0, 0, 0, 3 * 1.0)]
    [InlineData(0, 3, 0, 0, 3 * 2.5)]
    [InlineData(0, 0, 3, 0, 3 * 5.5)]
    [InlineData(0, 0, 0, 3, 3 * 11.0)]
    public void CalculatePrice_ZoneDropoffSteps_AreMultipliedByCorrectStepPrice(
        int grenoble, int border, int periphery, int outside, double expectedZoneCost)
    {
        var sut = MakeSut();
        var contractDate = new DateTimeOffset(2026, 1, 14, 10, 0, 0, TimeSpan.Zero);
        var startDate = new DateTimeOffset(2026, 1, 14, 16, 0, 0, TimeSpan.Zero);

        var result = sut.CalculateDeliveryPriceWithoutVat(startDate, contractDate, 1, grenoble, border, periphery, outside, DefaultPackingSize, 0, 0);

        result.Should().Be(2 + expectedZoneCost);
    }

    [Fact]
    public void CalculatePrice_IncludesPackingSizePrice()
    {
        var sut = MakeSut();
        var packingSize = DefaultPackingSize with { Price = 5.0 };
        var contractDate = new DateTimeOffset(2026, 1, 14, 10, 0, 0, TimeSpan.Zero);
        var startDate = new DateTimeOffset(2026, 1, 14, 16, 0, 0, TimeSpan.Zero);

        var result = sut.CalculateDeliveryPriceWithoutVat(startDate, contractDate, 1, 0, 0, 0, 0, packingSize, 0, 0);

        result.Should().Be(2 + 5.0);
    }

    [Fact]
    public void CalculatePrice_IncludesUrgencyCoefficientTimesDistance()
    {
        var sut = MakeSut();
        var contractDate = new DateTimeOffset(2026, 1, 14, 10, 0, 0, TimeSpan.Zero);
        var startDate = new DateTimeOffset(2026, 1, 14, 16, 0, 0, TimeSpan.Zero);

        var result = sut.CalculateDeliveryPriceWithoutVat(startDate, contractDate, 1, 0, 0, 0, 0, DefaultPackingSize, 1.5, 10.0);

        result.Should().Be(2 + 1.5 * 10.0);
    }

    [Fact]
    public void CalculatePrice_WithAllFactors_ReturnsCorrectTotal()
    {
        var sut = MakeSut();
        var packingSize = DefaultPackingSize with { Price = 3.0 };
        var contractDate = new DateTimeOffset(2026, 1, 14, 10, 0, 0, TimeSpan.Zero);
        var startDate = new DateTimeOffset(2026, 1, 14, 20, 0, 0, TimeSpan.Zero);

        var result = sut.CalculateDeliveryPriceWithoutVat(startDate, contractDate, 1, 1, 2, 1, 1, packingSize, 2.0, 5.0);

        result.Should().Be(2 + 1 * 1 + 2 * 2.5 + 1 * 5.5 + 1 * 11 + 3 + 2 * 5);
    }
}
