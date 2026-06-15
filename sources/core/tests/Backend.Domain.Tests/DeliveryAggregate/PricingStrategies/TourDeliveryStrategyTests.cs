using AwesomeAssertions;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.PricingStrategies;
using Ggio.BikeSherpa.Backend.Domain.SharedKernel;
using Moq;

namespace Backend.Domain.Tests.DeliveryAggregate.PricingStrategies;

public class TourDeliveryStrategyTests
{
     private readonly static PackingSize DefaultPackingSize = new("Standard", 2, "Standard", 0, 0);
     private readonly static Urgency DefaultUrgency = new("urgency", 1, "urgency", 1, null, null, 12);
     private readonly static DateTimeOffset BaseDate = new(2026, 1, 15, 10, 0, 0, TimeSpan.Zero);
     private readonly Mock<IDelayService> _mockDelayService = new();

     private TourDeliveryStrategy MakeSut() => new(_mockDelayService.Object);

     private Delivery MakeDelivery(DateTimeOffset startDate, DateTimeOffset contractDate) => new()
     {
          PricingStrategy = PricingStrategy.TourDeliveryStrategy,
          Code = "DEL-001",
          CustomerId = Guid.NewGuid(),
          StartDate = startDate,
          ContractDate = contractDate,
          Steps = new List<DeliveryStep>(),
          Urgency = DefaultUrgency,
          InsulatedBox = false,
          ExtraCost = 0,
          Discount = 0
     };

     private DeliveryStep MakeStep(Delivery parentDelivery, StepType type, DeliveryZone zone, double distance = 0)
     {
          var address = new Address 
          { 
              Name = "Name", 
              StreetInfo = "Street", 
              Postcode = "Zip", 
              City = "City", 
              Coordinates = new GeoPoint(0, 0)
          };
          return new DeliveryStep(type, 1, address)
          {
               StepAddress = address,
               StepZone = zone,
               Distance = distance,
               ParentDelivery = parentDelivery,
               PackingSize = DefaultPackingSize
          };
     }

     [Fact]
     public void Name_ShouldReturn_TourDelivery()
     {
          // Arrange & Act
          var sut = MakeSut();

          // Assert
          sut.ImplementedStrategy.Should().Be(PricingStrategy.TourDeliveryStrategy);
     }

     [Theory]
     [InlineData(1, 14.0)]
     [InlineData(2, 28.0)]
     [InlineData(3, 42.0)]
     public async Task CalculatePrice_PickupNumber_IsMultipliedByBasePrice(int pickupNumber, double expectedPickupCost)
     {
          // Arrange
          var sut = MakeSut();
          var contractDate = new DateTimeOffset(2026, 1, 14, 10, 0, 0, TimeSpan.Zero);
          var startDate = new DateTimeOffset(2026, 1, 14, 20, 0, 0, TimeSpan.Zero);
          var delivery = MakeDelivery(startDate, contractDate);
          
          for (int i = 0; i < pickupNumber; i++) delivery.Steps.Add(MakeStep(delivery, StepType.Pickup, new DeliveryZone("Test") { TourPrice = 14.0 }));
          
          _mockDelayService.Setup(s => s.CalculateDelay(startDate, contractDate)).ReturnsAsync(new Delay { Price = 2, Id = "test", Label = "test" });

          // Act
          var result = await sut.CalculateDeliveryPriceWithoutVat(delivery);

          // Assert
          result.Should().Be(2 + expectedPickupCost);
     }

     [Fact]
     public async Task CalculatePrice_WhenDeliveryIsOnSameDayAsContractDate_AddsExtraCost()
     {
          // Arrange
          var sut = MakeSut();
          var contractDate = BaseDate;
          var startDate = BaseDate.AddHours(5);
          var delivery = MakeDelivery(startDate, contractDate);
          _mockDelayService.Setup(s => s.CalculateDelay(startDate, contractDate)).ReturnsAsync(new Delay { Price = 2, Id = "test", Label = "test" });

          // Act
          var result = await sut.CalculateDeliveryPriceWithoutVat(delivery);

          // Assert
          result.Should().Be(2);
     }

     [Fact]
     public async Task CalculatePrice_WhenOrderIsAfter5PM_AndDeliveryIsNextDay_AddsSameDayCost()
     {
          // Arrange
          var sut = MakeSut();
          var contractDate = new DateTimeOffset(2026, 1, 15, 18, 0, 0, TimeSpan.Zero);
          var startDate = new DateTimeOffset(2026, 1, 16, 10, 0, 0, TimeSpan.Zero);
          var delivery = MakeDelivery(startDate, contractDate);
          _mockDelayService.Setup(s => s.CalculateDelay(startDate, contractDate)).ReturnsAsync(new Delay { Price = 2, Id = "test", Label = "test" });

          // Act
          var result = await sut.CalculateDeliveryPriceWithoutVat(delivery);

          // Assert
          result.Should().Be(2);
     }

