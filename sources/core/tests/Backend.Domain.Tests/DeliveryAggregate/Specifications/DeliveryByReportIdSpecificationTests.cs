using System;
using System.Collections.Generic;
using System.Linq;
using Ardalis.Specification;
using AutoFixture;
using AutoFixture.AutoMoq;
using AwesomeAssertions;
using Ggio.BikeSherpa.Backend.Domain.CustomerAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Specification;
using Xunit;

namespace Backend.Domain.Tests.DeliveryAggregate.Specifications;

public class DeliveryByReportIdSpecificationTests
{
     private readonly static IFixture Fixture = new Fixture().Customize(new AutoMoqCustomization());
     private readonly static List<Delivery> Delivery = Fixture.CreateMany<Delivery>(1).ToList();
     private readonly static List<Customer> Customer = Fixture.CreateMany<Customer>(1).ToList();
     private readonly static List<Delivery> Deliveries = Fixture.CreateMany<Delivery>(10).ToList();
     private readonly static List<Customer> Customers = Fixture.CreateMany<Customer>(10).ToList();

     [Fact]
     public void WhenReportIdMatches_ReturnsSingleDelivery()
     {
          // Arrange
          Delivery[0].GenerateReportId(Customer[0]);
          var spec = new DeliveryByReportIdSpecification(Delivery.First().ReportId!);

          // Act
          var result = InMemorySpecificationEvaluator.Default.Evaluate(Delivery, spec).SingleOrDefault();

          // Assert
          result.Should().NotBeNull();
          result.ReportId.Should().Be(Delivery.First().ReportId);
     }

     [Fact]
     public void WhenReportIdDoesNotMatch_ReturnsNoDelivery()
     {
          // Arrange
          var spec = new DeliveryByReportIdSpecification(Fixture.Create<Guid>().ToString());

          // Act
          var result = InMemorySpecificationEvaluator.Default.Evaluate(Deliveries, spec).SingleOrDefault();

          // Assert
          result.Should().BeNull();
     }

     [Fact]
     public void WhenMultipleDeliveries_OnlyReturnsTheOneMatchingReportId()
     {
          // Arrange
          for (int i = 0; i < Deliveries.Count; i++)
          {
               Deliveries[i].GenerateReportId(Customers[i]);
          }
          var spec = new DeliveryByReportIdSpecification(Deliveries.First().ReportId!);

          // Act
          var results = InMemorySpecificationEvaluator.Default.Evaluate(Deliveries, spec).ToList();

          // Assert
          results.Should().HaveCount(1);
          results[0].ReportId.Should().Be(Deliveries.First().ReportId);
     }
}
