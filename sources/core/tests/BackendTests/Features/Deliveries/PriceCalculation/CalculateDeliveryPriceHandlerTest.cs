using AutoFixture;
using AwesomeAssertions;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Services.PricingStrategy;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Spi;
using Ggio.BikeSherpa.Backend.Domain.SharedKernel;
using Ggio.BikeSherpa.Backend.Features.Deliveries.Model;
using Ggio.BikeSherpa.Backend.Features.Deliveries.PriceCalculation;
using Moq;

namespace BackendTests.Features.Deliveries.PriceCalculation;

public class CalculateDeliveryPriceHandlerTest
{
     private readonly Fixture _fixture = new();

     [Fact]
     public async Task Handle_ShouldReturnCorrectResult_WhenValidQueryIsProvided()
     {
          // Arrange
          var packingSize = new PackingSize("Xl", 1, "Label Xl", 0, 0);
          var urgency = new Urgency("High", 1, "Label High", 0, null, null, 12);
          var sut = MakeSut(out var pricingStrategyServiceMock, packingSize, urgency);

          var deliveryCrud = new DeliveryCrud
          {
               PricingStrategy = PricingStrategy.SimpleDeliveryStrategy,
               Steps =
               [
                    new DeliveryStepDto
                    {
                         Data = _fixture.Build<DeliveryStepCrud>()
                              .With(s => s.StepType, StepType.Pickup)
                              .With(s => s.Order, 1)
                              .With(s => s.PackingSize, packingSize.Name)
                              .Create()
                    },

                    new DeliveryStepDto
                    {
                         Data = _fixture.Build<DeliveryStepCrud>()
                              .With(s => s.StepType, StepType.Dropoff)
                              .With(s => s.Order, 2)
                              .With(s => s.PackingSize, packingSize.Name)
                              .Create()
                    }
               ],
               Urgency = urgency.Name
          };

          var query = new CalculateDeliveryPriceQuery(deliveryCrud);
          var expectedPriceWithoutVat = 100.0;

          pricingStrategyServiceMock
               .Setup(x => x.CalculateDeliveryPriceWithoutVat(It.IsAny<Delivery>()))
               .ReturnsAsync(expectedPriceWithoutVat);

          // Act
          var result = await sut.Handle(query, CancellationToken.None);

          // Assert
          result.Price.Should().Be(expectedPriceWithoutVat);
          result.PriceWithVat.Should().Be(expectedPriceWithoutVat * 1.2);

          pricingStrategyServiceMock.Verify(x => x.CalculateDeliveryPriceWithoutVat(It.IsAny<Delivery>()), Times.Once);
     }

     [Fact]
     public async Task Handle_ShouldCalculateVatCorrectly_WhenPriceIsZero()
     {
          // Arrange
          var packingSize = new PackingSize("Xl", 1, "Label Xl", 0, 0);
          var urgency = new Urgency("High", 1, "Label High", 0, null, null, 16);
          var sut = MakeSut(out var pricingStrategyServiceMock, packingSize, urgency);

          var deliveryCrud = new DeliveryCrud
          {
               PricingStrategy = PricingStrategy.TourDeliveryStrategy,
               Steps =
               [
                    new DeliveryStepDto
                    {
                         Data = _fixture.Build<DeliveryStepCrud>()
                              .With(s => s.StepType, StepType.Pickup)
                              .With(s => s.PackingSize, packingSize.Name)
                              .Create()

                    }
               ],
               Urgency = urgency.Name
          };

          var query = new CalculateDeliveryPriceQuery(deliveryCrud);

          pricingStrategyServiceMock
               .Setup(x => x.CalculateDeliveryPriceWithoutVat(It.IsAny<Delivery>()))
               .ReturnsAsync(0.0);

          // Act
          var result = await sut.Handle(query, CancellationToken.None);

          // Assert
          result.Price.Should().Be(0.0);
          result.PriceWithVat.Should().Be(0.0);
     }