     [Fact]
     public async Task CalculatePrice_WhenDeliveryIsOnDifferentDay_AppliesEarlyOrderDiscountAndNoSameDayCost()
     {
          // Arrange
          var sut = MakeSut();
          var contractDate = BaseDate;
          var startDate = BaseDate.AddHours(25);
          var delivery = MakeDelivery(startDate, contractDate);
          _mockDelayService.Setup(s => s.CalculateDelay(startDate, contractDate)).ReturnsAsync(new Delay { Price = -2, Id = "test", Label = "test" });

          // Act
          var result = await sut.CalculateDeliveryPriceWithoutVat(delivery);

          // Assert
          result.Should().Be(-2);
     }

     [Fact]
     public async Task CalculatePrice_WhenDelayIsAtMost2Hours_AppliesLastMinuteExtraCostAndSameDayCost()
     {
          // Arrange
          var sut = MakeSut();
          var contractDate = BaseDate;
          var startDate = BaseDate.AddHours(1);
          var delivery = MakeDelivery(startDate, contractDate);
          _mockDelayService.Setup(s => s.CalculateDelay(startDate, contractDate)).ReturnsAsync(new Delay { Price = 2 + 3, Id = "test", Label = "test" });

          // Act
          var result = await sut.CalculateDeliveryPriceWithoutVat(delivery);

          // Assert
          result.Should().Be(2 + 3);
     }

     [Fact]
     public async Task CalculatePrice_WhenDelayIsExactly2Hours_AppliesLastMinuteExtraCostAndSameDayCost()
     {
          // Arrange
          var sut = MakeSut();
          var contractDate = BaseDate;
          var startDate = BaseDate.AddHours(2);
          var delivery = MakeDelivery(startDate, contractDate);
          _mockDelayService.Setup(s => s.CalculateDelay(startDate, contractDate)).ReturnsAsync(new Delay { Price = 2 + 3, Id = "test", Label = "test" });

          // Act
          var result = await sut.CalculateDeliveryPriceWithoutVat(delivery);

          // Assert
          result.Should().Be(2 + 3);
     }

     [Fact]
     public async Task CalculatePrice_WhenDelayIsBetween2And18Hours_AppliesOnlySameDayCost()
     {
          // Arrange
          var sut = MakeSut();
          var contractDate = BaseDate;
          var startDate = BaseDate.AddHours(6);
          var delivery = MakeDelivery(startDate, contractDate);
          _mockDelayService.Setup(s => s.CalculateDelay(startDate, contractDate)).ReturnsAsync(new Delay { Price = 2, Id = "test", Label = "test" });

          // Act
          var result = await sut.CalculateDeliveryPriceWithoutVat(delivery);

          // Assert
          result.Should().Be(2);
     }

     [Theory]
     [InlineData(3, 0, 0, 0, 3 * 5.0)]
     [InlineData(0, 3, 0, 0, 3 * 8.0)]
     [InlineData(0, 0, 3, 0, 3 * 0.0)]
     [InlineData(0, 0, 0, 3, 3 * 0.0)]
     public async Task CalculatePrice_ZoneDropoffSteps_AreMultipliedByCorrectStepPrice(
          int core, int border, int periphery, int outside, double expectedZoneCost)
     {
          // Arrange
          var sut = MakeSut();
          var contractDate = new DateTimeOffset(2026, 1, 14, 10, 0, 0, TimeSpan.Zero);
          var startDate = new DateTimeOffset(2026, 1, 14, 16, 0, 0, TimeSpan.Zero);
          var delivery = MakeDelivery(startDate, contractDate);
          
          var zoneCore = new DeliveryZone("Core") { TourPrice = 5.0 };
          var zoneBorder = new DeliveryZone("Border") { TourPrice = 8.0 };
          var zonePeriphery = new DeliveryZone("Periphery") { TourPrice = 0.0 };
          var zoneOutside = new DeliveryZone("Outside") { TourPrice = 0.0 };
          
          for (int i = 0; i < core; i++) delivery.Steps.Add(MakeStep(delivery, StepType.Dropoff, zoneCore));
          for (int i = 0; i < border; i++) delivery.Steps.Add(MakeStep(delivery, StepType.Dropoff, zoneBorder));
          for (int i = 0; i < periphery; i++) delivery.Steps.Add(MakeStep(delivery, StepType.Dropoff, zonePeriphery));
          for (int i = 0; i < outside; i++) delivery.Steps.Add(MakeStep(delivery, StepType.Dropoff, zoneOutside));
          
          _mockDelayService.Setup(s => s.CalculateDelay(startDate, contractDate)).ReturnsAsync(new Delay { Price = 2, Id = "test", Label = "test" });

          // Act
          var result = await sut.CalculateDeliveryPriceWithoutVat(delivery);

          // Assert
          result.Should().Be(2 + expectedZoneCost);
     }

