using AutoFixture;
using AutoFixture.AutoMoq;
using AwesomeAssertions;
using Ggio.BikeSherpa.Backend.Domain.CustomerAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Services.PricingStrategy;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Services.Repositories;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.SPI;
using Ggio.BikeSherpa.Backend.Domain.SharedKernel;
using Moq;

namespace Backend.Domain.Tests.DeliveryAggregate;

public class DeliveryTests
{
    private readonly Mock<IPricingStrategyService> _pricingStrategyServiceMock = new();
    private readonly Mock<IItinerarySpi> _mockItineraryService = new();
    private readonly IFixture _fixture = new Fixture().Customize(new AutoMoqCustomization());

    private Delivery MakeSut()
    {
        var delivery = _fixture.Create<Delivery>();
        delivery.Steps = [];
        return delivery;
    }

    private DeliveryStep CreatePickupStep(bool completed = false)
    {
        var deliveryStep = _fixture.Create<DeliveryStep>();
        deliveryStep.Id = Guid.NewGuid();
        deliveryStep.StepType = StepType.Pickup;
        deliveryStep.Completed = completed;
        deliveryStep.StepAddress.Coordinates = new GeoPoint(1, 2);

        return deliveryStep;
    }

    private DeliveryStep CreateDropoffStep(bool completed = false)
    {
        var deliveryStep = _fixture.Create<DeliveryStep>();
        deliveryStep.Id = Guid.NewGuid();
        deliveryStep.StepType = StepType.Dropoff;
        deliveryStep.Completed = completed;
        deliveryStep.StepAddress.Coordinates = new GeoPoint(3, 4);

        return deliveryStep;
    }

    [Fact]
    public void GenerateReportId_ReturnsIdStartingWithCustomerCode()
    {
        // Arrange
        var delivery = MakeSut();
        var customer = _fixture.Create<Customer>();

        // Act
        delivery.GenerateReportId(customer);

        // Assert
        delivery.ReportId.Should().StartWith($"{customer.Code}-");
    }

    [Fact]
    public void GenerateReportId_TimestampPartHasExpectedLength()
    {
        // Arrange
        var delivery = MakeSut();
        var customer = _fixture.Create<Customer>();
        customer.Code = "TEST";

        // Act
        delivery.GenerateReportId(customer);

        // Assert
        // ReportId format = CUSTOMERCODE-yyyyMMddHHmmss (14-character timestamp)
        var timestamp = delivery.ReportId![("TEST".Length + 1)..];
        timestamp.Should().HaveLength(14);
    }

    [Fact]
    public void GenerateReportId_DifferentCustomerCodes_ProduceDifferentPrefixes()
    {
        // Arrange
        var delivery = MakeSut();
        var customers = _fixture.CreateMany<Customer>(2).ToList();
        customers[0].Code = "TEST1";
        customers[1].Code = "TEST2";

        // Act
        delivery.GenerateReportId(customers[0]);
        var id1 = delivery.ReportId;
        delivery.GenerateReportId(customers[1]);
        var id2 = delivery.ReportId;

        // Assert
        id1.Should().StartWith("TEST1-");
        id2.Should().StartWith("TEST2-");
    }

    [Fact]
    public void UpdateDeliveryStartDateTime_SetsNewStartDateAndRecalculatesTotalPrice()
    {
        // Arrange
        var delivery = MakeSut();
        var newDate = _fixture.Create<DateTimeOffset>();

        // Act
        delivery.UpdateDeliveryStartDateTime(newDate, _pricingStrategyServiceMock.Object);

        // Assert
        delivery.StartDate.Should().Be(newDate);
        _pricingStrategyServiceMock.Verify(p => p.CalculateDeliveryPriceWithoutVat(delivery), Times.Once);
    }

    [Fact]
    public async Task ReorderSteps_ReordersAllStepsSettingCorrectOrderForMovedStep()
    {
        // Arrange
        var delivery = MakeSut();
        var step1 = CreatePickupStep();
        var step2 = CreateDropoffStep();
        var step3 = CreateDropoffStep();
        step1.Order = 1;
        step2.Order = 2;
        step3.Order = 3;
        delivery.Steps.AddRange([step1, step2, step3]);
        var (_, _, _, mockItineraryService) = CreateStepDependencies(delivery);

        // Act
        await delivery.ReorderStepsAsync(step3.Id, 2, mockItineraryService.Object);

        // Assert
        delivery.Steps.Single(s => s.Id == step1.Id).Order.Should().Be(1);
        delivery.Steps.Single(s => s.Id == step3.Id).Order.Should().Be(2);
        delivery.Steps.Single(s => s.Id == step2.Id).Order.Should().Be(3);
    }

