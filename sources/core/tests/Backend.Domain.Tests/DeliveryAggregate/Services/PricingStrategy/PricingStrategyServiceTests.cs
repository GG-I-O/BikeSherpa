using AutoFixture;
using AutoFixture.AutoMoq;
using AwesomeAssertions;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.PricingStrategies;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Services.PricingStrategy;
using Ggio.BikeSherpa.Backend.Domain.SharedKernel;
using Moq;
using PricingStrategyEnum = Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations.PricingStrategy;

namespace Backend.Domain.Tests.DeliveryAggregate.Services.PricingStrategy;

public class PricingStrategyServiceTests
{
     private static DateTimeOffset _startDate;
     private static DateTimeOffset _contractDate;
     private readonly static Urgency Urgency = new("urgency", 1, "urgency", 1, null, null, 12);
     private readonly static PackingSize PackingSize = new("packing", 1, "label", 3, 10);
     private readonly static DeliveryZone CoreZone = new("Centre");
     private readonly static DeliveryZone BorderZone = new("Limitrophe");
     private readonly static DeliveryZone PeripheryZone = new("Périphérie");
     private readonly static DeliveryZone OutsideZone = new("Extérieur");
     private static Address? _defaultAddress;
     private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());

     private readonly Mock<IPricingStrategy> _mockPricingStrategy;
     private readonly PricingStrategyService _sut;

     public PricingStrategyServiceTests()
     {
          _mockPricingStrategy = new Mock<IPricingStrategy>();
          _startDate = _fixture.Create<DateTimeOffset>();
          _contractDate = _fixture.Create<DateTimeOffset>().AddHours(10);
          _defaultAddress = _fixture.Create<Address>();
          _mockPricingStrategy.Setup(s => s.ImplementedStrategy).Returns(PricingStrategyEnum.SimpleDeliveryStrategy);
          _mockPricingStrategy
               .Setup(s => s.CalculateDeliveryPriceWithoutVat(
                    It.IsAny<DateTimeOffset>(),
                    It.IsAny<DateTimeOffset>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<Urgency>(),
                    It.IsAny<double>(),
                    It.IsAny<double>(),
                    It.IsAny<double>()))
               .Returns(0);

          _sut = new PricingStrategyService([_mockPricingStrategy.Object]);
     }

     private Delivery MakeDelivery(
          PricingStrategyEnum pricingStrategy = PricingStrategyEnum.SimpleDeliveryStrategy,
          List<DeliveryStep>? steps = null,
          double? totalPrice = null,
          Urgency? urgency = null,
          PackingSize? packingSize = null) =>
          new()
          {
               PricingStrategy = pricingStrategy,
               Code = _fixture.Create<string>(),
               CustomerId = _fixture.Create<Guid>(),
               Urgency = urgency ?? Urgency,
               PackingSize = packingSize ?? PackingSize,
               InsulatedBox = false,
               ContractDate = _contractDate,
               StartDate = _startDate,
               Steps = steps ?? [],
               TotalPrice = totalPrice
          };

     private Delivery MakeDeliveryWithAdjustments(
          double? discount = null,
          double? extraCost = null,
          double? distance = null,
          double? totalPrice = null)
     {
          var delivery = MakeDelivery(totalPrice: totalPrice);
          delivery.Discount = discount;
          delivery.ExtraCost = extraCost;

          return delivery;
     }

     private static DeliveryStep MakeStep(Delivery parentDelivery, StepType type, DeliveryZone zone, double distance = 0, bool notBilled = false) => new(type, 1, _defaultAddress!)
     {
          Id = Guid.NewGuid(),
          StepAddress = _defaultAddress!,
          StepZone = zone,
          Distance = distance,
          ParentDelivery = parentDelivery,
          NotBilled = notBilled
     };

     private PricingStrategyService MakeSutWith(
          params IPricingStrategy[] strategies) =>
          new(strategies);

     [Fact]
     public void CalculatePrice_DelegatesToMatchingStrategy_AndNotToOthers()
     {
          // Arrange
          var matchingMock = new Mock<IPricingStrategy>();
          matchingMock.Setup(s => s.ImplementedStrategy).Returns(PricingStrategyEnum.SimpleDeliveryStrategy);
          matchingMock.Setup(s => s.CalculateDeliveryPriceWithoutVat(
                    It.IsAny<DateTimeOffset>(),
                    It.IsAny<DateTimeOffset>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<Urgency>(),
                    It.IsAny<double>(),
                    It.IsAny<double>(),
                    It.IsAny<double>()))
               .Returns(42.0);

          var otherMock = new Mock<IPricingStrategy>();
          otherMock.Setup(s => s.ImplementedStrategy).Returns(PricingStrategyEnum.TourDeliveryStrategy);
          otherMock.Setup(s => s.CalculateDeliveryPriceWithoutVat(
                    It.IsAny<DateTimeOffset>(),
                    It.IsAny<DateTimeOffset>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<int>(),
                    It.IsAny<Urgency>(),
                    It.IsAny<double>(),
                    It.IsAny<double>(),
                    It.IsAny<double>()))
               .Returns(99.0);

          // Act
          var sut = MakeSutWith([matchingMock.Object, otherMock.Object]);
          var result = sut.CalculateDeliveryPriceWithoutVat(MakeDelivery());

          // Assert
          result.Should().Be(42.0);
          matchingMock.Verify(s => s.CalculateDeliveryPriceWithoutVat(
               It.IsAny<DateTimeOffset>(),
               It.IsAny<DateTimeOffset>(),
               It.IsAny<int>(),
               It.IsAny<int>(),
               It.IsAny<int>(),
               It.IsAny<int>(),
               It.IsAny<int>(),
               It.IsAny<Urgency>(),
               It.IsAny<double>(),
               It.IsAny<double>(),
               It.IsAny<double>()), Times.Once);

          otherMock.Verify(s => s.CalculateDeliveryPriceWithoutVat(
               It.IsAny<DateTimeOffset>(),
               It.IsAny<DateTimeOffset>(),
               It.IsAny<int>(),
               It.IsAny<int>(),
               It.IsAny<int>(),
               It.IsAny<int>(),
               It.IsAny<int>(),
               It.IsAny<Urgency>(),
               It.IsAny<double>(),
               It.IsAny<double>(),
               It.IsAny<double>()), Times.Never);
     }

     [Fact]
     public void CalculatePrice_WhenCustomStrategy_ReturnsTotalPriceWithoutCallingStrategy()
     {
          // Arrange
          var customMock = new Mock<IPricingStrategy>();
          customMock.Setup(s => s.ImplementedStrategy).Returns(PricingStrategyEnum.CustomStrategy);
          var sut = MakeSutWith([customMock.Object]);

          // Act
          var result = sut.CalculateDeliveryPriceWithoutVat(
               MakeDelivery(PricingStrategyEnum.CustomStrategy, totalPrice: 99.5));

          // Assert
          result.Should().Be(99.5);
          customMock.Verify(s => s.CalculateDeliveryPriceWithoutVat(
               It.IsAny<DateTimeOffset>(),
               It.IsAny<DateTimeOffset>(),
               It.IsAny<int>(),
               It.IsAny<int>(),
               It.IsAny<int>(),
               It.IsAny<int>(),
               It.IsAny<int>(),
               It.IsAny<Urgency>(),
               It.IsAny<double>(),
               It.IsAny<double>(),
               It.IsAny<double>()), Times.Never);
     }

     [Fact]
     public void CalculatePrice_WhenNoStrategyMatches_ReturnsTotalPrice()
     {
          // Arrange
          var tourMock = new Mock<IPricingStrategy>();
          tourMock.Setup(s => s.ImplementedStrategy).Returns(PricingStrategyEnum.TourDeliveryStrategy);
          var sut = MakeSutWith([tourMock.Object]);

          // Act
          var result = sut.CalculateDeliveryPriceWithoutVat(MakeDelivery(totalPrice: 123.45));

          // Assert
          result.Should().Be(123.45);
          tourMock.Verify(s => s.CalculateDeliveryPriceWithoutVat(
               It.IsAny<DateTimeOffset>(),
               It.IsAny<DateTimeOffset>(),
               It.IsAny<int>(),
               It.IsAny<int>(),
               It.IsAny<int>(),
               It.IsAny<int>(),
               It.IsAny<int>(),
               It.IsAny<Urgency>(),
               It.IsAny<double>(),
               It.IsAny<double>(),
               It.IsAny<double>()), Times.Never);
     }

     [Fact]
     public void CalculatePrice_CountsOnlyPickupStepsAsPickups()
     {
          // Arrange
          var delivery = MakeDelivery();
          var steps = new List<DeliveryStep>
          {
               MakeStep(delivery, StepType.Pickup, CoreZone),
               MakeStep(delivery, StepType.Pickup, CoreZone),
               MakeStep(delivery, StepType.Dropoff, CoreZone)
          };

          delivery.Steps = steps;

          // Act
          _sut.CalculateDeliveryPriceWithoutVat(delivery);

          // Assert
          _mockPricingStrategy.Verify(s => s.CalculateDeliveryPriceWithoutVat(
               It.IsAny<DateTimeOffset>(),
               It.IsAny<DateTimeOffset>(),
               2,
               It.IsAny<int>(),
               It.IsAny<int>(),
               It.IsAny<int>(),
               It.IsAny<int>(),
               It.IsAny<Urgency>(),
               It.IsAny<double>(),
               It.IsAny<double>(),
               It.IsAny<double>()), Times.Once);
     }

     [Fact]
     public void CalculatePrice_CountsDropoffStepsPerZone()
     {
          // Arrange
          var delivery = MakeDelivery();
          var steps = new List<DeliveryStep>
          {
               MakeStep(delivery, StepType.Dropoff, CoreZone),
               MakeStep(delivery, StepType.Dropoff, CoreZone),
               MakeStep(delivery, StepType.Dropoff, BorderZone),
               MakeStep(delivery, StepType.Dropoff, PeripheryZone),
               MakeStep(delivery, StepType.Dropoff, OutsideZone),
               MakeStep(delivery, StepType.Dropoff, OutsideZone)
          };

          delivery.Steps = steps;

          // Act
          _sut.CalculateDeliveryPriceWithoutVat(delivery);

          // Assert
          _mockPricingStrategy.Verify(s => s.CalculateDeliveryPriceWithoutVat(
               It.IsAny<DateTimeOffset>(),
               It.IsAny<DateTimeOffset>(),
               It.IsAny<int>(),
               2,
               1,
               1,
               2,
               It.IsAny<Urgency>(),
               It.IsAny<double>(),
               It.IsAny<double>(),
               It.IsAny<double>()), Times.Once);
     }

     [Fact]
     public void CalculatePrice_SumsDistanceAcrossAllSteps()
     {
          // Arrange
          var distance1 = _fixture.Create<double>();
          var distance2 = _fixture.Create<double>();
          var delivery = MakeDelivery();
          var steps = new List<DeliveryStep>
          {
               MakeStep(delivery, StepType.Pickup, CoreZone, distance1),
               MakeStep(delivery, StepType.Dropoff, CoreZone, distance2)
          };

          delivery.Steps = steps;

          // Act
          _sut.CalculateDeliveryPriceWithoutVat(delivery);

          // Assert
          _mockPricingStrategy.Verify(s => s.CalculateDeliveryPriceWithoutVat(
               It.IsAny<DateTimeOffset>(),
               It.IsAny<DateTimeOffset>(),
               It.IsAny<int>(),
               It.IsAny<int>(),
               It.IsAny<int>(),
               It.IsAny<int>(),
               It.IsAny<int>(),
               It.IsAny<Urgency>(),
               distance1 + distance2,
               It.IsAny<double>(),
               It.IsAny<double>()), Times.Once);
     }

     [Fact]
     public void CalculatePrice_SumsDistanceAcrossAllSteps_WhileIgnoringNotBilledSteps()
     {
          // Arrange
          var distance1 = _fixture.Create<double>();
          var distance2 = _fixture.Create<double>();
          var delivery = MakeDelivery();
          var steps = new List<DeliveryStep>
          {
               MakeStep(delivery, StepType.Pickup, CoreZone, distance1),
               MakeStep(delivery, StepType.Dropoff, CoreZone, distance2, true),
               MakeStep(delivery, StepType.Dropoff, CoreZone, distance2)
          };

          delivery.Steps = steps;

          // Act
          _sut.CalculateDeliveryPriceWithoutVat(delivery);

          // Assert
          _mockPricingStrategy.Verify(s => s.CalculateDeliveryPriceWithoutVat(
               It.IsAny<DateTimeOffset>(),
               It.IsAny<DateTimeOffset>(),
               It.IsAny<int>(),
               It.IsAny<int>(),
               It.IsAny<int>(),
               It.IsAny<int>(),
               It.IsAny<int>(),
               It.IsAny<Urgency>(),
               distance1 + distance2,
               It.IsAny<double>(),
               It.IsAny<double>()), Times.Once);
     }


     [Fact]
     public void CalculatePrice_WhenDeliveryDistanceIsZero_SumsBillableStepsDistance()
     {
          // Arrange
          const double billableDistance = 12.5;
          const double ignoredDistance = 8.5;
          var delivery = MakeDeliveryWithAdjustments(distance: 0);
          var steps = new List<DeliveryStep>
          {
               MakeStep(delivery, StepType.Pickup, CoreZone, billableDistance),
               MakeStep(delivery, StepType.Dropoff, CoreZone, ignoredDistance, true)
          };

          delivery.Steps = steps;

          // Act
          _sut.CalculateDeliveryPriceWithoutVat(delivery);

          // Assert
          _mockPricingStrategy.Verify(s => s.CalculateDeliveryPriceWithoutVat(
               It.IsAny<DateTimeOffset>(),
               It.IsAny<DateTimeOffset>(),
               It.IsAny<int>(),
               It.IsAny<int>(),
               It.IsAny<int>(),
               It.IsAny<int>(),
               It.IsAny<int>(),
               It.IsAny<Urgency>(),
               billableDistance,
               It.IsAny<double>(),
               It.IsAny<double>()), Times.Once);
     }

     [Fact]
     public void CalculatePrice_PassesPackingSizeFromRepository()
     {
          // Arrange
          PackingSize packing = new("Large", 2, "Large Label", 10, 20);

          // Act
          _sut.CalculateDeliveryPriceWithoutVat(MakeDelivery(packingSize: packing));

          // Assert
          _mockPricingStrategy.Verify(s => s.CalculateDeliveryPriceWithoutVat(
               It.IsAny<DateTimeOffset>(),
               It.IsAny<DateTimeOffset>(),
               It.IsAny<int>(),
               It.IsAny<int>(),
               It.IsAny<int>(),
               It.IsAny<int>(),
               It.IsAny<int>(),
               It.IsAny<Urgency>(),
               It.IsAny<double>(),
               It.IsAny<double>(),
               It.IsAny<double>()), Times.Once);
     }
}
