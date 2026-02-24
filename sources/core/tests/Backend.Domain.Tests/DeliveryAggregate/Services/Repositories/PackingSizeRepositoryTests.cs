using AwesomeAssertions;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Services.Repositories;

namespace Backend.Domain.Tests.DeliveryAggregate.Services.Repositories;

public class PackingSizeRepositoryTests
{
    private readonly static PackingSize Small = new(1, "Small", 5, 30, 2, 1);
    private readonly static PackingSize Large = new(2, "Large", 20, 80, 6, 4);

    private static PackingSizeRepository MakeSut(IEnumerable<PackingSize>? sizes = null) =>
        new(sizes ?? [Small, Large]);

    [Fact]
    public void Constructor_StoresAllPackingSizes()
    {
        var sut = MakeSut();

        sut.PackingSizes.Should().HaveCount(2);
    }

    [Fact]
    public void Constructor_CopiesAllProperties()
    {
        var sut = MakeSut([Small]);

        var stored = sut.PackingSizes.Single();
        stored.Should().Be(Small);
    }

    [Fact]
    public void FromName_WhenNameMatches_ReturnsCorrectPackingSize()
    {
        var sut = MakeSut();

        var result = sut.FromName("Small");

        result.Should().Be(Small);
    }

    [Theory]
    [InlineData("small")]
    [InlineData("SMALL")]
    [InlineData("Small")]
    [InlineData("sMALL")]
    public void FromName_IsCaseInsensitive(string name)
    {
        var sut = MakeSut();

        var result = sut.FromName(name);

        result.Should().Be(Small);
    }

    [Fact]
    public void FromName_WhenNameIsEmpty_ThrowsArgumentException()
    {
        var sut = MakeSut();

        var act = () => sut.FromName("");

        act.Should().Throw<ArgumentException>()
            .WithMessage("Veuillez indiquer une taille de colis.");
    }

    [Fact]
    public void FromName_WhenNameIsUnknown_ThrowsArgumentException()
    {
        var sut = MakeSut();

        var act = () => sut.FromName("Unknown");

        act.Should().Throw<ArgumentException>()
            .WithMessage("Taille de colis inconnue.");
    }

    [Fact]
    public void FromName_WhenMultipleSizesShareSameName_Throws()
    {
        var duplicate = Small with { Id = 99 };
        var sut = MakeSut([Small, duplicate, Large]);

        var act = () => sut.FromName("Small");

        act.Should().Throw<InvalidOperationException>();
    }
}