    [Fact]
    public async Task ReorderStepsAsync_WhenStepNotFound_Throws()
    {
        // Arrange
        var delivery = MakeSut();

        // Act
        var act = () => delivery.ReorderStepsAsync(Guid.NewGuid(), 1, _mockItineraryService.Object);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public void UpdateStepCourier_SetsNewCourierOnStep()
    {
        // Arrange
        var delivery = MakeSut();
        var step = CreatePickupStep();
        delivery.Steps.Add(step);
        var courierId = Guid.NewGuid();

        // Act
        delivery.UpdateStepCourier(step.Id, courierId);

        // Assert
        delivery.Steps.Single(s => s.Id == step.Id).CourierId.Should().Be(courierId);
    }

    [Fact]
    public void UpdateStepCourier_WhenStepNotFound_Throws()
    {
        // Arrange
        var delivery = MakeSut();
        // Act
        var act = () => delivery.UpdateStepCourier(Guid.NewGuid(), Guid.NewGuid());

        // Assert
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void UpdateStepCompletion_MarksStepAsCompletedAndSetsRealDeliveryDate()
    {
        // Arrange
        var delivery = MakeSut();
        var step = CreateDropoffStep();
        delivery.Steps.Add(step);

        // Act
        delivery.UpdateStepCompletion(step.Id, true);

        // Assert
        step.Completed.Should().BeTrue();
        step.RealDeliveryDate.Should().NotBe(null);
    }

    [Fact]
    public void UpdateStepCompletion_MarksStepAsNotCompleted()
    {
        // Arrange
        var delivery = MakeSut();
        var step = CreateDropoffStep(completed: true);
        delivery.Steps.Add(step);

        // Act
        delivery.UpdateStepCompletion(step.Id, false);

        // Assert
        step.Completed.Should().BeFalse();
    }

    [Fact]
    public void UpdateStepCompletion_WhenPickupStepCompleted_StartsDelivery()
    {
        // Arrange
        var delivery = MakeSut();
        var pickup = CreatePickupStep();
        delivery.Steps.Add(pickup);

        // Act
        delivery.UpdateStepCompletion(pickup.Id, true);

        // Assert
        delivery.Status.Should().Be(DeliveryStatus.Started);
    }

    [Fact]
    public void UpdateStepCompletion_WhenAllStepsCompleted_CompletesDelivery()
    {
        // Arrange
        var delivery = MakeSut();
        var pickup = CreatePickupStep(completed: true);
        var dropoff = CreateDropoffStep(completed: false);
        delivery.Steps.AddRange([pickup, dropoff]);
        delivery.Status = DeliveryStatus.Started;

        // Act
        delivery.UpdateStepCompletion(dropoff.Id, true);

        // Assert
        delivery.Status.Should().Be(DeliveryStatus.Completed);
    }

    [Fact]
    public void UpdateStepCompletion_WhenDeliveryIsAlreadyCompleted_Throws()
    {
        // Arrange
        var delivery = MakeSut();
        var step = CreatePickupStep();
        delivery.Steps.Add(step);
        delivery.Status = DeliveryStatus.Completed;

        // Act
        var act = () => delivery.UpdateStepCompletion(step.Id, true);

        // Assert
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void UpdateStepCompletion_WhenDeliveryIsCancelled_Throws()
    {
        // Arrange
        var delivery = MakeSut();
        var step = CreatePickupStep();
        delivery.Steps.Add(step);
        delivery.Status = DeliveryStatus.Cancelled;

        // Act
        var act = () => delivery.UpdateStepCompletion(step.Id, true);

        // Assert
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Cancel_WhenPending_ChangesStatusToCancelled()
    {
        // Arrange
        var delivery = MakeSut();

        // Act
        delivery.Cancel();

        // Assert
        delivery.Status.Should().Be(DeliveryStatus.Cancelled);
    }

    [Fact]
    public void Cancel_WhenStarted_ChangesStatusToCancelled()
    {
        // Arrange
        var delivery = MakeSut();
        delivery.Status = DeliveryStatus.Started;

        // Act
        delivery.Cancel();

        // Assert
        delivery.Status.Should().Be(DeliveryStatus.Cancelled);
    }

    [Fact]
    public async Task AddStep_AddsStepToStepsList()
    {
        // Arrange
        var delivery = MakeSut();
        var (mockDeliveryZoneRepository, mockPricingStrategyService, address, mockItineraryService) = CreateStepDependencies(delivery);

        // Act
        await delivery.AddStepAsync(
            StepType.Pickup,
            address,
            "CommentPickup",
            mockDeliveryZoneRepository.Object,
            mockPricingStrategyService.Object,
            mockItineraryService.Object
            );
        await delivery.AddStepAsync(
            StepType.Dropoff,
            address,
            "CommentDropoff",
            mockDeliveryZoneRepository.Object,
            mockPricingStrategyService.Object,
            mockItineraryService.Object
            );

        // Assert
        delivery.Steps.Should().HaveCount(2);
        delivery.Steps[0].StepType.Should().Be(StepType.Pickup);
        delivery.Steps[0].Order.Should().Be(1);
        delivery.Steps[0].StepAddress.Should().Be(address);
        delivery.Steps[0].StepZone.Should().NotBe(null);
        delivery.Steps[0].Id.Should().NotBeEmpty();
        delivery.Steps[0].EstimatedDeliveryDate.Should().Be(delivery.StartDate);
        delivery.Steps[1].EstimatedDeliveryDate.Should().Be(delivery.StartDate.AddMinutes(15));
    }

    [Fact]
    public async Task AddStep_ReturnsCreatedStepWithCorrectProperties()
    {
        // Arrange
        var delivery = MakeSut();
        var (mockDeliveryZoneRepository, mockPricingStrategyService, address, mockItineraryService) = CreateStepDependencies(delivery);

        // Act
        var step = await delivery.AddStepAsync(
            StepType.Dropoff,
            address,
            "CommentDropoff",
            mockDeliveryZoneRepository.Object,
            mockPricingStrategyService.Object,
            mockItineraryService.Object);

        // Assert
        step.StepType.Should().Be(StepType.Dropoff);
        step.Order.Should().Be(1);
        step.Id.Should().NotBeEmpty();
    }

    [Fact]
    public async Task AddStep_RecalculatesTotalPrice()
    {
        // Arrange
        var delivery = MakeSut();
        var (mockDeliveryZoneRepository, mockPricingStrategyService, address, mockItineraryService) = CreateStepDependencies(delivery, calculatedPrice: 25.0);

        // Act
        await delivery.AddStepAsync(StepType.Pickup,
            address,
            "CommentPickup",
            mockDeliveryZoneRepository.Object,
            mockPricingStrategyService.Object,
            mockItineraryService.Object);

        // Assert
        delivery.TotalPrice.Should().Be(25.0);
    }

    [Fact]
    public async Task AddStep_LooksUpDeliveryZoneByAddressCity()
    {
        // Arrange
        var delivery = MakeSut();
        var (mockDeliveryZoneRepository, mockPricingStrategyService, address, mockItineraryService) = CreateStepDependencies(delivery);

        // Act
        await delivery.AddStepAsync(StepType.Pickup,
            address,
            "CommentPickup",
            mockDeliveryZoneRepository.Object,
            mockPricingStrategyService.Object,
            mockItineraryService.Object);

        // Assert
        mockDeliveryZoneRepository.Verify(r => r.GetByAddress(address.City), Times.Once);
    }

    [Fact]
    public async Task DeleteStep_RemovesStepFromStepsList()
    {
        // Arrange
        var delivery = MakeSut();
        var mockPricingStrategyService = new Mock<IPricingStrategyService>();
        mockPricingStrategyService.Setup(p => p.CalculateDeliveryPriceWithoutVat(delivery)).Returns(0.0);
        var step = CreatePickupStep();
        delivery.Steps.Add(step);

        // Act
        await delivery.DeleteStepAsync(step, mockPricingStrategyService.Object, _mockItineraryService.Object);

        // Assert
        delivery.Steps.Should().BeEmpty();
    }

    [Fact]
    public async Task DeleteStep_RecalculatesTotalPrice()
    {
        // Arrange
        var delivery = MakeSut();
        var mockPricingStrategyService = new Mock<IPricingStrategyService>();
        mockPricingStrategyService.Setup(p => p.CalculateDeliveryPriceWithoutVat(delivery)).Returns(0.0);
        var step = CreatePickupStep();
        delivery.Steps.Add(step);

        // Act
        await delivery.DeleteStepAsync(step, mockPricingStrategyService.Object, _mockItineraryService.Object);

        // Assert
        mockPricingStrategyService.Verify(p => p.CalculateDeliveryPriceWithoutVat(delivery), Times.Once);
    }

    [Fact]
    public async Task DeleteStep_OnlyRemovesTargetStep()
    {
        // Arrange
        var delivery = MakeSut();
        var mockPricingStrategyService = new Mock<IPricingStrategyService>();
        mockPricingStrategyService.Setup(p => p.CalculateDeliveryPriceWithoutVat(delivery)).Returns(0.0);
        var stepToKeep = CreatePickupStep();
        var stepToDelete = CreatePickupStep();
        delivery.Steps.AddRange([stepToKeep, stepToDelete]);

        // Act
        await delivery.DeleteStepAsync(stepToDelete, mockPricingStrategyService.Object, _mockItineraryService.Object);

        // Assert
        delivery.Steps.Should().HaveCount(1);
        delivery.Steps.Should().ContainSingle(s => s.Id == stepToKeep.Id);
    }

    [Fact]
    public async Task UpdateSteps_AddsNewStepsThatDontExistYet()
    {
        // Arrange
        var delivery = MakeSut();
        var (mockDeliveryZoneRepository, mockPricingStrategyService, mockItineraryService) = CreateUpdateStepsDependencies(delivery);
        var newStep = CreatePickupStep();

        // Act
        await delivery.UpdateStepsAsync([newStep], mockDeliveryZoneRepository.Object, mockPricingStrategyService.Object, mockItineraryService.Object);

        // Assert
        delivery.Steps.Should().ContainSingle(s => s.StepType == newStep.StepType && s.StepAddress == newStep.StepAddress);
    }

    [Fact]
    public async Task UpdateSteps_UpdatesExistingSteps()
    {
        // Arrange
        var delivery = MakeSut();
        var zone = _fixture.Create<DeliveryZone>();
        var (mockDeliveryZoneRepository, mockPricingStrategyService, mockItineraryService) = CreateUpdateStepsDependencies(delivery, zone);
        var existingStep = CreatePickupStep();
        delivery.Steps.Add(existingStep);

        // Act
        var updatedStep = new DeliveryStep(
            StepType.Dropoff,
            1,
            existingStep.StepAddress,
            comment: "NewComm"
            )
        {
            Id = existingStep.Id,
            StepAddress = existingStep.StepAddress,
            StepZone = zone
        };

        await delivery.UpdateStepsAsync(
            [updatedStep],
            mockDeliveryZoneRepository.Object,
            mockPricingStrategyService.Object,
            mockItineraryService.Object
            );

        // Assert
        delivery.Steps.Should().HaveCount(1);
        delivery.Steps[0].StepType.Should().Be(StepType.Dropoff);
        delivery.Steps[0].Order.Should().Be(1);
        delivery.Steps[0].Distance.Should().Be(0);
        delivery.Steps[0].StepZone.Should().Be(zone);
        delivery.Steps[0].Comment.Should().Be("NewComm");
    }
    
    [Fact]
        public async Task UpdateSteps_WhenMixingExistingAndNewSteps_ReordersAndKeepsExistingStep()
        {
            // Arrange
            var delivery = MakeSut();
            var zone = _fixture.Create<DeliveryZone>();
            var (mockDeliveryZoneRepository, mockPricingStrategyService, mockItineraryService) = CreateUpdateStepsDependencies(delivery, zone, calculatedPrice: 42.0);

            var existingStep = CreatePickupStep();
            existingStep.Order = 1;
            existingStep.EstimatedDeliveryDate = delivery.StartDate;
            delivery.Steps.Add(existingStep);

            var incomingExistingStep = new DeliveryStep(
                StepType.Dropoff,
                1,
                existingStep.StepAddress,
                comment: "Updated existing step")
            {
                Id = existingStep.Id,
                StepAddress = existingStep.StepAddress,
                StepZone = zone,
                Distance = 12.5,
                EstimatedDeliveryDate = existingStep.EstimatedDeliveryDate,
                Completed = true
            };

            var newStep = CreateDropoffStep();
            newStep.StepAddress.Coordinates = new GeoPoint(8, 9);

            // Act
            await delivery.UpdateStepsAsync(
                [incomingExistingStep, newStep],
                mockDeliveryZoneRepository.Object,
                mockPricingStrategyService.Object,
                mockItineraryService.Object);

            // Assert
            delivery.Steps.Should().HaveCount(2);

            var updatedExistingStep = delivery.Steps.Single(s => s.Id == existingStep.Id);
            updatedExistingStep.StepType.Should().Be(StepType.Dropoff);
            updatedExistingStep.Order.Should().Be(1);
            updatedExistingStep.Comment.Should().Be("Updated existing step");
            updatedExistingStep.Completed.Should().BeTrue();
            updatedExistingStep.StepZone.Should().Be(zone);
            updatedExistingStep.Distance.Should().Be(0);

            var createdStep = delivery.Steps.Single(s => s.Id != existingStep.Id);
            createdStep.StepType.Should().Be(StepType.Dropoff);
            createdStep.Order.Should().Be(2);
            createdStep.Comment.Should().Be(newStep.Comment);
            createdStep.StepZone.Should().NotBeNull();
            createdStep.Id.Should().NotBeEmpty();

            delivery.TotalPrice.Should().Be(42.0);
        }
    
    [Fact]
    public async Task UpdateSteps_WhenIncomingListIsAlreadyOrdered_ReassignsSequentialOrders()
    {
        // Arrange
        var delivery = MakeSut();
        var (mockDeliveryZoneRepository, mockPricingStrategyService, mockItineraryService) = CreateUpdateStepsDependencies(delivery);

        var step1 = CreatePickupStep();
        var step2 = CreateDropoffStep();
        var step3 = CreateDropoffStep();

        step1.Order = 99;
        step2.Order = 98;
        step3.Order = 97;

        // Act
        await delivery.UpdateStepsAsync([step1, step2, step3], mockDeliveryZoneRepository.Object, mockPricingStrategyService.Object, mockItineraryService.Object);

        // Assert
        delivery.Steps.Select(s => s.Order).Should().Equal(1, 2, 3);
        delivery.Steps[0].Distance.Should().Be(0);
        delivery.Steps[1].EstimatedDeliveryDate.Should().Be(delivery.Steps[0].EstimatedDeliveryDate.AddMinutes(15));
        delivery.Steps[2].EstimatedDeliveryDate.Should().Be(delivery.Steps[1].EstimatedDeliveryDate.AddMinutes(15));
    }

    [Fact]
    public async Task UpdateSteps_RemovesStepsNotInIncomingList()
    {
        // Arrange
        var delivery = MakeSut();
        var (mockDeliveryZoneRepository, mockPricingStrategyService, mockItineraryService) = CreateUpdateStepsDependencies(delivery);
        var stepToKeep = CreatePickupStep();
        var stepToRemove = CreatePickupStep();
        delivery.Steps.AddRange([stepToKeep, stepToRemove]);

        // Act
        await delivery.UpdateStepsAsync([stepToKeep], mockDeliveryZoneRepository.Object, mockPricingStrategyService.Object, mockItineraryService.Object);

        // Assert
        delivery.Steps.Should().HaveCount(1);
        delivery.Steps.Should().ContainSingle(s => s.Id == stepToKeep.Id);
        delivery.Steps.Should().NotContain(s => s.Id == stepToRemove.Id);
    }

    [Fact]
    public async Task UpdateSteps_WhenEmptyListProvided_RemovesAllExistingSteps()
    {
        // Arrange
        var delivery = MakeSut();
        var (mockDeliveryZoneRepository, mockPricingStrategyService, mockItineraryService) = CreateUpdateStepsDependencies(delivery);
        delivery.Steps.AddRange([CreatePickupStep(), CreatePickupStep()]);

        // Act
        await delivery.UpdateStepsAsync([], mockDeliveryZoneRepository.Object, mockPricingStrategyService.Object, mockItineraryService.Object);

        // Assert
        delivery.Steps.Should().BeEmpty();
    }

    [Fact]
    public async Task UpdateSteps_RecalculatesTotalPrice()
    {
        // Arrange
        var delivery = MakeSut();
        var (mockDeliveryZoneRepository, mockPricingStrategyService, mockItineraryService) = CreateUpdateStepsDependencies(delivery, calculatedPrice: 35.0);
        var step = CreatePickupStep();

        // Act
        await delivery.UpdateStepsAsync([step], mockDeliveryZoneRepository.Object, mockPricingStrategyService.Object, mockItineraryService.Object);

        // Assert
        delivery.TotalPrice.Should().Be(35.0);
    }

    [Fact]
    public void UpdateStepUpdateStepDeliveryTime_ChangesUpdatedStepAndNextStepsDeliveryTime()
    {
        // Arrange
        var delivery = MakeSut();
        var step1 = CreatePickupStep();
        var step2 = CreateDropoffStep();
        var step3 = CreateDropoffStep();
        var step4 = CreateDropoffStep();
        var startTime = DateTimeOffset.UtcNow;
        step1.Order = 1;
        step2.Order = 2;
        step3.Order = 3;
        step4.Order = 4;
        step1.EstimatedDeliveryDate = startTime;
        step2.EstimatedDeliveryDate = startTime.AddMinutes(15);
        step3.EstimatedDeliveryDate = startTime.AddMinutes(25);
        step4.EstimatedDeliveryDate = startTime.AddMinutes(35);
        delivery.Steps.AddRange([step1, step2, step3, step4]);

        // Act
        delivery.UpdateStepDeliveryTime(step2.Id, startTime.AddMinutes(20));

        // Assert
        step2.EstimatedDeliveryDate.Should().Be(startTime.AddMinutes(20));
        step3.EstimatedDeliveryDate.Should().Be(startTime.AddMinutes(30));
        step4.EstimatedDeliveryDate.Should().Be(startTime.AddMinutes(40));
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private (Mock<IDeliveryZoneRepository> mockDeliveryZoneRepository, Mock<IPricingStrategyService> mockPricingStrategyService, Address address, Mock<IItinerarySpi> mockItineraryService)
        CreateStepDependencies(Delivery delivery, double calculatedPrice = 15.0)
    {
        var mockDeliveryZoneRepository = new Mock<IDeliveryZoneRepository>();
        var mockPricingStrategyService = new Mock<IPricingStrategyService>();
        var mockItineraryService = new Mock<IItinerarySpi>();
        var address = _fixture.Create<Address>();
        address.Coordinates = new GeoPoint(5.2, 45.3);
        mockDeliveryZoneRepository.Setup(r => r.GetByAddress(address.City)).Returns(_fixture.Create<DeliveryZone>());
        mockPricingStrategyService.Setup(p => p.CalculateDeliveryPriceWithoutVat(delivery)).Returns(calculatedPrice);
        mockItineraryService.Setup(i => i.GetItineraryInfoAsync(
            It.IsAny<GeoPoint>(),
            It.IsAny<GeoPoint>(),
            It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ItineraryResult(10.0, 20.0));
        return (mockDeliveryZoneRepository, mockPricingStrategyService, address, mockItineraryService);
    }

    private (Mock<IDeliveryZoneRepository> mockDeliveryZoneRepository, Mock<IPricingStrategyService> mockPricingStrategyService, Mock<IItinerarySpi> mockItineraryService)
        CreateUpdateStepsDependencies(Delivery delivery, DeliveryZone? zone = null, double calculatedPrice = 10.0)
    {
        var mockDeliveryZoneRepository = new Mock<IDeliveryZoneRepository>();
        var mockPricingStrategyService = new Mock<IPricingStrategyService>();
        var mockItineraryService = new Mock<IItinerarySpi>();
        mockDeliveryZoneRepository.Setup(r => r.GetByAddress(It.IsAny<string>())).Returns(zone ?? _fixture.Create<DeliveryZone>());
        mockPricingStrategyService.Setup(p => p.CalculateDeliveryPriceWithoutVat(delivery)).Returns(calculatedPrice);
        mockItineraryService.Setup(i => i.GetItineraryInfoAsync(
                It.IsAny<GeoPoint>(),
                It.IsAny<GeoPoint>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ItineraryResult(10.0, 20.0));
        return (mockDeliveryZoneRepository, mockPricingStrategyService, mockItineraryService);
    }
}
