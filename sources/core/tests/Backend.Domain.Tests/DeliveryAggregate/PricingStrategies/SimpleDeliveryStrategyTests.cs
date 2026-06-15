using AwesomeAssertions;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.PricingStrategies;
using Ggio.BikeSherpa.Backend.Domain.SharedKernel;
using Moq;

namespace Backend.Domain.Tests.DeliveryAggregate.PricingStrategies;

public class SimpleDeliveryStrategyTests
{
     private readonly static PackingSize DefaultPackingSize = new("Standard", 2, "Standard", 0, 0);
     private readonly static Urgency DefaultUrgency = new("urgency", 1, "urgency", 1, null, null, 12);
     private readonly static DateTimeOffset BaseDate = new(2026, 1, 15, 10, 0, 0, TimeSpan.Zero);
     private readonly Mock<IDelayService> _mockDelayService = new();

     private SimpleDeliveryStrategy MakeSut() => new(_mockDelayService.Object);

     private Delivery MakeDelivery(DateTimeOffset startDate, DateTimeOffset contractDate) => new()
     {
          PricingStrategy = PricingStrategy.SimpleDeliveryStrategy,
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
     public void Name_ShouldReturn_SimpleDelivery()
     {
          // Arrange & Act
          var sut = MakeSut();

          // Assert
          sut.ImplementedStrategy.Should().Be(PricingStrategy.SimpleDeliveryStrategy);
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
     [InlineData(3, 0, 0, 0, 3 * 1.0)]
     [InlineData(0, 3, 0, 0, 3 * 2.5)]
     [InlineData(0, 0, 3, 0, 3 * 5.5)]
     [InlineData(0, 0, 0, 3, 3 * 11.0)]
     public async Task CalculatePrice_ZoneDropoffSteps_AreMultipliedByCorrectStepPrice(
          int core, int border, int periphery, int outside, double expectedZoneCost)
     {
          // Arrange
          var sut = MakeSut();
          var contractDate = new DateTimeOffset(2026, 1, 14, 10, 0, 0, TimeSpan.Zero);
          var startDate = new DateTimeOffset(2026, 1, 14, 16, 0, 0, TimeSpan.Zero);
          var delivery = MakeDelivery(startDate, contractDate);
          
          var zoneCore = new DeliveryZone("Core") { SimplePrice = 1.0 };
          var zoneBorder = new DeliveryZone("Border") { SimplePrice = 2.5 };
          var zonePeriphery = new DeliveryZone("Periphery") { SimplePrice = 5.5 };
          var zoneOutside = new DeliveryZone("Outside") { SimplePrice = 11.0 };
          
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
          var packingSize = DefaultPackingSize with { SimplePrice = 5.0 };
          var contractDate = new DateTimeOffset(2026, 1, 14, 10, 0, 0, TimeSpan.Zero);
          var startDate = new DateTimeOffset(2026, 1, 14, 16, 0, 0, TimeSpan.Zero);
          var delivery = MakeDelivery(startDate, contractDate);
          
          delivery.Steps.Add(MakeStep(delivery, StepType.Dropoff, new DeliveryZone("Test")));
          delivery.Steps[0].PackingSize = packingSize;
          
          _mockDelayService.Setup(s => s.CalculateDelay(startDate, contractDate)).ReturnsAsync(new Delay { Price = 2, Id = "test", Label = "test" });

          // Act
          var result = await sut.CalculateDeliveryPriceWithoutVat(delivery);

          // Assert
          result.Should().Be(2 + 5.0);
     }

     [Fact]
     public async Task CalculatePrice_IncludesUrgencyCoefficientTimesDistance()
     {
          // Arrange
          var sut = MakeSut();
          var contractDate = new DateTimeOffset(2026, 1, 14, 10, 0, 0, TimeSpan.Zero);
          var startDate = new DateTimeOffset(2026, 1, 14, 16, 0, 0, TimeSpan.Zero);
          var urgency = DefaultUrgency with { PriceCoefficient = 1.5 };
          
          var delivery = MakeDelivery(startDate, contractDate);
          delivery.Urgency = urgency;
          delivery.Steps.Add(MakeStep(delivery, StepType.Dropoff, new DeliveryZone("Test"), 10.0));
          _mockDelayService.Setup(s => s.CalculateDelay(startDate, contractDate)).ReturnsAsync(new Delay { Price = 2, Id = "test", Label = "test" });

          // Act
          var result = await sut.CalculateDeliveryPriceWithoutVat(delivery);

          // Assert
          result.Should().Be(2 + 1.5 * 10.0);
     }

     [Fact]
     public async Task CalculatePrice_WithAllFactors_ReturnsCorrectTotal()
     {
          // Arrange
          var sut = MakeSut();
          var packingSize = DefaultPackingSize with { SimplePrice = 3.0 };
          var contractDate = new DateTimeOffset(2026, 1, 14, 10, 0, 0, TimeSpan.Zero);
          var startDate = new DateTimeOffset(2026, 1, 14, 20, 0, 0, TimeSpan.Zero);
          var urgency = DefaultUrgency with { PriceCoefficient = 2 };
          var delivery = MakeDelivery(startDate, contractDate);
          delivery.Urgency = urgency;
          
          var zoneCore = new DeliveryZone("Core") { SimplePrice = 1 };
          var zoneBorder = new DeliveryZone("Border") { SimplePrice = 2.5 };
          var zonePeriphery = new DeliveryZone("Periphery") { SimplePrice = 5.5 };
          var zoneOutside = new DeliveryZone("Outside") { SimplePrice = 11 };

          // Steps
          // Coût PEC le jour même (2 in mock)
          // 1 dépôt zone centrale (SimplePrice 1)
          // 2 dépôts bordure (SimplePrice 2.5)
          // 1 dépôt périphérie (SimplePrice 5.5)
          // 1 dépôt extérieur (SimplePrice 11)
          // 1 packing (SimplePrice 3)
          // Distance (10) * urgence coefficient (2) = 20

          // Expected: 2 + 1 + 2 * 2.5 + 5.5 + 11 + 3 + 2 * 10
          
          var dropStep = MakeStep(delivery, StepType.Dropoff, zoneCore, 10.0);
          dropStep.PackingSize = packingSize;
          delivery.Steps.Add(dropStep);
          
          delivery.Steps.Add(MakeStep(delivery, StepType.Dropoff, zoneBorder));
          delivery.Steps.Add(MakeStep(delivery, StepType.Dropoff, zoneBorder));
          delivery.Steps.Add(MakeStep(delivery, StepType.Dropoff, zonePeriphery));
          delivery.Steps.Add(MakeStep(delivery, StepType.Dropoff, zoneOutside));
          
          _mockDelayService.Setup(s => s.CalculateDelay(startDate, contractDate)).ReturnsAsync(new Delay { Price = 2, Id = "test", Label = "test" });
          
          // Act
          var result = await sut.CalculateDeliveryPriceWithoutVat(delivery);

          // Assert
          // 2 (delay) + 1 (core) + 2*2.5 (border) + 5.5 (periph) + 11 (outside) + 3 (packing) + 2 * 10 (dist*urg)
          // = 2 + 1 + 5 + 5.5 + 11 + 3 + 20 = 47.5
          result.Should().Be(47.5);
     }
}
