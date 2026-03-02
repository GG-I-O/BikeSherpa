using AutoFixture;
using AutoFixture.AutoMoq;
using AwesomeAssertions;
using Ggio.BikeSherpa.Backend.Domain;
using Ggio.BikeSherpa.Backend.Domain.CustomerAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Services.PricingStrategy;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Services.Repositories;
using Moq;

namespace Backend.Domain.Tests.DeliveryAggregate;

public class DeliveryTests
{
    private readonly Mock<IPricingStrategyService> _pricingStrategyServiceMock;
    private readonly IFixture _fixture;
    private readonly Delivery _sut;

    public DeliveryTests()
    {
        _fixture = new Fixture().Customize(new AutoMoqCustomization());
        _pricingStrategyServiceMock = new Mock<IPricingStrategyService>();
        _sut = _fixture.Create<Delivery>();
    }

    private DeliveryStep CreatePickupStep(bool completed = false)
    {
        var deliveryStep = _fixture.Create<DeliveryStep>();
        deliveryStep.Id = Guid.NewGuid();
        deliveryStep.StepType = StepType.Pickup;
        deliveryStep.Completed = completed;

        return deliveryStep;
    }

    private DeliveryStep CreateDropoffStep(bool completed = false)
    {
        var deliveryStep = _fixture.Create<DeliveryStep>();
        deliveryStep.Id = Guid.NewGuid();
        deliveryStep.StepType = StepType.Dropoff;
        deliveryStep.Completed = completed;

        return deliveryStep;
    }

    [Fact]
    public void GenerateReportId_ReturnsIdStartingWithCustomerCode()
    {
        // Arrange
        var customer = _fixture.Create<Customer>();

        // Act
        var reportId = _sut.GenerateReportId(customer);

        // Assert
        reportId.Should().StartWith($"{customer.Code}-");
    }

    [Fact]
    public void GenerateReportId_TimestampPartHasExpectedLength()
    {
        // Arrange
        var customer = _fixture.Create<Customer>();
        customer.Code = "TEST";

        // Act
        var reportId = _sut.GenerateReportId(customer);

        // Assert
        // ReportId format = CUSTOMERCODE-yyyyMMddHHmmss (14-character timestamp)
        var timestamp = reportId[(reportId.IndexOf('-') + 1)..];
        timestamp.Should().HaveLength(14);
    }

    [Fact]
    public void GenerateReportId_DifferentCustomerCodes_ProduceDifferentPrefixes()
    {
        // Arrange
        var customers = _fixture.CreateMany<Customer>(2).ToList();
        customers[0].Code = "TEST1";
        customers[1].Code = "TEST2";

        // Act
        var id1 = _sut.GenerateReportId(customers[0]);
        var id2 = _sut.GenerateReportId(customers[1]);

        // Assert
        id1.Should().StartWith("TEST1-");
        id2.Should().StartWith("TEST2-");
    }

    [Fact]
    public void UpdateDeliveryStartDateTime_SetsNewStartDateAndRecalculatesTotalPrice()
    {
        // Arrange
        var newDate = _fixture.Create<DateTimeOffset>();

        // Act
        _sut.UpdateDeliveryStartDateTime(newDate, _pricingStrategyServiceMock.Object);

        // Assert
        _sut.StartDate.Should().Be(newDate);
        _pricingStrategyServiceMock.Verify(p => p.CalculateDeliveryPriceWithoutVat(_sut), Times.Once);
    }

    [Fact]
    public void ReorderSteps_ReordersAllStepsSettingCorrectOrderForMovedStep()
    {
        // Arrange
        _sut.Steps.Clear();
        var step1 = CreatePickupStep();
        var step2 = CreateDropoffStep();
        var step3 = CreateDropoffStep();
        step1.Order = 1;
        step2.Order = 2;
        step3.Order = 3;
        _sut.Steps.AddRange([step1, step2, step3]);

        // Act
        _sut.ReorderSteps(step3.Id, 2);

        // Assert
        _sut.Steps.Single(s => s.Id == step1.Id).Order.Should().Be(1);
        _sut.Steps.Single(s => s.Id == step3.Id).Order.Should().Be(2);
        _sut.Steps.Single(s => s.Id == step2.Id).Order.Should().Be(3);
    }

