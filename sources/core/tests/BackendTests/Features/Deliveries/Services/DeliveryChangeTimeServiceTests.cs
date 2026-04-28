using Ardalis.Specification;
using AutoFixture;
using AwesomeAssertions;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.SPI;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Specification;
using Ggio.BikeSherpa.Backend.Domain.SharedKernel;
using Ggio.BikeSherpa.Backend.Features.Deliveries.Services;
using Ggio.DddCore;
using Moq;

namespace BackendTests.Features.Deliveries.Services;

public class DeliveryChangeTimeServiceTests
{
     private readonly Mock<IReadRepository<Delivery>> _mockDeliveryRepository = new();
     private readonly Mock<IApplicationTransaction> _mockTransaction = new();
     private readonly Mock<IItinerarySpi> _mockItineraryService = new();
     private readonly Fixture _fixture = new();

     private readonly Guid _courierId = Guid.NewGuid();

     public DeliveryChangeTimeServiceTests()
     {
          _mockItineraryService
               .Setup(x => x.GetItineraryInfoAsync(
                    It.IsAny<GeoPoint>(),
                    It.IsAny<GeoPoint>(),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(new ItineraryResult(12.3, 45));
     }

     private DeliveryChangeTimeService CreateSut()
     {
          return new DeliveryChangeTimeService(
               _mockDeliveryRepository.Object,
               _mockTransaction.Object,
               _mockItineraryService.Object
          );
     }

     private DeliveryStep CreateStep(int order, DateTimeOffset estimatedDeliveryDate, Guid? courierId = null)
     {
          var address = _fixture
               .Build<Address>()
               .With(a => a.Postcode, "38000")
               .With(a => a.City, "Grenoble")
               .Create();

          return _fixture
               .Build<DeliveryStep>()
               .With(s => s.Order, order)
               .With(s => s.CourierId, courierId)
               .With(s => s.EstimatedDeliveryDate, estimatedDeliveryDate)
               .With(s => s.RealDeliveryDate, (DateTimeOffset?)null)
               .With(s => s.CreatedAt, DateTimeOffset.UtcNow)
               .With(s => s.UpdatedAt, DateTimeOffset.UtcNow)
               .With(s => s.StepAddress, address)
               .Create();
     }

     private Delivery CreateDelivery(params DeliveryStep[] steps)
     {
          return _fixture
               .Build<Delivery>()
               .With(d => d.Steps, steps.ToList())
               .With(d => d.ContractDate, DateTimeOffset.UtcNow)
               .With(d => d.StartDate, DateTimeOffset.UtcNow)
               .With(d => d.CreatedAt, DateTimeOffset.UtcNow)
               .With(d => d.UpdatedAt, DateTimeOffset.UtcNow)
               .Create();
     }

     private void SetupRepositoryDeliveries(params Delivery[] deliveries)
     {
          _mockDeliveryRepository
               .Setup(x => x.ListAsync(
                    It.Is<ISpecification<Delivery>>(s => s is DeliveryStepByCourierAndDate),
                    It.IsAny<CancellationToken>()))
               .ReturnsAsync(deliveries.ToList());
     }

     [Fact]
     public async Task ChangeTime_ShouldUpdateRequestedStepTime()
     {
          // Arrange
          var initialDate = DateTimeOffset.UtcNow.Date.AddHours(10);
          var newDate = initialDate.AddMinutes(30);

          var step = CreateStep(1, initialDate, _courierId);
          var delivery = CreateDelivery(step);

          SetupRepositoryDeliveries(delivery);

          var sut = CreateSut();

          // Act
          await sut.ChangeTime(delivery, step, newDate, CancellationToken.None);

          // Assert
          step.EstimatedDeliveryDate.Should().BeCloseTo(newDate, TimeSpan.FromSeconds(1));

          _mockTransaction.Verify(
               x => x.CommitAsync(It.IsAny<CancellationToken>()),
               Times.Once);
     }

     [Fact]
     public async Task ChangeTime_ShouldShiftFollowingSteps_WhenNewTimeDoesNotDisruptOrder()
     {
          // Arrange
          var date = DateTimeOffset.UtcNow.Date;

          var firstStep = CreateStep(1, date.AddHours(10), _courierId);
          var secondStep = CreateStep(2, date.AddHours(11), _courierId);
          var thirdStep = CreateStep(3, date.AddHours(12), _courierId);

          var delivery = CreateDelivery(firstStep, secondStep, thirdStep);

          SetupRepositoryDeliveries(delivery);

          var newSecondStepDate = secondStep.EstimatedDeliveryDate.AddMinutes(30);

          var sut = CreateSut();

          // Act
          await sut.ChangeTime(delivery, secondStep, newSecondStepDate, CancellationToken.None);

          // Assert
          firstStep.EstimatedDeliveryDate.Should().BeCloseTo(date.AddHours(10), TimeSpan.FromSeconds(1));
          secondStep.EstimatedDeliveryDate.Should().BeCloseTo(date.AddHours(11).AddMinutes(30), TimeSpan.FromSeconds(1));
          thirdStep.EstimatedDeliveryDate.Should().BeCloseTo(date.AddHours(12).AddMinutes(30), TimeSpan.FromSeconds(1));

          _mockTransaction.Verify(
               x => x.CommitAsync(It.IsAny<CancellationToken>()),
               Times.Once);
     }

     [Fact]
     public async Task ChangeTime_ShouldOnlyUpdateRequestedStep_WhenNewTimeDisruptsPreviousStepOrder()
     {
          // Arrange
          var date = DateTimeOffset.UtcNow.Date;

          var firstStep = CreateStep(1, date.AddHours(9), _courierId);
          var secondStep = CreateStep(2, date.AddHours(10), _courierId);
          var thirdStep = CreateStep(3, date.AddHours(11), _courierId);
          var fourthStep = CreateStep(4, date.AddHours(12), _courierId);

          var delivery = CreateDelivery(firstStep, secondStep, thirdStep, fourthStep);

          SetupRepositoryDeliveries(delivery);

          var newThirdStepDate = date.AddHours(9).AddMinutes(30);

          var sut = CreateSut();

          // Act
          await sut.ChangeTime(delivery, thirdStep, newThirdStepDate, CancellationToken.None);

          // Assert
          firstStep.EstimatedDeliveryDate.Should().BeCloseTo(date.AddHours(9), TimeSpan.FromSeconds(1));
          secondStep.EstimatedDeliveryDate.Should().BeCloseTo(date.AddHours(10), TimeSpan.FromSeconds(1));
          thirdStep.EstimatedDeliveryDate.Should().BeCloseTo(newThirdStepDate, TimeSpan.FromSeconds(1));
          fourthStep.EstimatedDeliveryDate.Should().BeCloseTo(date.AddHours(12), TimeSpan.FromSeconds(1));

          _mockTransaction.Verify(
               x => x.CommitAsync(It.IsAny<CancellationToken>()),
               Times.Once);
     }

     [Fact]
     public async Task ChangeTime_ShouldDoNothing_WhenStepHasNoCourier()
     {
          // Arrange
          var initialDate = DateTimeOffset.UtcNow.Date.AddHours(10);
          var newDate = initialDate.AddHours(1);

          var step = CreateStep(1, initialDate);
          var delivery = CreateDelivery(step);

          var sut = CreateSut();

          // Act
          await sut.ChangeTime(delivery, step, newDate, CancellationToken.None);

          // Assert
          step.EstimatedDeliveryDate.Should().BeCloseTo(initialDate, TimeSpan.FromSeconds(1));

          _mockDeliveryRepository.Verify(
               x => x.ListAsync(
                    It.IsAny<ISpecification<Delivery>>(),
                    It.IsAny<CancellationToken>()),
               Times.Never);

          _mockTransaction.Verify(
               x => x.CommitAsync(It.IsAny<CancellationToken>()),
               Times.Never);
     }

     [Fact]
     public async Task ChangeTime_ShouldDoNothing_WhenRepositoryReturnsNoDeliveries()
     {
          // Arrange
          var initialDate = DateTimeOffset.UtcNow.Date.AddHours(10);
          var newDate = initialDate.AddHours(1);

          var step = CreateStep(1, initialDate, _courierId);
          var delivery = CreateDelivery(step);

          SetupRepositoryDeliveries();

          var sut = CreateSut();

          // Act
          await sut.ChangeTime(delivery, step, newDate, CancellationToken.None);

          // Assert
          step.EstimatedDeliveryDate.Should().BeCloseTo(initialDate, TimeSpan.FromSeconds(1));

          _mockDeliveryRepository.Verify(
               x => x.ListAsync(
                    It.Is<ISpecification<Delivery>>(s => s is DeliveryStepByCourierAndDate),
                    It.IsAny<CancellationToken>()),
               Times.Once);

          _mockTransaction.Verify(
               x => x.CommitAsync(It.IsAny<CancellationToken>()),
               Times.Never);
     }

     [Fact]
     public async Task ChangeTime_ShouldDoNothing_WhenRepositoryReturnsDeliveriesWithoutRequestedStep()
     {
          // Arrange
          var date = DateTimeOffset.UtcNow.Date;

          var requestedStep = CreateStep(1, date.AddHours(10), _courierId);
          var requestedDelivery = CreateDelivery(requestedStep);

          var otherStep = CreateStep(1, date.AddHours(11), _courierId);
          var otherDelivery = CreateDelivery(otherStep);

          var newDate = requestedStep.EstimatedDeliveryDate.AddHours(1);

          SetupRepositoryDeliveries(otherDelivery);

          var sut = CreateSut();

          // Act
          await sut.ChangeTime(requestedDelivery, requestedStep, newDate, CancellationToken.None);

          // Assert
          requestedStep.EstimatedDeliveryDate.Should().BeCloseTo(date.AddHours(10), TimeSpan.FromSeconds(1));
          otherStep.EstimatedDeliveryDate.Should().BeCloseTo(date.AddHours(11), TimeSpan.FromSeconds(1));

          _mockTransaction.Verify(
               x => x.CommitAsync(It.IsAny<CancellationToken>()),
               Times.Never);
     }

     [Fact]
     public async Task ChangeOrder_ShouldReorderSteps_WhenTargetStepIsInSameDelivery()
     {
          // Arrange
          var date = DateTimeOffset.UtcNow.Date;

          var firstStep = CreateStep(1, date.AddHours(10), _courierId);
          var secondStep = CreateStep(2, date.AddHours(11), _courierId);
          var thirdStep = CreateStep(3, date.AddHours(12), _courierId);

          var delivery = CreateDelivery(firstStep, secondStep, thirdStep);

          SetupRepositoryDeliveries(delivery);

          var sut = CreateSut();

          // Act
          await sut.ChangeOrder(delivery, firstStep, 1, CancellationToken.None);

          // Assert
          var orderedSteps = delivery.Steps.OrderBy(s => s.Order).ToList();

          orderedSteps.Should().HaveCount(3);
          orderedSteps[0].Id.Should().Be(secondStep.Id);
          orderedSteps[1].Id.Should().Be(firstStep.Id);
          orderedSteps[2].Id.Should().Be(thirdStep.Id);

          _mockItineraryService.Verify(
               x => x.GetItineraryInfoAsync(
                    It.IsAny<GeoPoint>(),
                    It.IsAny<GeoPoint>(),
                    It.IsAny<CancellationToken>()),
               Times.AtLeastOnce);

          _mockTransaction.Verify(
               x => x.CommitAsync(It.IsAny<CancellationToken>()),
               Times.Once);
     }

     [Fact]
     public async Task ChangeOrder_ShouldReorderStepsBackward_WhenTargetStepIsInSameDelivery()
     {
          // Arrange
          var date = DateTimeOffset.UtcNow.Date;

          var firstStep = CreateStep(1, date.AddHours(10), _courierId);
          var secondStep = CreateStep(2, date.AddHours(11), _courierId);
          var thirdStep = CreateStep(3, date.AddHours(12), _courierId);

          var delivery = CreateDelivery(firstStep, secondStep, thirdStep);

          SetupRepositoryDeliveries(delivery);

          var sut = CreateSut();

          // Act
          await sut.ChangeOrder(delivery, thirdStep, -1, CancellationToken.None);

          // Assert
          var orderedSteps = delivery.Steps.OrderBy(s => s.Order).ToList();

          orderedSteps.Should().HaveCount(3);
          orderedSteps[0].Id.Should().Be(firstStep.Id);
          orderedSteps[1].Id.Should().Be(thirdStep.Id);
          orderedSteps[2].Id.Should().Be(secondStep.Id);

          _mockItineraryService.Verify(
               x => x.GetItineraryInfoAsync(
                    It.IsAny<GeoPoint>(),
                    It.IsAny<GeoPoint>(),
                    It.IsAny<CancellationToken>()),
               Times.AtLeastOnce);

          _mockTransaction.Verify(
               x => x.CommitAsync(It.IsAny<CancellationToken>()),
               Times.Once);
     }

     [Fact]
     public async Task ChangeOrder_ShouldSwapTimes_WhenTargetStepIsInAnotherDelivery()
     {
          // Arrange
          var date = DateTimeOffset.UtcNow.Date;

          var firstDeliveryStep = CreateStep(1, date.AddHours(10), _courierId);
          var secondDeliveryStep = CreateStep(1, date.AddHours(11), _courierId);

          var firstDelivery = CreateDelivery(firstDeliveryStep);
          var secondDelivery = CreateDelivery(secondDeliveryStep);

          SetupRepositoryDeliveries(firstDelivery, secondDelivery);

          var sut = CreateSut();

          // Act
          await sut.ChangeOrder(firstDelivery, firstDeliveryStep, 1, CancellationToken.None);

          // Assert
          firstDeliveryStep.EstimatedDeliveryDate.Should().BeCloseTo(date.AddHours(11), TimeSpan.FromSeconds(1));
          secondDeliveryStep.EstimatedDeliveryDate.Should().BeCloseTo(date.AddHours(10), TimeSpan.FromSeconds(1));

          _mockItineraryService.Verify(
               x => x.GetItineraryInfoAsync(
                    It.IsAny<GeoPoint>(),
                    It.IsAny<GeoPoint>(),
                    It.IsAny<CancellationToken>()),
               Times.Never);

          _mockTransaction.Verify(
               x => x.CommitAsync(It.IsAny<CancellationToken>()),
               Times.Once);
     }

     [Fact]
     public async Task ChangeOrder_ShouldSwapTimesBackward_WhenTargetStepIsInAnotherDelivery()
     {
          // Arrange
          var date = DateTimeOffset.UtcNow.Date;

          var firstDeliveryStep = CreateStep(1, date.AddHours(10), _courierId);
          var secondDeliveryStep = CreateStep(1, date.AddHours(11), _courierId);

          var firstDelivery = CreateDelivery(firstDeliveryStep);
          var secondDelivery = CreateDelivery(secondDeliveryStep);

          SetupRepositoryDeliveries(firstDelivery, secondDelivery);

          var sut = CreateSut();

          // Act
          await sut.ChangeOrder(secondDelivery, secondDeliveryStep, -1, CancellationToken.None);

          // Assert
          firstDeliveryStep.EstimatedDeliveryDate.Should().BeCloseTo(date.AddHours(11), TimeSpan.FromSeconds(1));
          secondDeliveryStep.EstimatedDeliveryDate.Should().BeCloseTo(date.AddHours(10), TimeSpan.FromSeconds(1));

          _mockItineraryService.Verify(
               x => x.GetItineraryInfoAsync(
                    It.IsAny<GeoPoint>(),
                    It.IsAny<GeoPoint>(),
                    It.IsAny<CancellationToken>()),
               Times.Never);

          _mockTransaction.Verify(
               x => x.CommitAsync(It.IsAny<CancellationToken>()),
               Times.Once);
     }

     [Fact]
     public async Task ChangeOrder_ShouldDoNothing_WhenIncrementMovesBeforeFirstStep()
     {
          // Arrange
          var date = DateTimeOffset.UtcNow.Date;

          var firstStep = CreateStep(1, date.AddHours(10), _courierId);
          var secondStep = CreateStep(2, date.AddHours(11), _courierId);

          var delivery = CreateDelivery(firstStep, secondStep);

          SetupRepositoryDeliveries(delivery);

          var sut = CreateSut();

          // Act
          await sut.ChangeOrder(delivery, firstStep, -1, CancellationToken.None);

          // Assert
          firstStep.Order.Should().Be(1);
          secondStep.Order.Should().Be(2);

          firstStep.EstimatedDeliveryDate.Should().BeCloseTo(date.AddHours(10), TimeSpan.FromSeconds(1));
          secondStep.EstimatedDeliveryDate.Should().BeCloseTo(date.AddHours(11), TimeSpan.FromSeconds(1));

          _mockItineraryService.Verify(
               x => x.GetItineraryInfoAsync(
                    It.IsAny<GeoPoint>(),
                    It.IsAny<GeoPoint>(),
                    It.IsAny<CancellationToken>()),
               Times.Never);

          _mockTransaction.Verify(
               x => x.CommitAsync(It.IsAny<CancellationToken>()),
               Times.Never);
     }

     [Fact]
     public async Task ChangeOrder_ShouldDoNothing_WhenIncrementMovesAfterLastStep()
     {
          // Arrange
          var date = DateTimeOffset.UtcNow.Date;

          var firstStep = CreateStep(1, date.AddHours(10), _courierId);
          var secondStep = CreateStep(2, date.AddHours(11), _courierId);

          var delivery = CreateDelivery(firstStep, secondStep);

          SetupRepositoryDeliveries(delivery);

          var sut = CreateSut();

          // Act
          await sut.ChangeOrder(delivery, secondStep, 1, CancellationToken.None);

          // Assert
          firstStep.Order.Should().Be(1);
          secondStep.Order.Should().Be(2);

          firstStep.EstimatedDeliveryDate.Should().BeCloseTo(date.AddHours(10), TimeSpan.FromSeconds(1));
          secondStep.EstimatedDeliveryDate.Should().BeCloseTo(date.AddHours(11), TimeSpan.FromSeconds(1));

          _mockItineraryService.Verify(
               x => x.GetItineraryInfoAsync(
                    It.IsAny<GeoPoint>(),
                    It.IsAny<GeoPoint>(),
                    It.IsAny<CancellationToken>()),
               Times.Never);

          _mockTransaction.Verify(
               x => x.CommitAsync(It.IsAny<CancellationToken>()),
               Times.Never);
     }

     [Fact]
     public async Task ChangeOrder_ShouldDoNothing_WhenStepHasNoCourier()
     {
          // Arrange
          var date = DateTimeOffset.UtcNow.Date;

          var firstStep = CreateStep(1, date.AddHours(10));
          var secondStep = CreateStep(2, date.AddHours(11));

          var delivery = CreateDelivery(firstStep, secondStep);

          var sut = CreateSut();

          // Act
          await sut.ChangeOrder(delivery, firstStep, 1, CancellationToken.None);

          // Assert
          firstStep.Order.Should().Be(1);
          secondStep.Order.Should().Be(2);

          firstStep.EstimatedDeliveryDate.Should().BeCloseTo(date.AddHours(10), TimeSpan.FromSeconds(1));
          secondStep.EstimatedDeliveryDate.Should().BeCloseTo(date.AddHours(11), TimeSpan.FromSeconds(1));

          _mockDeliveryRepository.Verify(
               x => x.ListAsync(
                    It.IsAny<ISpecification<Delivery>>(),
                    It.IsAny<CancellationToken>()),
               Times.Never);

          _mockItineraryService.Verify(
               x => x.GetItineraryInfoAsync(
                    It.IsAny<GeoPoint>(),
                    It.IsAny<GeoPoint>(),
                    It.IsAny<CancellationToken>()),
               Times.Never);

          _mockTransaction.Verify(
               x => x.CommitAsync(It.IsAny<CancellationToken>()),
               Times.Never);
     }

     [Fact]
     public async Task ChangeOrder_ShouldDoNothing_WhenRepositoryReturnsNoDeliveries()
     {
          // Arrange
          var date = DateTimeOffset.UtcNow.Date;

          var firstStep = CreateStep(1, date.AddHours(10), _courierId);
          var secondStep = CreateStep(2, date.AddHours(11), _courierId);

          var delivery = CreateDelivery(firstStep, secondStep);

          SetupRepositoryDeliveries();

          var sut = CreateSut();

          // Act
          await sut.ChangeOrder(delivery, firstStep, 1, CancellationToken.None);

          // Assert
          firstStep.Order.Should().Be(1);
          secondStep.Order.Should().Be(2);

          firstStep.EstimatedDeliveryDate.Should().BeCloseTo(date.AddHours(10), TimeSpan.FromSeconds(1));
          secondStep.EstimatedDeliveryDate.Should().BeCloseTo(date.AddHours(11), TimeSpan.FromSeconds(1));

          _mockItineraryService.Verify(
               x => x.GetItineraryInfoAsync(
                    It.IsAny<GeoPoint>(),
                    It.IsAny<GeoPoint>(),
                    It.IsAny<CancellationToken>()),
               Times.Never);

          _mockTransaction.Verify(
               x => x.CommitAsync(It.IsAny<CancellationToken>()),
               Times.Never);
     }

     [Fact]
     public async Task ChangeOrder_ShouldDoNothing_WhenRepositoryReturnsDeliveriesWithoutRequestedStep()
     {
          // Arrange
          var date = DateTimeOffset.UtcNow.Date;

          var requestedStep = CreateStep(1, date.AddHours(10), _courierId);
          var requestedDelivery = CreateDelivery(requestedStep);

          var otherStep = CreateStep(1, date.AddHours(11), _courierId);
          var otherDelivery = CreateDelivery(otherStep);

          SetupRepositoryDeliveries(otherDelivery);

          var sut = CreateSut();

          // Act
          await sut.ChangeOrder(requestedDelivery, requestedStep, 1, CancellationToken.None);

          // Assert
          requestedStep.Order.Should().Be(1);
          requestedStep.EstimatedDeliveryDate.Should().BeCloseTo(date.AddHours(10), TimeSpan.FromSeconds(1));
          otherStep.EstimatedDeliveryDate.Should().BeCloseTo(date.AddHours(11), TimeSpan.FromSeconds(1));

          _mockItineraryService.Verify(
               x => x.GetItineraryInfoAsync(
                    It.IsAny<GeoPoint>(),
                    It.IsAny<GeoPoint>(),
                    It.IsAny<CancellationToken>()),
               Times.Never);

          _mockTransaction.Verify(
               x => x.CommitAsync(It.IsAny<CancellationToken>()),
               Times.Never);
     }

     [Fact]
     public async Task ChangeTime_ShouldIgnoreStepsFromAnotherDate()
     {
          // Arrange
          var date = DateTimeOffset.UtcNow.Date;

          var requestedStep = CreateStep(1, date.AddHours(10), _courierId);
          var sameCourierOtherDateStep = CreateStep(1, date.AddDays(1).AddHours(10), _courierId);

          var requestedDelivery = CreateDelivery(requestedStep);
          var otherDelivery = CreateDelivery(sameCourierOtherDateStep);

          SetupRepositoryDeliveries(requestedDelivery, otherDelivery);

          var newDate = requestedStep.EstimatedDeliveryDate.AddHours(1);

          var sut = CreateSut();

          // Act
          await sut.ChangeTime(requestedDelivery, requestedStep, newDate, CancellationToken.None);

          // Assert
          requestedStep.EstimatedDeliveryDate.Should().BeCloseTo(newDate, TimeSpan.FromSeconds(1));
          sameCourierOtherDateStep.EstimatedDeliveryDate.Should().BeCloseTo(date.AddDays(1).AddHours(10), TimeSpan.FromSeconds(1));

          _mockTransaction.Verify(
               x => x.CommitAsync(It.IsAny<CancellationToken>()),
               Times.Once);
     }

     [Fact]
     public async Task ChangeTime_ShouldIgnoreStepsFromAnotherCourier()
     {
          // Arrange
          var date = DateTimeOffset.UtcNow.Date;
          var otherCourierId = Guid.NewGuid();

          var requestedStep = CreateStep(1, date.AddHours(10), _courierId);
          var otherCourierStep = CreateStep(2, date.AddHours(11), otherCourierId);

          var delivery = CreateDelivery(requestedStep, otherCourierStep);

          SetupRepositoryDeliveries(delivery);

          var newDate = requestedStep.EstimatedDeliveryDate.AddHours(1);

          var sut = CreateSut();

          // Act
          await sut.ChangeTime(delivery, requestedStep, newDate, CancellationToken.None);

          // Assert
          requestedStep.EstimatedDeliveryDate.Should().BeCloseTo(newDate, TimeSpan.FromSeconds(1));
          otherCourierStep.EstimatedDeliveryDate.Should().BeCloseTo(date.AddHours(11), TimeSpan.FromSeconds(1));

          _mockTransaction.Verify(
               x => x.CommitAsync(It.IsAny<CancellationToken>()),
               Times.Once);
     }
}