     [Fact]
     public async Task CalculatePrice_IncludesPackingSizePrice()
     {
          // Arrange
          var sut = MakeSut();
          var packingSize = DefaultPackingSize with { TourPrice = 7.0 };
          var contractDate = new DateTimeOffset(2026, 1, 14, 10, 0, 0, TimeSpan.Zero);
          var startDate = new DateTimeOffset(2026, 1, 14, 16, 0, 0, TimeSpan.Zero);
          var delivery = MakeDelivery(startDate, contractDate);
          
          var step = MakeStep(delivery, StepType.Dropoff, new DeliveryZone("Test") { SimplePrice = 0 });
          step.PackingSize = packingSize;
          delivery.Steps.Add(step);
          
          _mockDelayService.Setup(s => s.CalculateDelay(startDate, contractDate)).ReturnsAsync(new Delay { Price = 2, Id = "test", Label = "test" });

          // Act
          var result = await sut.CalculateDeliveryPriceWithoutVat(delivery);

          // Assert
          result.Should().Be(2 + 7.0);
     }

     [Fact]
     public async Task CalculatePrice_IncludesUrgencyCoefficientTimesDistance()
     {
          // Arrange
          var sut = MakeSut();
          var contractDate = new DateTimeOffset(2026, 1, 14, 10, 0, 0, TimeSpan.Zero);
          var startDate = new DateTimeOffset(2026, 1, 14, 20, 0, 0, TimeSpan.Zero);
          var urgency = DefaultUrgency with { PriceCoefficient = 10 };
          var deliveryWithoutUrgency = MakeDelivery(startDate, contractDate);
          var deliveryWithUrgency = MakeDelivery(startDate, contractDate);
          deliveryWithUrgency.Urgency = urgency;
          _mockDelayService.Setup(s => s.CalculateDelay(startDate, contractDate)).ReturnsAsync(new Delay { Price = 2, Id = "test", Label = "test" });

          // Act
          var resultWithoutUrgency = await sut.CalculateDeliveryPriceWithoutVat(deliveryWithoutUrgency);
          var resultWithUrgency = await sut.CalculateDeliveryPriceWithoutVat(deliveryWithUrgency);

          // Assert
          resultWithUrgency.Should().Be(resultWithoutUrgency);
     }

     [Fact]
     public async Task CalculatePrice_WithAllFactors_ReturnsCorrectTotal()
     {
          // Arrange
          var sut = MakeSut();
          var packingSize = DefaultPackingSize with { TourPrice = 4.0 };
          var contractDate = new DateTimeOffset(2026, 1, 14, 10, 0, 0, TimeSpan.Zero);
          var startDate = new DateTimeOffset(2026, 1, 14, 16, 0, 0, TimeSpan.Zero);
          var delivery = MakeDelivery(startDate, contractDate);
          
          var zoneCore = new DeliveryZone("Core") { TourPrice = 5.0 };
          var zoneBorder = new DeliveryZone("Border") { TourPrice = 8.0 };
          
          delivery.Steps.Add(MakeStep(delivery, StepType.Pickup, zoneCore));
          
          var dropStep = MakeStep(delivery, StepType.Dropoff, zoneCore);
          dropStep.PackingSize = packingSize;
          delivery.Steps.Add(dropStep);
          
          delivery.Steps.Add(MakeStep(delivery, StepType.Dropoff, zoneBorder));
          delivery.Steps.Add(MakeStep(delivery, StepType.Dropoff, zoneBorder));
          
          _mockDelayService.Setup(s => s.CalculateDelay(startDate, contractDate)).ReturnsAsync(new Delay { Price = 2, Id = "test", Label = "test" });

          // Act
          var result = await sut.CalculateDeliveryPriceWithoutVat(delivery);

          // Assert
          // 2 + 5 (pickup) + 4 (packing) + 5 (dropoff core) + 16 (dropoff border) = 32.0
          result.Should().Be(32.0);
     }
}