     [Fact]
     public async Task Handle_ShouldCalculateTotalDistance_WhenMultipleStepsProvided()
     {
          // Arrange
          var packingSize = new PackingSize("Xl", 1, "Label Xl", 0, 0);
          var urgency = new Urgency("High", 1, "Label High", 0, null, null, 16);
          var sut = MakeSut(out var pricingStrategyServiceMock, packingSize, urgency);

          var deliveryCrud = new DeliveryCrud
          {
               PricingStrategy = PricingStrategy.SimpleDeliveryStrategy,
               Steps =
               [
                    new DeliveryStepDto
                    {
                         Data = _fixture.Build<DeliveryStepCrud>()
                              .With(s => s.StepType, StepType.Pickup)
                              .With(s => s.Order, 1)
                              .With(s => s.PackingSize, packingSize.Name)
                              .Create()
                    },
                    new DeliveryStepDto
                    {
                         Data = _fixture.Build<DeliveryStepCrud>()
                              .With(s => s.StepType, StepType.Dropoff)
                              .With(s => s.Order, 2)
                              .With(s => s.PackingSize, packingSize.Name)
                              .Create()
                    },

                    new DeliveryStepDto
                    {
                         Data = _fixture.Build<DeliveryStepCrud>()
                              .With(s => s.StepType, StepType.Dropoff)
                              .With(s => s.Order, 3)
                              .With(s => s.PackingSize, packingSize.Name)
                              .Create()
                    }
               ],
               Urgency = urgency.Name
          };

          var query = new CalculateDeliveryPriceQuery(deliveryCrud);

          pricingStrategyServiceMock
               .Setup(x => x.CalculateDeliveryPriceWithoutVat(It.IsAny<Delivery>()))
               .ReturnsAsync(150.0);

          // Act
          var result = await sut.Handle(query, CancellationToken.None);

          // Assert
          result.Price.Should().Be(150.0);
          result.PriceWithVat.Should().Be(180.0);
     }

     [Fact]
     public async Task Handle_ShouldReturnIsInZoneTrue_WhenAllStepsAreInZone()
     {
          // Arrange
          var packingSize = new PackingSize("Xl", 1, "Label Xl", 0, 0);
          var urgency = new Urgency("High", 1, "Label High", 0, null, null, 12);
          var zoneMock = CreateZoneRepositoryMock(inZone: true);
          var sut = MakeSut(out var pricingStrategyServiceMock, packingSize, urgency, zoneMock);
          pricingStrategyServiceMock
               .Setup(x => x.CalculateDeliveryPriceWithoutVat(It.IsAny<Delivery>()))
               .ReturnsAsync(100.0);

          var deliveryCrud = new DeliveryCrud
          {
               PricingStrategy = PricingStrategy.SimpleDeliveryStrategy,
               Steps =
               [
                    CreateStepDto(StepType.Pickup, 1, packingSize),
                    CreateStepDto(StepType.Dropoff, 2, packingSize)
               ],
               Urgency = urgency.Name
          };

          // Act
          var result = await sut.Handle(new CalculateDeliveryPriceQuery(deliveryCrud), CancellationToken.None);

          // Assert
          result.IsInZone.Should().BeTrue();
          zoneMock.Verify(x => x.IsInZone(It.IsAny<string>()), Times.Exactly(2));
     }

     [Fact]
     public async Task Handle_ShouldReturnIsInZoneFalse_WhenAllStepsAreOutOfZone()
     {
          // Arrange
          var packingSize = new PackingSize("Xl", 1, "Label Xl", 0, 0);
          var urgency = new Urgency("High", 1, "Label High", 0, null, null, 12);
          var zoneMock = CreateZoneRepositoryMock(inZone: false);
          var sut = MakeSut(out var pricingStrategyServiceMock, packingSize, urgency, zoneMock);
          pricingStrategyServiceMock
               .Setup(x => x.CalculateDeliveryPriceWithoutVat(It.IsAny<Delivery>()))
               .ReturnsAsync(100.0);

          var deliveryCrud = new DeliveryCrud
          {
               PricingStrategy = PricingStrategy.SimpleDeliveryStrategy,
               Steps =
               [
                    CreateStepDto(StepType.Pickup, 1, packingSize),
                    CreateStepDto(StepType.Dropoff, 2, packingSize)
               ],
               Urgency = urgency.Name
          };

          // Act
          var result = await sut.Handle(new CalculateDeliveryPriceQuery(deliveryCrud), CancellationToken.None);

          // Assert
          result.IsInZone.Should().BeFalse();
     }