    [Fact]
    public void ReorderSteps_WhenStepNotFound_Throws()
    {
        // Arrange & Act
        var act = () => _sut.ReorderSteps(Guid.NewGuid(), 1);

        // Assert
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void UpdateStepCourier_SetsNewCourierOnStep()
    {
        // Arrange
        var step = CreatePickupStep();
        _sut.Steps.Add(step);
        var courierId = Guid.NewGuid();

        // Act
        _sut.UpdateStepCourier(step.Id, courierId);

        // Assert
        _sut.Steps.Single(s => s.Id == step.Id).CourierId.Should().Be(courierId);
    }

    [Fact]
    public void UpdateStepCourier_WhenStepNotFound_Throws()
    {
        // Arrange & Act
        var act = () => _sut.UpdateStepCourier(Guid.NewGuid(), Guid.NewGuid());

        // Assert
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public async Task UpdateStepCompletion_MarksStepAsCompletedAndSetsRealDeliveryDate()
    {
        // Arrange
        var step = CreateDropoffStep();
        _sut.Steps.Add(step);

        // Act
        await _sut.UpdateStepCompletion(step.Id, true);

        // Assert
        step.Completed.Should().BeTrue();
        step.RealDeliveryDate.Should().NotBe(null);
    }

    [Fact]
    public async Task UpdateStepCompletion_MarksStepAsNotCompleted()
    {
        // Arrange
        var step = CreateDropoffStep(completed: true);
        _sut.Steps.Add(step);

        // Act
        await _sut.UpdateStepCompletion(step.Id, false);

        // Assert
        step.Completed.Should().BeFalse();
    }

    [Fact]
    public async Task UpdateStepCompletion_WhenPickupStepCompleted_StartsDelivery()
    {
        // Arrange
        var pickup = CreatePickupStep();
        _sut.Steps.Add(pickup);

        // Act
        await _sut.UpdateStepCompletion(pickup.Id, true);

        // Assert
        _sut.Status.Should().Be(DeliveryStatus.Started);
    }

    [Fact]
    public async Task UpdateStepCompletion_WhenAllStepsCompleted_CompletesDelivery()
    {
        // Arrange
        _sut.Steps.Clear();
        var pickup = CreatePickupStep(completed: true);
        var dropoff = CreateDropoffStep(completed: false);
        _sut.Steps.AddRange([pickup, dropoff]);
        _sut.Status = DeliveryStatus.Started;

        // Act
        await _sut.UpdateStepCompletion(dropoff.Id, true);

        // Assert
        _sut.Status.Should().Be(DeliveryStatus.Completed);
    }

    [Fact]
    public async Task UpdateStepCompletion_WhenDeliveryIsAlreadyCompleted_Throws()
    {
        // Arrange
        var step = CreatePickupStep();
        _sut.Steps.Add(step);
        _sut.Status = DeliveryStatus.Completed;

        // Act
        var act = async () => await _sut.UpdateStepCompletion(step.Id, true);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task UpdateStepCompletion_WhenDeliveryIsCancelled_Throws()
    {
        // Arrange
        var step = CreatePickupStep();
        _sut.Steps.Add(step);
        _sut.Status = DeliveryStatus.Cancelled;

        // Act
        var act = async () => await _sut.UpdateStepCompletion(step.Id, true);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>();
    }

    [Fact]
    public async Task Cancel_WhenPending_ChangesStatusToCancelled()
    {
        // Arrange & Act
        await _sut.Cancel();

        // Assert
        _sut.Status.Should().Be(DeliveryStatus.Cancelled);
    }

    [Fact]
    public async Task Cancel_WhenStarted_ChangesStatusToCancelled()
    {
        // Arrange
        _sut.Status = DeliveryStatus.Started;

        // Act
        await _sut.Cancel();

        // Assert
        _sut.Status.Should().Be(DeliveryStatus.Cancelled);
    }

    [Fact]
    public void AddStep_AddsStepToStepsList()
    {
        // Arrange
        _sut.Steps.Clear();
        var (mockDeliveryZoneRepository, mockPricingStrategyService, address) = CreateStepDependencies();

        // Act
        _sut.AddStep(StepType.Pickup, address, 5.0, mockDeliveryZoneRepository.Object, mockPricingStrategyService.Object);
        _sut.AddStep(StepType.Dropoff, address, 5.0, mockDeliveryZoneRepository.Object, mockPricingStrategyService.Object);

        // Assert
        _sut.Steps.Should().HaveCount(2);
        _sut.Steps[0].StepType.Should().Be(StepType.Pickup);
        _sut.Steps[0].Order.Should().Be(1);
        _sut.Steps[0].Distance.Should().Be(5.0);
        _sut.Steps[0].StepAddress.Should().Be(address);
        _sut.Steps[0].StepZone.Should().NotBe(null);
        _sut.Steps[0].Id.Should().NotBeEmpty();
        _sut.Steps[0].EstimatedDeliveryDate.Should().Be(_sut.StartDate);
        _sut.Steps[1].EstimatedDeliveryDate.Should().Be(_sut.StartDate.AddMinutes(15));
    }

    [Fact]
    public void AddStep_ReturnsCreatedStepWithCorrectProperties()
    {
        // Arrange
        _sut.Steps.Clear();
        var (mockDeliveryZoneRepository, mockPricingStrategyService, address) = CreateStepDependencies();

        // Act
        var step = _sut.AddStep(StepType.Dropoff, address, 7.5, mockDeliveryZoneRepository.Object, mockPricingStrategyService.Object);

        // Assert
        step.StepType.Should().Be(StepType.Dropoff);
        step.Order.Should().Be(1);
        step.Distance.Should().Be(7.5);
        step.Id.Should().NotBeEmpty();
    }

    [Fact]
    public void AddStep_RecalculatesTotalPrice()
    {
        // Arrange
        _sut.Steps.Clear();
        var (mockDeliveryZoneRepository, mockPricingStrategyService, address) = CreateStepDependencies(calculatedPrice: 25.0);

        // Act
        _sut.AddStep(StepType.Pickup, address, 5.0, mockDeliveryZoneRepository.Object, mockPricingStrategyService.Object);

        // Assert
        _sut.TotalPrice.Should().Be(25.0);
    }

    [Fact]
    public void AddStep_LooksUpDeliveryZoneByAddressCity()
    {
        // Arrange
        _sut.Steps.Clear();
        var (mockDeliveryZoneRepository, mockPricingStrategyService, address) = CreateStepDependencies();

        // Act
        _sut.AddStep(StepType.Pickup, address, 5.0, mockDeliveryZoneRepository.Object, mockPricingStrategyService.Object);

        // Assert
        mockDeliveryZoneRepository.Verify(r => r.GetByAddress(address.City), Times.Once);
    }

    [Fact]
    public void DeleteStep_RemovesStepFromStepsList()
    {
        // Arrange
        _sut.Steps.Clear();
        var mockPricingStrategyService = new Mock<IPricingStrategyService>();
        mockPricingStrategyService.Setup(p => p.CalculateDeliveryPriceWithoutVat(_sut)).Returns(0.0);
        var step = CreatePickupStep();
        _sut.Steps.Add(step);

        // Act
        _sut.DeleteStep(step, mockPricingStrategyService.Object);

        // Assert
        _sut.Steps.Should().BeEmpty();
    }

    [Fact]
    public void DeleteStep_RecalculatesTotalPrice()
    {
        // Arrange
        var mockPricingStrategyService = new Mock<IPricingStrategyService>();
        mockPricingStrategyService.Setup(p => p.CalculateDeliveryPriceWithoutVat(_sut)).Returns(0.0);
        var step = CreatePickupStep();
        _sut.Steps.Add(step);

        // Act
        _sut.DeleteStep(step, mockPricingStrategyService.Object);

        // Assert
        mockPricingStrategyService.Verify(p => p.CalculateDeliveryPriceWithoutVat(_sut), Times.Once);
    }

    [Fact]
    public void DeleteStep_OnlyRemovesTargetStep()
    {
        // Arrange
        _sut.Steps.Clear();
        var mockPricingStrategyService = new Mock<IPricingStrategyService>();
        mockPricingStrategyService.Setup(p => p.CalculateDeliveryPriceWithoutVat(_sut)).Returns(0.0);
        var stepToKeep = CreatePickupStep();
        var stepToDelete = CreatePickupStep();
        _sut.Steps.AddRange([stepToKeep, stepToDelete]);

        // Act
        _sut.DeleteStep(stepToDelete, mockPricingStrategyService.Object);

        // Assert
        _sut.Steps.Should().HaveCount(1);
        _sut.Steps.Should().ContainSingle(s => s.Id == stepToKeep.Id);
    }

    [Fact]
    public void UpdateSteps_AddsNewStepsThatDontExistYet()
    {
        // Arrange
        var (mockDeliveryZoneRepository, mockPricingStrategyService) = CreateUpdateStepsDependencies();
        var newStep = CreatePickupStep();

        // Act
        _sut.UpdateSteps([newStep], mockDeliveryZoneRepository.Object, mockPricingStrategyService.Object);

        // Assert
        _sut.Steps.Should().ContainSingle(s => s.Id == newStep.Id);
    }

    [Fact]
    public void UpdateSteps_UpdatesExistingSteps()
    {
        // Arrange
        var zone = _fixture.Create<DeliveryZone>();
        var (mockDeliveryZoneRepository, mockPricingStrategyService) = CreateUpdateStepsDependencies(zone);
        var existingStep = CreatePickupStep();
        _sut.Steps.Add(existingStep);

        // Act
        var updatedStep = new DeliveryStep(StepType.Dropoff, 3, existingStep.StepAddress, zone, 8.0)
        {
            Id = existingStep.Id
        };

        _sut.UpdateSteps([updatedStep], mockDeliveryZoneRepository.Object, mockPricingStrategyService.Object);

        // Assert
        _sut.Steps.Should().HaveCount(1);
        _sut.Steps[0].StepType.Should().Be(StepType.Dropoff);
        _sut.Steps[0].Order.Should().Be(3);
        _sut.Steps[0].Distance.Should().Be(8.0);
        _sut.Steps[0].StepZone.Should().Be(zone);
    }

    [Fact]
    public void UpdateSteps_RemovesStepsNotInIncomingList()
    {
        // Arrange
        var (mockDeliveryZoneRepository, mockPricingStrategyService) = CreateUpdateStepsDependencies();
        var stepToKeep = CreatePickupStep();
        var stepToRemove = CreatePickupStep();
        _sut.Steps.AddRange([stepToKeep, stepToRemove]);

        // Act
        _sut.UpdateSteps([stepToKeep], mockDeliveryZoneRepository.Object, mockPricingStrategyService.Object);

        // Assert
        _sut.Steps.Should().HaveCount(1);
        _sut.Steps.Should().ContainSingle(s => s.Id == stepToKeep.Id);
        _sut.Steps.Should().NotContain(s => s.Id == stepToRemove.Id);
    }

    [Fact]
    public void UpdateSteps_WhenEmptyListProvided_RemovesAllExistingSteps()
    {
        // Arrange
        var (mockDeliveryZoneRepository, mockPricingStrategyService) = CreateUpdateStepsDependencies();
        _sut.Steps.AddRange([CreatePickupStep(), CreatePickupStep()]);

        // Act
        _sut.UpdateSteps([], mockDeliveryZoneRepository.Object, mockPricingStrategyService.Object);

        // Assert
        _sut.Steps.Should().BeEmpty();
    }

    [Fact]
    public void UpdateSteps_RecalculatesTotalPrice()
    {
        // Arrange
        var (mockDeliveryZoneRepository, mockPricingStrategyService) = CreateUpdateStepsDependencies(calculatedPrice: 35.0);
        var step = CreatePickupStep();

        // Act
        _sut.UpdateSteps([step], mockDeliveryZoneRepository.Object, mockPricingStrategyService.Object);

        // Assert
        _sut.TotalPrice.Should().Be(35.0);
    }

    [Fact]
    public void UpdateStepUpdateStepDeliveryTime_ChangesUpdatedStepAndNextStepsDeliveryTime()
    {
        // Arrange
        _sut.Steps.Clear();
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
        _sut.Steps.AddRange(step1, step2, step3, step4);

        // Act
        _sut.UpdateStepDeliveryTime(step2.Id, startTime.AddMinutes(20));

        // Assert
        step2.EstimatedDeliveryDate.Should().Be(startTime.AddMinutes(20));
        step3.EstimatedDeliveryDate.Should().Be(startTime.AddMinutes(30));
        step4.EstimatedDeliveryDate.Should().Be(startTime.AddMinutes(40));
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    private (Mock<IDeliveryZoneRepository> mockDeliveryZoneRepository, Mock<IPricingStrategyService> mockPricingStrategyService, Address address)
        CreateStepDependencies(double calculatedPrice = 15.0)
    {
        var mockDeliveryZoneRepository = new Mock<IDeliveryZoneRepository>();
        var mockPricingStrategyService = new Mock<IPricingStrategyService>();
        var address = _fixture.Create<Address>();
        mockDeliveryZoneRepository.Setup(r => r.GetByAddress(address.City)).Returns(_fixture.Create<DeliveryZone>());
        mockPricingStrategyService.Setup(p => p.CalculateDeliveryPriceWithoutVat(_sut)).Returns(calculatedPrice);
        return (mockDeliveryZoneRepository, mockPricingStrategyService, address);
    }

    private (Mock<IDeliveryZoneRepository> mockDeliveryZoneRepository, Mock<IPricingStrategyService> mockPricingStrategyService)
        CreateUpdateStepsDependencies(DeliveryZone? zone = null, double calculatedPrice = 10.0)
    {
        var mockDeliveryZoneRepository = new Mock<IDeliveryZoneRepository>();
        var mockPricingStrategyService = new Mock<IPricingStrategyService>();
        mockDeliveryZoneRepository.Setup(r => r.GetByAddress(It.IsAny<string>())).Returns(zone ?? _fixture.Create<DeliveryZone>());
        mockPricingStrategyService.Setup(p => p.CalculateDeliveryPriceWithoutVat(_sut)).Returns(calculatedPrice);
        return (mockDeliveryZoneRepository, mockPricingStrategyService);
    }
}
