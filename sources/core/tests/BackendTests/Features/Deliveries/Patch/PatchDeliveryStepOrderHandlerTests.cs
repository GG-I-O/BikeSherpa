using Ardalis.Result;
using Ardalis.Specification;
using AutoFixture;
using AutoFixture.AutoMoq;
using AwesomeAssertions;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Specification;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Spi;
using Ggio.BikeSherpa.Backend.Features.Deliveries.Patch;
using Ggio.DddCore;
using Moq;

namespace BackendTests.Features.Deliveries.Patch;

public class PatchDeliveryStepOrderHandlerTests
{
     private readonly Guid _deliveryId;
     private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());
     private readonly Mock<IReadRepository<Delivery>> _mockDeliveryRepository = new();
     private readonly Mock<IItinerarySpi> _mockItineraryService = new();
     private readonly Mock<IApplicationTransaction> _mockTransaction = new();
     private readonly Guid _stepId;

     public PatchDeliveryStepOrderHandlerTests()
     {
          _deliveryId = Guid.NewGuid();

          var mockDelivery = _fixture.Build<Delivery>()
               .With(d => d.Id, _deliveryId)
               .With(d => d.Steps, [])
               .Create();

          var mockSteps = _fixture.Build<DeliveryStep>()
               .With(s => s.ParentDelivery, mockDelivery)
               .CreateMany(5)
               .ToList();

          mockDelivery.Steps.AddRange(mockSteps);
          _stepId = mockDelivery.Steps.First().Id;

          _mockDeliveryRepository
               .Setup(x => x.FirstOrDefaultAsync(
                    It.Is<ISpecification<Delivery>>(s => s is DeliveryByIdSpecification),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(mockDelivery);

          _mockItineraryService
               .Setup(x => x.GetItineraryInfoAsync(
                    It.IsAny<GeoPoint>(),
                    It.IsAny<GeoPoint>(),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(new ItineraryResult(12.3, 45));
     }

     private PatchDeliveryStepOrderHandler CreateSut() => new(
          _mockDeliveryRepository.Object,
          _mockTransaction.Object,
          _mockItineraryService.Object
     );

     [Fact]
     public async Task Handle_ShouldReturnSuccess_WhenRequestIsValid()
     {
          // Arrange
          var sut = CreateSut();
          var command = new PatchDeliveryStepOrderCommand(_deliveryId, _stepId, 4);

          // Act
          var result = await sut.Handle(command, CancellationToken.None);

          // Assert
          result.IsSuccess.Should().BeTrue();
          _mockTransaction.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
     }

     [Fact]
     public async Task Handle_ShouldReturnNotFound_WhenDeliveryDoesNotExist()
     {
          // Arrange
          _mockDeliveryRepository
               .Setup(x => x.FirstOrDefaultAsync(
                    It.Is<ISpecification<Delivery>>(s => s is DeliveryByIdSpecification),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(null as Delivery);

          var sut = CreateSut();
          var command = new PatchDeliveryStepOrderCommand(_deliveryId, _stepId, 4);

          // Act
          var result = await sut.Handle(command, CancellationToken.None);

          // Assert
          result.IsSuccess.Should().BeFalse();
          result.IsNotFound().Should().BeTrue();
          _mockTransaction.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
     }
}
