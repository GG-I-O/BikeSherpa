using System;
using AwesomeAssertions;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.PricingStrategies;
using Xunit;

namespace Backend.Domain.Tests.DeliveryAggregate.PricingStrategies;

public class SimpleDeliveryStrategyTests
{
    private readonly static PackingSize DefaultPackingSize = new("Standard", 10, 50, 0, 0);
    private readonly static DateTimeOffset BaseDate = new(2026, 1, 15, 10, 0, 0, TimeSpan.Zero);
    private static SimpleDeliveryStrategy MakeSut() => new();

    [Fact]
    public void Name_ShouldReturn_SimpleDelivery()
    {
        // Arrange & Act
        var sut = MakeSut();

        // Assert
        sut.Name.Should().Be("SimpleDelivery");
    }

    [Fact]
    public void CalculatePrice_WhenDeliveryIsOnSameDayAsContractDate_AddsExtraCost()
    {
        // Arrange
        var sut = MakeSut();
        var contractDate = BaseDate;
        var startDate = BaseDate.AddHours(5);

        // Act
        var result = sut.CalculateDeliveryPriceWithoutVat(startDate, contractDate, 1, 0, 0, 0, 0, DefaultPackingSize, 0, 0);

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
        var result = sut.CalculateDeliveryPriceWithoutVat(startDate, contractDate, 1, 0, 0, 0, 0, DefaultPackingSize, 0, 0);

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
        var result = sut.CalculateDeliveryPriceWithoutVat(startDate, contractDate, 1, 0, 0, 0, 0, DefaultPackingSize, 0, 0);

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
        var result = sut.CalculateDeliveryPriceWithoutVat(startDate, contractDate, 1, 0, 0, 0, 0, DefaultPackingSize, 0, 0);

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
        var result = sut.CalculateDeliveryPriceWithoutVat(startDate, contractDate, 1, 0, 0, 0, 0, DefaultPackingSize, 0, 0);

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
        var result = sut.CalculateDeliveryPriceWithoutVat(startDate, contractDate, 1, 0, 0, 0, 0, DefaultPackingSize, 0, 0);

        // Assert
        result.Should().Be(2);
    }

    [Theory]
    [InlineData(3, 0, 0, 0, 3 * 1.0)]
    [InlineData(0, 3, 0, 0, 3 * 2.5)]
    [InlineData(0, 0, 3, 0, 3 * 5.5)]
    [InlineData(0, 0, 0, 3, 3 * 11.0)]
    public void CalculatePrice_ZoneDropoffSteps_AreMultipliedByCorrectStepPrice(
        int core, int border, int periphery, int outside, double expectedZoneCost)
    {
        // Arrange
        var sut = MakeSut();
        var contractDate = new DateTimeOffset(2026, 1, 14, 10, 0, 0, TimeSpan.Zero);
        var startDate = new DateTimeOffset(2026, 1, 14, 16, 0, 0, TimeSpan.Zero);

        // Act
        var result = sut.CalculateDeliveryPriceWithoutVat(startDate, contractDate, 1, core, border, periphery, outside, DefaultPackingSize, 0, 0);

        // Aassert
        result.Should().Be(2 + expectedZoneCost);
    }

    [Fact]
    public void CalculatePrice_IncludesPackingSizePrice()
    {
        // Arrange
        var sut = MakeSut();
        var packingSize = DefaultPackingSize with { Price = 5.0 };
        var contractDate = new DateTimeOffset(2026, 1, 14, 10, 0, 0, TimeSpan.Zero);
        var startDate = new DateTimeOffset(2026, 1, 14, 16, 0, 0, TimeSpan.Zero);

        // Act
        var result = sut.CalculateDeliveryPriceWithoutVat(startDate, contractDate, 1, 0, 0, 0, 0, packingSize, 0, 0);

        // Assert
        result.Should().Be(2 + 5.0);
    }

    [Fact]
    public void CalculatePrice_IncludesUrgencyCoefficientTimesDistance()
    {
        // Arrange
        var sut = MakeSut();
        var contractDate = new DateTimeOffset(2026, 1, 14, 10, 0, 0, TimeSpan.Zero);
        var startDate = new DateTimeOffset(2026, 1, 14, 16, 0, 0, TimeSpan.Zero);

        // Act
        var result = sut.CalculateDeliveryPriceWithoutVat(startDate, contractDate, 1, 0, 0, 0, 0, DefaultPackingSize, 1.5, 10.0);

        // Assert
        result.Should().Be(2 + 1.5 * 10.0);
    }

    [Fact]
    public void CalculatePrice_WithAllFactors_ReturnsCorrectTotal()
    {
        // Arrange
        var sut = MakeSut();
        var packingSize = DefaultPackingSize with { Price = 3.0 };
        var contractDate = new DateTimeOffset(2026, 1, 14, 10, 0, 0, TimeSpan.Zero);
        var startDate = new DateTimeOffset(2026, 1, 14, 20, 0, 0, TimeSpan.Zero);

        // Act
        var result = sut.CalculateDeliveryPriceWithoutVat(startDate, contractDate, 1, 1, 2, 1, 1, packingSize, 2.0, 5.0);

        // Assert
        result.Should().Be(2 + 1 * 1 + 2 * 2.5 + 1 * 5.5 + 1 * 11 + 3 + 2 * 5, "Coût PEC le jour même +\nnb dépôts dans la zone centrale x coût de la zone +\nnb dépôts en bordure x coût de la zone +\nnb dépôts périphérie x coût de la zone +\nnb dépôts en extérieur x coût de la zone +\ncoût du colisage +\ndistance kilométrique x coefficient d'urgence");
    }
}
