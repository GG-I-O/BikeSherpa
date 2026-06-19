using AutoFixture;
using AwesomeAssertions;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.PricingStrategies;
using Ggio.BikeSherpa.Backend.Domain.SharedKernel;
using Ggio.BikeSherpa.Backend.Features.Reports.Services;
using Moq;
using PricingStrategyEnum = Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations.PricingStrategy;

namespace BackendTests.Features.Reports.Services;

public class ReportServiceTests
{
     private readonly static Urgency Urgency = new("Standard", 1, "Standard", 1, null, null, 12);
     private readonly DateTimeOffset _contractDate = new(2026, 1, 14, 10, 0, 0, TimeSpan.Zero);
     private readonly Mock<IDelayService> _delayServiceMock = new();
     private readonly Fixture _fixture = new();
     private readonly Mock<IPricingStrategy> _pricingStrategyMock = new();

     private readonly DateTimeOffset _startDate = new(2026, 1, 15, 10, 0, 0, TimeSpan.Zero);
     private readonly ReportService _sut;
     private readonly Mock<IVatService> _vatServiceMock = new();

     public ReportServiceTests()
     {
          _pricingStrategyMock
               .Setup(s => s.ImplementedStrategy)
               .Returns(PricingStrategyEnum.SimpleDeliveryStrategy);

          _sut = new ReportService([_pricingStrategyMock.Object], _delayServiceMock.Object, _vatServiceMock.Object);
     }

     private Delivery MakeDelivery(
          PricingStrategyEnum pricingStrategy = PricingStrategyEnum.SimpleDeliveryStrategy,
          string? code = null,
          DateTimeOffset? startDate = null,
          DateTimeOffset? contractDate = null,
          double? totalPrice = null,
          double? discount = null,
          double? extraCost = null,
          List<DeliveryStep>? steps = null,
          Urgency? urgency = null) => new()
     {
          PricingStrategy = pricingStrategy,
          Code = code ?? _fixture.Create<string>(),
          CustomerId = _fixture.Create<Guid>(),
          Urgency = urgency ?? Urgency,
          InsulatedBox = false,
          ContractDate = contractDate ?? _contractDate,
          StartDate = startDate ?? _startDate,
          Steps = steps ?? [],
          TotalPrice = totalPrice,
          Discount = discount,
          ExtraCost = extraCost
     };

     [Fact]
     public async Task GenerateReport_MapsReportHeader()
     {
          // Arrange
          var customerName = _fixture.Create<string>();
          var from = new DateTimeOffset(2026, 1, 1, 0, 0, 0, TimeSpan.Zero);
          var to = new DateTimeOffset(2026, 1, 31, 0, 0, 0, TimeSpan.Zero);

          // Act
          var report = await _sut.GenerateDeliveryReportAsync(customerName, from, to, []);

          // Assert
          report.CustomerName.Should().Be(customerName);
          report.StartDate.Should().Be(from);
          report.EndDate.Should().Be(to);
          report.TotalPrice.Should().Be(0);
          report.Deliveries.Should().BeEmpty();
     }

     [Fact]
     public async Task GenerateReport_MapsDeliverySummaryAndTotals()
     {
          // Arrange
          var firstDelivery = MakeDelivery(code: "DEL-001", totalPrice: 12.50);
          var secondDelivery = MakeDelivery(code: "DEL-002", totalPrice: 27.25);

          // Act
          var report = await _sut.GenerateDeliveryReportAsync(
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
     public async Task GenerateReport_WhenDeliveryTotalPriceIsNull_UsesZero()
     {
          // Arrange
          var delivery = MakeDelivery(totalPrice: null);

          // Act
          var report = await _sut.GenerateDeliveryReportAsync("Customer", _startDate, _startDate.AddDays(1), [delivery]);

          // Assert
          report.TotalPrice.Should().Be(0);
          report.Deliveries.Single().DeliveryPrice.Should().Be(0);
     }
}
