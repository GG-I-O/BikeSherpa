using Ardalis.Specification;
using AutoFixture;
using AwesomeAssertions;
using Ggio.BikeSherpa.Backend.Domain.CustomerAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Services.PricingStrategy;
using Ggio.BikeSherpa.Backend.Domain.SharedKernel;
using Ggio.DddCore;
using Mediator;
using Moq;

namespace Backend.Domain.Tests.DeliveryAggregate;

public class DeliveryFactoryTests
{
     private readonly static Guid CustomerId = Guid.NewGuid();
     private readonly static string CustomerCode = "T01";
     private readonly static DateTimeOffset ContractDate = new(2026, 1, 15, 10, 0, 0, TimeSpan.Zero);
     private readonly static DateTimeOffset StartDate = new(2026, 1, 14, 10, 0, 0, TimeSpan.Zero);
     private readonly Mock<IReadRepository<Customer>> _customerRepositoryMock = new();
     private readonly Mock<IReadRepository<Delivery>> _deliveryRepositoryMock = new();
     private readonly Fixture _fixture = new();
     private readonly Mock<IMediator> _mediatorMock;
     private readonly Mock<IPricingStrategyService> _pricingStrategyServiceMock = new();
     private readonly DeliveryFactory _sut;

     public DeliveryFactoryTests()
     {
          _mediatorMock = new Mock<IMediator>();
          _mediatorMock
               .Setup(m => m.Publish(It.IsAny<AggregateRootAddedEvent>(), It.IsAny<CancellationToken>()))
               .Returns(ValueTask.CompletedTask);

          var fakeCustomer = _fixture.Build<Customer>()
               .With(c => c.Address, _fixture.Build<Address>()
                    .With(a => a.Complement, (string?)null)
                    .Create())
               .With(c => c.Id, CustomerId)
               .With(c => c.Code, CustomerCode)
               .Create();

          _customerRepositoryMock
               .Setup(r => r.FirstOrDefaultAsync(It.IsAny<ISpecification<Customer>>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(fakeCustomer);

          var fakeDelivery = _fixture.Build<Delivery>()
               .With(d => d.Steps, [])
               .With(d => d.Code, $"{CustomerCode}-{ContractDate.Day}{ContractDate.Month}{ContractDate.Year}-1")
               .Create();
          _deliveryRepositoryMock
               .Setup(d => d.ListAsync(It.IsAny<ISpecification<Delivery>>()))
               .ReturnsAsync([fakeDelivery]);

          _pricingStrategyServiceMock
               .Setup(s => s.CalculateDeliveryPriceWithoutVat(It.IsAny<Delivery>()))
               .Returns(55);

          _sut = new DeliveryFactory(_mediatorMock.Object, _customerRepositoryMock.Object, _deliveryRepositoryMock.Object, _pricingStrategyServiceMock.Object);
     }

     private Task<Delivery> CreateDefault(
          PricingStrategy strategy = PricingStrategy.SimpleDeliveryStrategy,
          Guid? customerId = null,
          string urgency = "Normal",
          double? totalPrice = null,
          double? discount = null,
          string[]? details = null,
          string packingSize = "Standard",
          bool insulatedBox = false,
          DateTimeOffset? contractDate = null,
          DateTimeOffset? startDate = null) =>
          _sut.CreateDeliveryAsync(
               strategy, customerId ?? CustomerId, urgency,
               totalPrice, discount, details ?? [], packingSize,
               insulatedBox,
               contractDate ?? ContractDate, startDate ?? StartDate);

     [Fact]
     public async Task CreateDelivery_MapsAllPropertiesOntoTheReturnedDelivery()
     {
          // Arrange
          var details = new[] { "fragile", "urgent" };

          // Act
          var delivery = await CreateDefault(
               PricingStrategy.TourDeliveryStrategy,
               CustomerId,
               "Express",
               discount: 5.0,
               details: details,
               packingSize: "Large",
               insulatedBox: true,
               contractDate: ContractDate,
               startDate: StartDate);

          // Assert
          delivery.PricingStrategy.Should().Be(PricingStrategy.TourDeliveryStrategy);
          delivery.Code.Should().Be($"60114-T01-2");
          delivery.CustomerId.Should().Be(CustomerId);
          delivery.Urgency.Should().Be("Express");
          delivery.TotalPrice.Should().Be(55);
          delivery.Discount.Should().Be(5.0);
          delivery.Details.Should().BeEquivalentTo(details);
          delivery.PackingSize.Should().Be("Large");
          delivery.InsulatedBox.Should().BeTrue();
          delivery.ContractDate.Should().Be(ContractDate);
          delivery.StartDate.Should().Be(StartDate);
          delivery.Status.Should().Be(DeliveryStatus.Pending);
          delivery.ReportId.Should().NotBeNull();
          delivery.TotalPrice.Should().Be(55);
     }

     [Fact]
     public async Task CreateDelivery_InitialisesStepsAsEmptyList()
     {
          // Arrange & Act
          var delivery = await CreateDefault();

          // Assert
          delivery.Steps.Should().BeEmpty();
     }

     [Fact]
     public async Task CreateDelivery_WhenOptionalFieldsAreNull_LeavesThemNull()
     {
          // Arrange & Act
          var delivery = await CreateDefault(discount: null);

          // Assert
          delivery.Discount.Should().BeNull();
     }

     [Fact]
     public async Task CreateDelivery_PublishesDomainEntityAddedEventContainingTheCreatedDelivery()
     {
          // Arrange & Act
          var delivery = await CreateDefault();

          // Assert
          _mediatorMock.Verify(
               m => m.Publish(It.Is<AggregateRootAddedEvent>(e => ReferenceEquals(e.NewAggregate, delivery)), It.IsAny<CancellationToken>()),
               Times.Once);
     }
}
