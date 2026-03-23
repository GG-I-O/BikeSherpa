using Ardalis.Specification;
using AutoFixture;
using AutoFixture.AutoMoq;
using AwesomeAssertions;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Specification;

namespace Backend.Domain.Tests.DeliveryAggregate.Specifications;

public class DeliveryByUpdatedAtSpecificationTests
{
     private readonly static IFixture Fixture = new Fixture().Customize(new AutoMoqCustomization());
     private readonly static List<Delivery> Delivery = Fixture.CreateMany<Delivery>(1).ToList();
     private readonly static List<Delivery> Deliveries = Fixture.CreateMany<Delivery>(10).ToList();
     private readonly static DateTimeOffset UpdateDate = Fixture.Create<DateTimeOffset>();
     
     [Fact]
     public void WhenUpdatedAtEqualsUpdateDate_ReturnsDelivery()
     {
          // Arrange
          Delivery[0].UpdatedAt = UpdateDate;
          var spec = new DeliveryByUpdatedAtSpecification(UpdateDate);

          // Act
          var result = InMemorySpecificationEvaluator.Default.Evaluate(Delivery, spec).SingleOrDefault();

          // Assert
          result.Should().NotBeNull();
          result.UpdatedAt.Should().Be(UpdateDate);
     }
     
     [Fact]
     public void WhenUpdatedAtIsAfterUpdateDate_ReturnsDelivery()
     {
          // Arrange
          Delivery[0].UpdatedAt = UpdateDate.AddDays(1);
          var spec = new DeliveryByUpdatedAtSpecification(UpdateDate);

          // Act
          var result = InMemorySpecificationEvaluator.Default.Evaluate(Delivery, spec).SingleOrDefault();

          // Assert
          result.Should().NotBeNull();
          result.UpdatedAt.Should().Be(UpdateDate.AddDays(1));
     }

     [Fact]
     public void WhenUpdatedAtIsBeforeUpdateDate_ReturnsNoDelivery()
     {
          // Arrange
          Delivery[0].UpdatedAt = UpdateDate.Subtract(new TimeSpan(1, 0, 0));
          var spec = new DeliveryByUpdatedAtSpecification(UpdateDate);

          // Act
          var result = InMemorySpecificationEvaluator.Default.Evaluate(Delivery, spec).SingleOrDefault();

          // Assert
          result.Should().BeNull();
     }
     
     [Fact]
     public void WhenUpdatedAtIsAfterUpdateDateForSeveralDeliveries_ReturnsMultipleDeliveries()
     {
          // Arrange
          Deliveries[0].UpdatedAt = UpdateDate.Subtract(new TimeSpan(1, 10, 18));
          Deliveries[1].UpdatedAt = UpdateDate.Subtract(new TimeSpan(2, 25, 22));
          Deliveries[2].UpdatedAt = UpdateDate.Subtract(new TimeSpan(3, 37, 23));
          Deliveries[3].UpdatedAt = UpdateDate.Subtract(new TimeSpan(4, 46, 41));
          Deliveries[4].UpdatedAt = UpdateDate.Subtract(new TimeSpan(5, 57, 15));
          Deliveries[5].UpdatedAt = UpdateDate;
          Deliveries[6].UpdatedAt = UpdateDate.Add(new TimeSpan(1, 9, 0));
          Deliveries[7].UpdatedAt = UpdateDate.Add(new TimeSpan(2,2,2));
          Deliveries[8].UpdatedAt = UpdateDate.Add(new TimeSpan(3, 7, 3));
          Deliveries[9].UpdatedAt = UpdateDate.Add(new TimeSpan(4, 10, 4));
          var spec = new DeliveryByUpdatedAtSpecification(UpdateDate);

          // Act
          var result = InMemorySpecificationEvaluator.Default.Evaluate(Deliveries, spec).ToList();

          // Assert
          result.Should().NotBeNull();
          result.Should().HaveCount(5);
     }
}
