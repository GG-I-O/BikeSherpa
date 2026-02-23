using AwesomeAssertions;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Services.Repositories;

namespace Backend.Domain.Tests.DeliveryAggregate.Services.Repositories;

public class DeliveryZoneRepositoryTests
{
    private static readonly City Grenoble = new(1, "Grenoble");
    private static readonly City Meylan = new(2, "Meylan");
    private static readonly City Echirolles = new(3, "Echirolles");
    private static readonly DeliveryZone ZoneGrenoble = new(1, "Grenoble", [Grenoble]);
    private static readonly DeliveryZone ZoneLimitrophe = new(2, "Limitrophe", [Echirolles, Meylan]);
    private static readonly DeliveryZone ZoneExterieur = new(4, "Extérieur", []);
    private static DeliveryZoneRepository MakeSut(IEnumerable<DeliveryZone>? zones = null) =>
        new(zones ?? [ZoneGrenoble, ZoneLimitrophe, ZoneExterieur]);

    [Fact]
    public void Constructor_StoresAllZones()
    {
        var sut = MakeSut();

        sut.DeliveryZones.Should().HaveCount(3);
    }

    [Theory]
    [InlineData("Grenoble", "Grenoble")]
    [InlineData("grenoble", "Grenoble")]
    [InlineData("Echirolles", "Limitrophe")]
    [InlineData("EchiRolles", "Limitrophe")]
    [InlineData("Meylan", "Limitrophe")]
    [InlineData("MEYLAN", "Limitrophe")]
    public void FromAddress_WhenCityIsKnown_ReturnsItsZone_AndIgnoresCase(string city, string expectedZone)
    {
        var sut = MakeSut();

        var result = sut.FromAddress(city);

        result.Name.Should().Be(expectedZone);
    }

    [Fact]
    public void FromAddress_WhenCityNotFound_ReturnsExterieurZone()
    {
        var sut = MakeSut();

        var result = sut.FromAddress("Paris");

        result.Name.Should().Be("Extérieur");
    }

    [Fact]
    public void FromAddress_WhenCityIsEmpty_Throws()
    {
        var sut = MakeSut();

        var act = () => sut.FromAddress("");

        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void FromAddress_WhenNoMatchAndNoExterieurZone_Throws()
    {
        var sut = MakeSut([ZoneGrenoble, ZoneLimitrophe]);

        var act = () => sut.FromAddress("Paris");

        act.Should().Throw<InvalidOperationException>();
    }
}
