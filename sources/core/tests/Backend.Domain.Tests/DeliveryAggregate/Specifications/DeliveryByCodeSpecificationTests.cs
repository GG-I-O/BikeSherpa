using Ardalis.Specification;
using AutoFixture;
using AutoFixture.AutoMoq;
using AwesomeAssertions;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Specification;

namespace Backend.Domain.Tests.DeliveryAggregate.Specifications;

public class DeliveryByCodeSpecificationTests
{
    private readonly static IFixture Fixture = new Fixture().Customize(new AutoMoqCustomization());
    private readonly static List<Delivery> Delivery = Fixture.CreateMany<Delivery>(1).ToList();
    private readonly static List<Delivery> Deliveries = Fixture.CreateMany<Delivery>(10).ToList();

    [Fact]
    public void WhenCodeMatches_ReturnsSingleDelivery()
    {
        // Arrange
        var spec = new DeliveryByCodeSpecification(Delivery.First().Code);

        // Act
        var result = InMemorySpecificationEvaluator.Default.Evaluate(Delivery, spec).SingleOrDefault();

        // Assert
        result.Should().NotBeNull();
        result!.Code.Should().Be(Delivery.First().Code);
    }

    [Fact]
    public void WhenCodeDoesNotMatch_ReturnsNoDelivery()
    {
        // Arrange
        var spec = new DeliveryByCodeSpecification(Fixture.Create<Guid>().ToString());

        // Act
        var result = InMemorySpecificationEvaluator.Default.Evaluate(Deliveries, spec).SingleOrDefault();

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public void WhenMultipleDeliveries_OnlyReturnsTheOneMatchingCode()
    {
        // Arrange
        var spec = new DeliveryByCodeSpecification(Deliveries.First().Code);

        // Act
        var results = InMemorySpecificationEvaluator.Default.Evaluate(Deliveries, spec).ToList();

        // Assert
        results.Should().HaveCount(1);
        results[0].Code.Should().Be(Deliveries.First().Code);
    }
}
