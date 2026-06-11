using AutoFixture;
using AwesomeAssertions;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Services.PricingStrategy;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Services.Repositories;
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
                              .Create()
                    },

                    new DeliveryStepDto
                    {
                         Data = _fixture.Build<DeliveryStepCrud>()
                              .With(s => s.StepType, StepType.Dropoff)
                              .With(s => s.Order, 2)
                              .Create()
                    }
               ],
               Urgency = urgency.Name,
               PackingSize = packingSize.Name
          };

          var query = new CalculateDeliveryPriceQuery(deliveryCrud);
          var expectedPriceWithoutVat = 100.0;

          pricingStrategyServiceMock
               .Setup(x => x.CalculateDeliveryPriceWithoutVat(It.IsAny<Delivery>()))
               .Returns(expectedPriceWithoutVat);

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
                              .Create()

                    }
               ],
               Urgency = urgency.Name,
               PackingSize = packingSize.Name
          };

          var query = new CalculateDeliveryPriceQuery(deliveryCrud);

          pricingStrategyServiceMock
               .Setup(x => x.CalculateDeliveryPriceWithoutVat(It.IsAny<Delivery>()))
               .Returns(0.0);

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
                              .Create()
                    },
                    new DeliveryStepDto
                    {
                         Data = _fixture.Build<DeliveryStepCrud>()
                              .With(s => s.StepType, StepType.Dropoff)
                              .With(s => s.Order, 2)
                              .Create()
                    },

                    new DeliveryStepDto
                    {
                         Data = _fixture.Build<DeliveryStepCrud>()
                              .With(s => s.StepType, StepType.Dropoff)
                              .With(s => s.Order, 3)
                              .Create()
                    }
               ],
               Urgency = urgency.Name,
               PackingSize = packingSize.Name
          };

          var query = new CalculateDeliveryPriceQuery(deliveryCrud);

          pricingStrategyServiceMock
               .Setup(x => x.CalculateDeliveryPriceWithoutVat(It.IsAny<Delivery>()))
               .Returns(150.0);

          // Act
          var result = await sut.Handle(query, CancellationToken.None);

          // Assert
          result.Price.Should().Be(150.0);
          result.PriceWithVat.Should().Be(180.0);
     }

     private CalculateDeliveryPriceHandler MakeSut(out Mock<IPricingStrategyService> pricingStrategyServiceMock, PackingSize packingSize, Urgency urgency)
     {
          pricingStrategyServiceMock = new Mock<IPricingStrategyService>();
          var urgencyMock = new Mock<IUrgencyRepository>();
          var packingSizeMock = new Mock<IPackingSizeRepository>();
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
               new CalculateDeliveryPriceQueryValidator(urgencyMock.Object, packingSizeMock.Object),
               itineraryService.Object,
               vatServiceMock.Object);
     }
}
