using AwesomeAssertions;
using Ggio.BikeSherpa.Backend.Domain;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;
using Mediator;
using Moq;

namespace Backend.Domain.Tests.DeliveryAggregate;

public class DeliveryStatusMachineTests
{
    private readonly static Address SomeAddress = new()
    {
        Name = "Test",
        StreetInfo = "1 rue Test",
        Postcode = "75001",
        City = "Paris"
    };

    private readonly static DeliveryZone SomeZone = new(1, "Zone 1", []);

    private static Delivery CreateDelivery(
        DeliveryStatusEnum status = DeliveryStatusEnum.Pending,
        List<DeliveryStep>? steps = null)
    {
        var mediatorMock = new Mock<IMediator>();
        var delivery = new Delivery(mediatorMock.Object)
        {
            PricingStrategy = PricingStrategyEnum.SimpleDeliveryStrategy,
            Code = "TEST-001",
            CustomerId = Guid.NewGuid(),
            Urgency = "Normal",
            PackingSize = "Standard",
            InsulatedBox = false,
            ContractDate = DateTimeOffset.UtcNow,
            StartDate = DateTimeOffset.UtcNow,
            Steps = steps ?? []
        };
        delivery.Status = status;
        return delivery;
    }

    private static DeliveryStep CreateStep(StepTypeEnum type, bool completed = false) =>
        new(type, 1, SomeAddress, SomeZone, 1.0, DateTimeOffset.UtcNow) { Completed = completed };

    [Fact]
    public void Fire_Start_WhenPendingAndPickupStepCompleted_TransitionsToStarted()
    {
        var delivery = CreateDelivery(steps: [CreateStep(StepTypeEnum.Pickup, completed: true)]);
        var sut = new DeliveryStatusMachine(delivery);

        sut.Fire(DeliveryStatusTrigger.Start);

        delivery.Status.Should().Be(DeliveryStatusEnum.Started);
    }

    [Fact]
    public void Fire_Start_WhenPendingAndPickupStepNotCompleted_Throws()
    {
        var delivery = CreateDelivery(steps: [CreateStep(StepTypeEnum.Pickup, completed: false)]);
        var sut = new DeliveryStatusMachine(delivery);

        var act = () => sut.Fire(DeliveryStatusTrigger.Start);

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Fire_Start_WhenPendingAndNoSteps_Throws()
    {
        var delivery = CreateDelivery(steps: []);
        var sut = new DeliveryStatusMachine(delivery);

        var act = () => sut.Fire(DeliveryStatusTrigger.Start);

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Fire_Cancel_WhenPending_TransitionsToCancelled()
    {
        var delivery = CreateDelivery();
        var sut = new DeliveryStatusMachine(delivery);

        sut.Fire(DeliveryStatusTrigger.Cancel);

        delivery.Status.Should().Be(DeliveryStatusEnum.Cancelled);
    }

    [Fact]
    public void Fire_Complete_WhenStartedAndAllStepsCompleted_TransitionsToCompleted()
    {
        var delivery = CreateDelivery(
            status: DeliveryStatusEnum.Started,
            steps: [CreateStep(StepTypeEnum.Pickup, completed: true), CreateStep(StepTypeEnum.Dropoff, completed: true)]);
        var sut = new DeliveryStatusMachine(delivery);

        sut.Fire(DeliveryStatusTrigger.Complete);

        delivery.Status.Should().Be(DeliveryStatusEnum.Completed);
    }

    [Fact]
    public void Fire_Complete_WhenStartedAndNotAllStepsCompleted_Throws()
    {
        var delivery = CreateDelivery(
            status: DeliveryStatusEnum.Started,
            steps: [CreateStep(StepTypeEnum.Pickup, completed: true), CreateStep(StepTypeEnum.Dropoff, completed: false)]);
        var sut = new DeliveryStatusMachine(delivery);

        var act = () => sut.Fire(DeliveryStatusTrigger.Complete);

        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Fire_Cancel_WhenStarted_TransitionsToCancelled()
    {
        var delivery = CreateDelivery(status: DeliveryStatusEnum.Started);
        var sut = new DeliveryStatusMachine(delivery);

        sut.Fire(DeliveryStatusTrigger.Cancel);

        delivery.Status.Should().Be(DeliveryStatusEnum.Cancelled);
    }

    [Fact]
    public void Fire_Complete_WhenCompleted_IsIgnored()
    {
        var delivery = CreateDelivery(status: DeliveryStatusEnum.Completed);
        var sut = new DeliveryStatusMachine(delivery);

        sut.Fire(DeliveryStatusTrigger.Complete);

        delivery.Status.Should().Be(DeliveryStatusEnum.Completed);
    }

    [Fact]
    public void Fire_Start_WhenCompleted_IsIgnored()
    {
        var delivery = CreateDelivery(status: DeliveryStatusEnum.Completed);
        var sut = new DeliveryStatusMachine(delivery);

        sut.Fire(DeliveryStatusTrigger.Start);

        delivery.Status.Should().Be(DeliveryStatusEnum.Completed);
    }

    [Fact]
    public void Fire_Complete_WhenCancelled_IsIgnored()
    {
        var delivery = CreateDelivery(status: DeliveryStatusEnum.Cancelled);
        var sut = new DeliveryStatusMachine(delivery);

        sut.Fire(DeliveryStatusTrigger.Complete);

        delivery.Status.Should().Be(DeliveryStatusEnum.Cancelled);
    }

    [Fact]
    public void Fire_Cancel_WhenCancelled_IsIgnored()
    {
        var delivery = CreateDelivery(status: DeliveryStatusEnum.Cancelled);
        var sut = new DeliveryStatusMachine(delivery);

        sut.Fire(DeliveryStatusTrigger.Cancel);

        delivery.Status.Should().Be(DeliveryStatusEnum.Cancelled);
    }
}