     [Fact]
     public async Task Handle_ShouldReturnIsInZoneFalse_WhenOneStepIsOutOfZone()
     {
          // Arrange
          var packingSize = new PackingSize("Xl", 1, "Label Xl", 0, 0);
          var urgency = new Urgency("High", 1, "Label High", 0, null, null, 12);

          var pickup = CreateStepDto(StepType.Pickup, 1, packingSize);
          var dropoff1 = CreateStepDto(StepType.Dropoff, 2, packingSize);
          var dropoff2 = CreateStepDto(StepType.Dropoff, 3, packingSize);

          var zoneMock = new Mock<IDeliveryZoneRepository>();
          zoneMock.Setup(x => x.IsInZone(pickup.Data.StepAddress.City)).Returns(true);
          zoneMock.Setup(x => x.IsInZone(dropoff1.Data.StepAddress.City)).Returns(false);
          zoneMock.Setup(x => x.IsInZone(dropoff2.Data.StepAddress.City)).Returns(true);

          var sut = MakeSut(out var pricingStrategyServiceMock, packingSize, urgency, zoneMock);
          pricingStrategyServiceMock
               .Setup(x => x.CalculateDeliveryPriceWithoutVat(It.IsAny<Delivery>()))
               .ReturnsAsync(100.0);

          var deliveryCrud = new DeliveryCrud
          {
               PricingStrategy = PricingStrategy.TourDeliveryStrategy,
               Steps = [pickup, dropoff1, dropoff2],
               Urgency = urgency.Name
          };

          // Act
          var result = await sut.Handle(new CalculateDeliveryPriceQuery(deliveryCrud), CancellationToken.None);

          // Assert
          result.IsInZone.Should().BeFalse();
     }

     [Fact]
     public async Task Handle_ShouldStopCheckingZones_AfterFirstStepOutOfZone()
     {
          // Arrange
          var packingSize = new PackingSize("Xl", 1, "Label Xl", 0, 0);
          var urgency = new Urgency("High", 1, "Label High", 0, null, null, 12);

          var pickup = CreateStepDto(StepType.Pickup, 1, packingSize);
          var dropoff1 = CreateStepDto(StepType.Dropoff, 2, packingSize);
          var dropoff2 = CreateStepDto(StepType.Dropoff, 3, packingSize);

          var zoneMock = new Mock<IDeliveryZoneRepository>();
          zoneMock.Setup(x => x.IsInZone(pickup.Data.StepAddress.City)).Returns(true);
          zoneMock.Setup(x => x.IsInZone(dropoff1.Data.StepAddress.City)).Returns(false);
          zoneMock.Setup(x => x.IsInZone(dropoff2.Data.StepAddress.City)).Returns(true);

          var sut = MakeSut(out var pricingStrategyServiceMock, packingSize, urgency, zoneMock);
          pricingStrategyServiceMock
               .Setup(x => x.CalculateDeliveryPriceWithoutVat(It.IsAny<Delivery>()))
               .ReturnsAsync(100.0);

          var deliveryCrud = new DeliveryCrud
          {
               PricingStrategy = PricingStrategy.TourDeliveryStrategy,
               Steps = [pickup, dropoff1, dropoff2],
               Urgency = urgency.Name
          };

          // Act
          await sut.Handle(new CalculateDeliveryPriceQuery(deliveryCrud), CancellationToken.None);

          // Assert: the loop short-circuits, so the third step is never checked
          zoneMock.Verify(x => x.IsInZone(pickup.Data.StepAddress.City), Times.Once);
          zoneMock.Verify(x => x.IsInZone(dropoff1.Data.StepAddress.City), Times.Once);
          zoneMock.Verify(x => x.IsInZone(dropoff2.Data.StepAddress.City), Times.Never);
     }

