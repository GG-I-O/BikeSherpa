using Ardalis.Specification;
using AutoFixture;
using AutoFixture.AutoMoq;
using AwesomeAssertions;
using Ggio.BikeSherpa.Backend.Domain.CustomerAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Services.PricingStrategy;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Services.Repositories;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Specification;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.SPI;
using Ggio.BikeSherpa.Backend.Features.Deliveries.Add;
using Ggio.DddCore;
using JetBrains.Annotations;
using Moq;

namespace BackendTests.Features.Deliveries.Add;

[TestSubject(typeof(AddDeliveryStepHandler))]
public class AddDeliveryStepHandlerTest
{
     private readonly Mock<IApplicationTransaction> _mockTransaction = new();
     private readonly Mock<IReadRepository<Delivery>> _mockDeliveryRepository = new();
     private readonly Mock<IDeliveryZoneRepository> _mockDeliveryZoneRepository = new();
     private readonly Mock<IPricingStrategyService> _mockPricingStrategyService = new();
     private readonly Mock<IItinerarySpi> _mockItineraryApi = new();
     private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());

     private readonly AddDeliveryStepCommand _mockCommand;
     private readonly Delivery _mockDelivery;

     public AddDeliveryStepHandlerTest()
     {
          var mockCustomer = _fixture.Create<Customer>();
          _mockDelivery = _fixture.Build<Delivery>()
               .With(d => d.Steps, [])
               .Create();
          _mockDelivery.GenerateReportId(mockCustomer);
          _mockDelivery.TotalPrice = _mockPricingStrategyService.Object.CalculateDeliveryPriceWithoutVat(_mockDelivery);
          
          _mockCommand = _fixture.Build<AddDeliveryStepCommand>().Create();

          _mockDeliveryRepository
               .Setup(x => x.FirstOrDefaultAsync(
                    It.Is<ISpecification<Delivery>>(s => s is DeliveryByIdSpecification),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(_mockDelivery);
     }

     private AddDeliveryStepHandler CreateSut()
     {
          var validator = new AddDeliveryStepCommandValidator();
          return new AddDeliveryStepHandler(
               validator,
               _mockTransaction.Object,
               _mockDeliveryRepository.Object,
               _mockDeliveryZoneRepository.Object,
               _mockPricingStrategyService.Object,
               _mockItineraryApi.Object
          );
     }
     
     [Fact]
     public async Task Handle_ShouldCreateStepAndAddItToDelivery_WhenCommandIsValid()
     {
          // Arrange
          var sut = CreateSut();

          // Act
          var result = await sut.Handle(_mockCommand, CancellationToken.None);

          // Assert
          result.IsSuccess.Should().BeTrue();
          _mockDelivery.Steps[0].Id.Should().Be(result.Value);
          _mockTransaction.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
     }
     
     [Fact]
     public async Task Handle_ShouldReturnNotFound_WhenDeliveryDoesNotExit()
     {
          // Arrange
          Delivery? badDelivery = null;
          _mockDeliveryRepository
               .Setup(x => x.FirstOrDefaultAsync(
                    It.Is<ISpecification<Delivery>>(s => s is DeliveryByIdSpecification),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(badDelivery);
          var sut = CreateSut();

          // Act
          var result = await sut.Handle(_mockCommand, CancellationToken.None);

          // Assert
          result.IsSuccess.Should().BeFalse();
          _mockTransaction.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
     }

     [Fact]
     public void Handle_ShouldThrowValidationException_WhenDeliveryIdIsEmpty()
     {

     }

     [Fact]
     public void Handle_ShouldThrowValidationException_WhenStepTypeIsInvalid()
     {

     }

     [Fact]
     public void Handle_ShouldThrowValidationException_WhenStepAddressIsEmpty()
     {

     }
}
