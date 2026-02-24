using AwesomeAssertions;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Services.Repositories;

namespace Backend.Domain.Tests.DeliveryAggregate.Services.Repositories;

public class UrgencyRepositoryTests
{
    private readonly static Urgency Normal = new(1, "Normal", PriceCoefficient: 0.5);
    private readonly static Urgency Express = new(2, "Express", PriceCoefficient: 1.5);

    private static UrgencyRepository MakeSut(IEnumerable<Urgency>? urgencies = null) =>
        new(urgencies ?? [Normal, Express]);

    [Fact]
    public void Constructor_StoresAllUrgencies()
    {
        var sut = MakeSut();

        sut.Urgencies.Should().HaveCount(2);
    }

    [Fact]
    public void Constructor_CopiesAllProperties()
    {
        var sut = MakeSut([Normal]);

        sut.Urgencies.Single().Should().Be(Normal);
    }

    [Fact]
    public void GetUrgency_WhenNameMatches_ReturnsCorrectUrgency()
    {
        var sut = MakeSut();

        var result = sut.GetUrgency("Normal");

        result.Should().Be(Normal);
    }

    [Theory]
    [InlineData("normal")]
    [InlineData("NORMAL")]
    [InlineData("Normal")]
    [InlineData("nORMAL")]
    public void GetUrgency_IsCaseInsensitive(string name)
    {
        var sut = MakeSut();

        var result = sut.GetUrgency(name);

        result.Should().Be(Normal);
    }

    [Fact]
    public void GetUrgency_WhenNameIsEmpty_ThrowsArgumentException()
    {
        var sut = MakeSut();

        var act = () => sut.GetUrgency("");

        act.Should().Throw<ArgumentException>()
            .WithMessage("Veuillez indiquer une urgence.");
    }

    [Fact]
    public void GetUrgency_WhenNameIsUnknown_ThrowsArgumentException()
    {
        var sut = MakeSut();

        var act = () => sut.GetUrgency("Unknown");

        act.Should().Throw<ArgumentException>()
            .WithMessage("Urgence inconnue.");
    }

    [Fact]
    public void GetUrgency_WhenMultipleUrgenciesShareSameName_Throws()
    {
        var duplicate = Normal with { Id = 99 };
        var sut = MakeSut([Normal, duplicate, Express]);

        var act = () => sut.GetUrgency("Normal");

        act.Should().Throw<InvalidOperationException>();
    }

}
