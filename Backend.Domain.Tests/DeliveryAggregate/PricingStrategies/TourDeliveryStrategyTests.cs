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
        // Arrange
        var sut = MakeSut();
        
        var startDate = new DateTimeOffset(2026, 1, 14, 10, 0, 0, TimeSpan.Zero);
        var contractDate = new DateTimeOffset(2026, 1, 14, 20, 0, 0, TimeSpan.Zero);

        // Act
        var result = sut.CalculateDeliveryPriceWithoutVat(startDate, contractDate, pickupNumber, 0, 0, 0, 0, DefaultPackingSize, 0, 0);

        // Assert
        result.Should().Be(2 + expectedPickupCost);
    }

    [Fact]
    public void CalculatePrice_WhenDeliveryIsOnSameDayAsContract_AddsExtraCost()
    {
        // Arrange
        var sut = MakeSut();
        var contractDate = BaseDate.AddHours(5); // same day, 5h delay → standard

        // Act
        var result = sut.CalculateDeliveryPriceWithoutVat(BaseDate, contractDate, 0, 0, 0, 0, 0, DefaultPackingSize, 0, 0);

        // Assert
        result.Should().Be(2);
    }

    [Fact]
    public void CalculatePrice_WhenDeliveryIsOnDifferentDay_NoSameDayCost()
    {
        // Arrange
        var sut = MakeSut();
        var contractDate = BaseDate.AddHours(25);

        // Act
        var result = sut.CalculateDeliveryPriceWithoutVat(BaseDate, contractDate, 0, 0, 0, 0, 0, DefaultPackingSize, 0, 0);

        // Assert
        result.Should().Be(-2);
    }

    [Fact]
    public void CalculatePrice_WhenDelayIsMoreThan18Hours_AppliesEarlyOrderDiscount()
    {
        // Arrange
        var sut = MakeSut();
        var startDate = new DateTimeOffset(2026, 1, 14, 10, 0, 0, TimeSpan.Zero);
        var contractDate = new DateTimeOffset(2026, 1, 15, 12, 0, 0, TimeSpan.Zero); // 26h, different day

        // Act
        var result = sut.CalculateDeliveryPriceWithoutVat(startDate, contractDate, 0, 0, 0, 0, 0, DefaultPackingSize, 0, 0);

        // Assert
        result.Should().Be(-2);
    }

    [Fact]
    public void CalculatePrice_WhenDelayIsAtMost2Hours_AppliesLastMinuteExtraCost()
    {
        // Arrange
        var sut = MakeSut();
        var contractDate = BaseDate.AddHours(1);

        // Act
        var result = sut.CalculateDeliveryPriceWithoutVat(BaseDate, contractDate, 0, 0, 0, 0, 0, DefaultPackingSize, 0, 0);

        // Assert
        result.Should().Be(2 + 3);
    }

    [Fact]
    public void CalculatePrice_WhenDelayIsExactly2Hours_AppliesLastMinuteExtraCost()
    {
        // Arrange
        var sut = MakeSut();
        var contractDate = BaseDate.AddHours(2);

        // Act
        var result = sut.CalculateDeliveryPriceWithoutVat(BaseDate, contractDate, 0, 0, 0, 0, 0, DefaultPackingSize, 0, 0);

        // Assert
        result.Should().Be(2 + 3);
    }

    [Fact]
    public void CalculatePrice_WhenDelayIsBetween2And18Hours_AppliesNoCostAdjustment()
    {
        // Arrange
        var sut = MakeSut();
        var contractDate = BaseDate.AddHours(10);

        // Act
        var result = sut.CalculateDeliveryPriceWithoutVat(BaseDate, contractDate, 0, 0, 0, 0, 0, DefaultPackingSize, 0, 0);

        // Assert
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
        // Arrange
        var sut = MakeSut();
        var startDate = new DateTimeOffset(2026, 1, 14, 10, 0, 0, TimeSpan.Zero);
        var contractDate = new DateTimeOffset(2026, 1, 14, 20, 0, 0, TimeSpan.Zero);

        // Act
        var result = sut.CalculateDeliveryPriceWithoutVat(startDate, contractDate, 0, grenoble, border, periphery, outside, DefaultPackingSize, 0, 0);

        // Assert
        result.Should().Be(2 + expectedZoneCost);
    }

    [Fact]
    public void CalculatePrice_IncludesPackingSizeTourPrice()
    {
        // Arrange
        var sut = MakeSut();
        var packingSize = DefaultPackingSize with { TourPrice = 7.0 };
        var startDate = new DateTimeOffset(2026, 1, 14, 10, 0, 0, TimeSpan.Zero);
        var contractDate = new DateTimeOffset(2026, 1, 14, 20, 0, 0, TimeSpan.Zero);

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
        var startDate = new DateTimeOffset(2026, 1, 14, 10, 0, 0, TimeSpan.Zero);
        var contractDate = new DateTimeOffset(2026, 1, 14, 20, 0, 0, TimeSpan.Zero); // 10h, same day

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
        var startDate = new DateTimeOffset(2026, 1, 14, 10, 0, 0, TimeSpan.Zero);
        var contractDate = new DateTimeOffset(2026, 1, 14, 20, 0, 0, TimeSpan.Zero);

        // Act
        var result = sut.CalculateDeliveryPriceWithoutVat(startDate, contractDate, 2, 1, 2, 1, 1, packingSize, 0, 0);

        // Assert
        result.Should().Be(55);
    }
}