     [Theory]
     [InlineData(true)]
     [InlineData(false)]
     public async Task Handle_ShouldReturnZoneResultOfSingleStep_WhenOnlyOneStepProvided(bool inZone)
     {
          // Arrange
          var packingSize = new PackingSize("Xl", 1, "Label Xl", 0, 0);
          var urgency = new Urgency("High", 1, "Label High", 0, null, null, 16);
          var zoneMock = CreateZoneRepositoryMock(inZone);
          var sut = MakeSut(out var pricingStrategyServiceMock, packingSize, urgency, zoneMock);
          pricingStrategyServiceMock
               .Setup(x => x.CalculateDeliveryPriceWithoutVat(It.IsAny<Delivery>()))
               .ReturnsAsync(50.0);

          var deliveryCrud = new DeliveryCrud
          {
               PricingStrategy = PricingStrategy.TourDeliveryStrategy,
               Steps = [CreateStepDto(StepType.Pickup, 1, packingSize)],
               Urgency = urgency.Name
          };

          // Act
          var result = await sut.Handle(new CalculateDeliveryPriceQuery(deliveryCrud), CancellationToken.None);

          // Assert
          result.IsInZone.Should().Be(inZone);
          zoneMock.Verify(x => x.IsInZone(It.IsAny<string>()), Times.Once);
     }

     private CalculateDeliveryPriceHandler MakeSut(
          out Mock<IPricingStrategyService> pricingStrategyServiceMock,
          PackingSize packingSize,
          Urgency urgency,
          Mock<IDeliveryZoneRepository>? deliveryZoneRepositoryMock = null)
     {
          pricingStrategyServiceMock = new Mock<IPricingStrategyService>();
          var urgencyMock = new Mock<IUrgencyRepository>();
          var packingSizeMock = new Mock<IPackingSizeRepository>();
          deliveryZoneRepositoryMock ??= CreateZoneRepositoryMock(inZone: true);
          var vatServiceMock = new Mock<IVatService>();
          var itineraryService = new Mock<IItinerarySpi>();
          itineraryService.Setup(x => x.GetItineraryInfoAsync(It.IsAny<GeoPoint>(), It.IsAny<GeoPoint>(), It.IsAny<CancellationToken>()))
               .ReturnsAsync(_fixture.Create<ItineraryResult>());

          urgencyMock.Setup(x => x.GetByName(urgency.Name)).Returns(urgency);
          packingSizeMock.Setup(x => x.GetByName(packingSize.Name)).Returns(packingSize);
          vatServiceMock.Setup(x => x.GetPriceWithVatAsync(It.IsAny<double>())).ReturnsAsync((double price) => price * 1.2);
          return new CalculateDeliveryPriceHandler(pricingStrategyServiceMock.Object,
               urgencyMock.Object,
               packingSizeMock.Object,
               deliveryZoneRepositoryMock.Object,
               new CalculateDeliveryPriceQueryValidator(urgencyMock.Object),
               itineraryService.Object,
               vatServiceMock.Object);
     }

     private static Mock<IDeliveryZoneRepository> CreateZoneRepositoryMock(bool inZone)
     {
          var mock = new Mock<IDeliveryZoneRepository>();
          mock.Setup(x => x.IsInZone(It.IsAny<string>())).Returns(inZone);
          return mock;
     }

     private DeliveryStepDto CreateStepDto(StepType stepType, int order, PackingSize packingSize)
     {
          return new()
          {
               Data = _fixture.Build<DeliveryStepCrud>()
                    .With(s => s.StepType, stepType)
                    .With(s => s.Order, order)
                    .With(s => s.PackingSize, packingSize.Name)
                    .Create()
          };
     }
}
