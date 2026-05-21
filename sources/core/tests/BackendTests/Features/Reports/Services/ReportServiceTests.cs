using AutoFixture;
using AwesomeAssertions;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.PricingStrategies;
using Ggio.BikeSherpa.Backend.Domain.SharedKernel;
using Ggio.BikeSherpa.Backend.Features.Reports.Services;
using Moq;
using PricingStrategyEnum = Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations.PricingStrategy;

namespace BackendTests.Features.Reports.Services;

public class ReportServiceTests
{
     private readonly Fixture _fixture = new();
     private readonly Mock<IPricingStrategy> _pricingStrategyMock = new();

     private readonly static Urgency Urgency = new("Standard", 1, "Standard", 1, null, null);
     private readonly static PackingSize PackingSize = new("Small", 1, "Small", 3, 10);
     private readonly static DeliveryZone CoreZone = new(StepZone.InCore);
     private readonly static DeliveryZone BorderZone = new(StepZone.InBorder);
     private readonly static DeliveryZone PeripheryZone = new(StepZone.InPeriphery);
     private readonly static DeliveryZone OutsideZone = new(StepZone.Outside);

     private readonly DateTimeOffset _startDate = new(2026, 1, 15, 10, 0, 0, TimeSpan.Zero);
     private readonly DateTimeOffset _contractDate = new(2026, 1, 14, 10, 0, 0, TimeSpan.Zero);
     private readonly ReportService _sut;

     public ReportServiceTests()
     {
          _pricingStrategyMock
               .Setup(s => s.ImplementedStrategy)
               .Returns(PricingStrategyEnum.SimpleDeliveryStrategy);

          _sut = new ReportService([_pricingStrategyMock.Object]);
     }

     private Delivery MakeDelivery(
          PricingStrategyEnum pricingStrategy = PricingStrategyEnum.SimpleDeliveryStrategy,
          string? code = null,
          DateTimeOffset? startDate = null,
          DateTimeOffset? contractDate = null,
          double? totalPrice = null,
          double? discount = null,
          double? extraCost = null,
          double? distance = null,
          List<DeliveryStep>? steps = null,
          PackingSize? packingSize = null,
          Urgency? urgency = null)
     {
          return new Delivery
          {
               PricingStrategy = pricingStrategy,
               Code = code ?? _fixture.Create<string>(),
               CustomerId = _fixture.Create<Guid>(),
               Urgency = urgency ?? Urgency,
               PackingSize = packingSize ?? PackingSize,
               InsulatedBox = false,
               ContractDate = contractDate ?? _contractDate,
               StartDate = startDate ?? _startDate,
               Steps = steps ?? [],
               TotalPrice = totalPrice,
               Discount = discount,
               ExtraCost = extraCost,
               Distance = distance
          };
     }

     private DeliveryStep MakeStep(
          Delivery parentDelivery,
          StepType stepType,
          DeliveryZone zone,
          double distance = 0,
          bool notBilled = false)
     {
          var address = _fixture.Create<Address>();

          return new DeliveryStep(stepType, 1, address)
          {
               Id = Guid.NewGuid(),
               StepAddress = address,
               StepZone = zone,
               Distance = distance,
               ParentDelivery = parentDelivery,
               NotBilled = notBilled
          };
     }

     private static ReportService MakeSutWith(params IPricingStrategy[] strategies)
     {
          return new ReportService(strategies);
     }

     [Fact]
     public void GenerateReport_MapsReportHeader()
     {
          // Arrange
          var customerName = _fixture.Create<string>();
          var from = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);
          var to = new DateTimeOffset(2026, 1, 31, 0, 0, 0, TimeSpan.Zero);

          // Act
          var report = _sut.GenerateReport(customerName, from, to, []);

