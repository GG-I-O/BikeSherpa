using Ardalis.Specification;
using AutoFixture;
using AutoFixture.AutoMoq;
using AwesomeAssertions;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Specification;

namespace Backend.Domain.Tests.DeliveryAggregate.Specifications;

public class DeliveryByIdSpecificationTests
{
     private readonly static IFixture Fixture = new Fixture().Customize(new AutoMoqCustomization());
     private readonly static List<Delivery> Delivery = Fixture.CreateMany<Delivery>(1).ToList();
     private readonly static List<Delivery> Deliveries = Fixture.CreateMany<Delivery>(10).ToList();

     [Fact]
     public void WhenIdMatches_ReturnsSingleDelivery()
     {
          // Arrange
          var spec = new DeliveryByIdSpecification(Delivery.First().Id);

          // Act
          var result = InMemorySpecificationEvaluator.Default.Evaluate(Delivery, spec).SingleOrDefault();

          // Assert
          result.Should().NotBeNull();
          result!.Id.Should().Be(Delivery.First().Id);
     }

     [Fact]
     public void WhenIdDoesNotMatch_ReturnsNoDelivery()
     {
          // Arrange
          var spec = new DeliveryByIdSpecification(Fixture.Create<Guid>());

          // Act
          var result = InMemorySpecificationEvaluator.Default.Evaluate(Deliveries, spec).SingleOrDefault();

          // Assert
          result.Should().BeNull();
     }

     [Fact]
     public void WhenMultipleDeliveries_OnlyReturnsTheOneMatchingId()
     {
          // Arrange
          var spec = new DeliveryByIdSpecification(Deliveries.First().Id);

          // Act
          var results = InMemorySpecificationEvaluator.Default.Evaluate(Deliveries, spec).ToList();

          // Assert
          results.Should().HaveCount(1);
          results[0].Id.Should().Be(Deliveries.First().Id);
     }
}