          // Assert
          report.CustomerName.Should().Be(customerName);
          report.StartDate.Should().Be(from);
          report.EndDate.Should().Be(to);
          report.TotalPrice.Should().Be(0);
          report.Deliveries.Should().BeEmpty();
     }

     [Fact]
     public void GenerateReport_MapsDeliverySummaryAndTotals()
     {
          // Arrange
          var firstDelivery = MakeDelivery(code: "DEL-001", totalPrice: 12.50);
          var secondDelivery = MakeDelivery(code: "DEL-002", totalPrice: 27.25);

          // Act
          var report = _sut.GenerateReport(
               "Customer",
               _startDate,
               _startDate.AddDays(1),
               [firstDelivery, secondDelivery]);

          // Assert
          report.TotalPrice.Should().Be(39.75);
          report.Deliveries.Should().HaveCount(2);

          report.Deliveries[0].DeliveryCode.Should().Be(firstDelivery.Code);
          report.Deliveries[0].DeliveryDate.Should().Be(firstDelivery.StartDate);
          report.Deliveries[0].DeliveryPrice.Should().Be(12.50);

          report.Deliveries[1].DeliveryCode.Should().Be(secondDelivery.Code);
          report.Deliveries[1].DeliveryDate.Should().Be(secondDelivery.StartDate);
          report.Deliveries[1].DeliveryPrice.Should().Be(27.25);
     }

     [Fact]
     public void GenerateReport_WhenDeliveryTotalPriceIsNull_UsesZero()
     {
          // Arrange
          var delivery = MakeDelivery(totalPrice: null);

          // Act
          var report = _sut.GenerateReport("Customer", _startDate, _startDate.AddDays(1), [delivery]);

          // Assert
          report.TotalPrice.Should().Be(0);
          report.Deliveries.Single().DeliveryPrice.Should().Be(0);
     }

     [Fact]
     public void GenerateReport_AddsSameDayAndDelayDetails_WhenStrategyReturnsNonZeroCosts()
     {
          // Arrange
          _pricingStrategyMock
               .Setup(s => s.SameDayExtraCost(_startDate, _contractDate))
               .Returns(2);

          _pricingStrategyMock
               .Setup(s => s.DelayCost(_startDate, _contractDate))
               .Returns(-2);

          var delivery = MakeDelivery();

          // Act
          var report = _sut.GenerateReport("Customer", _startDate, _startDate.AddDays(1), [delivery]);

          // Assert
          report.Deliveries.Single().Details.Should().ContainEquivalentOf(new
          {
               Description = "Demande le même jour",
               Price = 2.0,
               Quantity = 1
          });

          report.Deliveries.Single().Details.Should().ContainEquivalentOf(new
          {
               Description = "Délai de la demande",
               Price = -2.0,
               Quantity = 1
          });
     }

     [Fact]
     public void GenerateReport_AddsStepDetailsUsingCountsPerTypeAndZone()
     {
          // Arrange
          _pricingStrategyMock.Setup(s => s.PickupCost(2)).Returns(14);
          _pricingStrategyMock.Setup(s => s.DropOffInCoreCost(2)).Returns(10);
          _pricingStrategyMock.Setup(s => s.DropOffInBorderCost(1)).Returns(8);
          _pricingStrategyMock.Setup(s => s.DropOffInPeripheryCost(1)).Returns(5.5);
          _pricingStrategyMock.Setup(s => s.DropOffOutsideCost(2)).Returns(22);

          var delivery = MakeDelivery();
          delivery.Steps =
          [
               MakeStep(delivery, StepType.Pickup, CoreZone),
               MakeStep(delivery, StepType.Pickup, BorderZone),
               MakeStep(delivery, StepType.Dropoff, CoreZone),
               MakeStep(delivery, StepType.Dropoff, CoreZone),
               MakeStep(delivery, StepType.Dropoff, BorderZone),
               MakeStep(delivery, StepType.Dropoff, PeripheryZone),
               MakeStep(delivery, StepType.Dropoff, OutsideZone),
               MakeStep(delivery, StepType.Dropoff, OutsideZone)
          ];

          // Act
          var report = _sut.GenerateReport("Customer", _startDate, _startDate.AddDays(1), [delivery]);
          var details = report.Deliveries.Single().Details;

          // Assert
          details.Should().ContainEquivalentOf(new
          {
               Description = "Prise en charge",
               Price = 14.0,
               Quantity = 2
          });

          details.Should().ContainEquivalentOf(new
          {
               Description = "Livraison en centre-ville",
               Price = 10.0,
               Quantity = 2
          });

          details.Should().ContainEquivalentOf(new
          {
               Description = "Livraison en bordure",
               Price = 8.0,
               Quantity = 1
          });

          details.Should().ContainEquivalentOf(new
          {
               Description = "Livraison en périphérie",
               Price = 5.5,
               Quantity = 1
          });

          details.Should().ContainEquivalentOf(new
          {
               Description = "Livraison en zone externe",
               Price = 22.0,
               Quantity = 2
          });
     }

     [Fact]
     public void GenerateReport_WhenDeliveryDistanceIsProvided_UsesDeliveryDistanceForDistanceCost()
     {
          // Arrange
          const double deliveryDistance = 24.5;

          var delivery = MakeDelivery(distance: deliveryDistance);
          delivery.Steps =
          [
               MakeStep(delivery, StepType.Pickup, CoreZone, distance: 10),
               MakeStep(delivery, StepType.Dropoff, CoreZone, distance: 15)
          ];

          _pricingStrategyMock
               .Setup(s => s.TotalDistanceCost(deliveryDistance, delivery.Urgency))
               .Returns(12.345);

          // Act
          var report = _sut.GenerateReport("Customer", _startDate, _startDate.AddDays(1), [delivery]);

          // Assert
          _pricingStrategyMock.Verify(
               s => s.TotalDistanceCost(deliveryDistance, delivery.Urgency),
               Times.Once);

          report.Deliveries.Single().Details.Should().ContainEquivalentOf(new
          {
               Description = "Cout distance",
               Price = 12.34,
               Quantity = 1
          });
     }

     [Fact]
     public void GenerateReport_WhenDeliveryDistanceIsZero_SumsBillableStepDistances()
     {
          // Arrange
          const double billableDistance = 12.5;
          const double ignoredDistance = 8.5;

          var delivery = MakeDelivery(distance: 0);
          delivery.Steps =
          [
               MakeStep(delivery, StepType.Pickup, CoreZone, distance: billableDistance),
               MakeStep(delivery, StepType.Dropoff, CoreZone, distance: ignoredDistance, notBilled: true)
          ];

          _pricingStrategyMock
               .Setup(s => s.TotalDistanceCost(billableDistance, delivery.Urgency))
               .Returns(6.25);

          // Act
          var report = _sut.GenerateReport("Customer", _startDate, _startDate.AddDays(1), [delivery]);

          // Assert
          _pricingStrategyMock.Verify(
               s => s.TotalDistanceCost(billableDistance, delivery.Urgency),
               Times.Once);

          report.Deliveries.Single().Details.Should().ContainEquivalentOf(new
          {
               Description = "Cout distance",
               Price = 6.25,
               Quantity = 1
          });
     }

     [Fact]
     public void GenerateReport_AddsSimpleDeliveryPackingDetail()
     {
          // Arrange
          var packingSize = new PackingSize("Small", 1, "Small", 3, 10);
          var delivery = MakeDelivery(
               pricingStrategy: PricingStrategyEnum.SimpleDeliveryStrategy,
               packingSize: packingSize);

          // Act
          var report = _sut.GenerateReport("Customer", _startDate, _startDate.AddDays(1), [delivery]);

          // Assert
          report.Deliveries.Single().Details.Should().ContainEquivalentOf(new
          {
               Description = "Colisage",
               packingSize.Price,
               Quantity = 1
          });
     }

     [Fact]
     public void GenerateReport_AddsTourDeliveryPackingDetail()
     {
          // Arrange
          var tourStrategyMock = new Mock<IPricingStrategy>();
          tourStrategyMock
               .Setup(s => s.ImplementedStrategy)
               .Returns(PricingStrategyEnum.TourDeliveryStrategy);

          var sut = MakeSutWith(tourStrategyMock.Object);

          var packingSize = new PackingSize("Large", 2, "Large", 3, 10);
          var delivery = MakeDelivery(
               pricingStrategy: PricingStrategyEnum.TourDeliveryStrategy,
               packingSize: packingSize);

          // Act
          var report = sut.GenerateReport("Customer", _startDate, _startDate.AddDays(1), [delivery]);

          // Assert
          report.Deliveries.Single().Details.Should().ContainEquivalentOf(new
          {
               Description = "Colisage",
               Price = packingSize.TourPrice,
               Quantity = 1
          });
     }

     [Fact]
     public void GenerateReport_AddsDiscountAndExtraCostDetails_WhenGreaterThanZero()
     {
          // Arrange
          var delivery = MakeDelivery(discount: 5, extraCost: 7.5);

          // Act
          var report = _sut.GenerateReport("Customer", _startDate, _startDate.AddDays(1), [delivery]);

          // Assert
          report.Deliveries.Single().Details.Should().ContainEquivalentOf(new
          {
               Description = "Remise",
               Price = -5.0,
               Quantity = 1
          });

          report.Deliveries.Single().Details.Should().ContainEquivalentOf(new
          {
               Description = "Surcout",
               Price = 7.5,
               Quantity = 1
          });
     }

     [Fact]
     public void GenerateReport_WhenNoMatchingStrategy_DoesNotAddStrategyBasedDetails()
     {
          // Arrange
          var tourStrategyMock = new Mock<IPricingStrategy>();
          tourStrategyMock
               .Setup(s => s.ImplementedStrategy)
               .Returns(PricingStrategyEnum.TourDeliveryStrategy);

          var sut = MakeSutWith(tourStrategyMock.Object);

          var delivery = MakeDelivery(pricingStrategy: PricingStrategyEnum.SimpleDeliveryStrategy);

          // Act
          var report = sut.GenerateReport("Customer", _startDate, _startDate.AddDays(1), [delivery]);

          // Assert
          report.Deliveries.Single().Details.Should().ContainSingle(d => d.Description == "Colisage");
     }
}