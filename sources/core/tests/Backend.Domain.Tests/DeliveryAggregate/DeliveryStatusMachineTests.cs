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
        DeliveryStatus status = DeliveryStatus.Pending,
        List<DeliveryStep>? steps = null)
    {
        var mediatorMock = new Mock<IMediator>();
        var delivery = new Delivery(mediatorMock.Object)
        {
            PricingStrategy = PricingStrategy.SimpleDeliveryStrategy,
            Code = "TEST-001",
            CustomerId = Guid.NewGuid(),
            Urgency = "Normal",
            PackingSize = "Standard",
            InsulatedBox = false,
            ContractDate = DateTimeOffset.UtcNow,
            StartDate = DateTimeOffset.UtcNow,
            Steps = steps ?? [],
            Status = status
        };

        return delivery;
    }

    private static DeliveryStep CreateStep(StepType type, bool completed = false) =>
        new(type, 1, SomeAddress, SomeZone, 1.0, DateTimeOffset.UtcNow) { Completed = completed };

    [Fact]
    public void Fire_Start_WhenPendingAndPickupStepCompleted_TransitionsToStarted()
    {
        // Arrange
        var delivery = CreateDelivery(steps: [CreateStep(StepType.Pickup, completed: true)]);
        var sut = new DeliveryStatusMachine(delivery);

        // Act
        sut.Fire(DeliveryStatusTrigger.Start);

        // Assert
        delivery.Status.Should().Be(DeliveryStatus.Started);
    }

    [Fact]
    public void Fire_Start_WhenPendingAndPickupStepNotCompleted_Throws()
    {
        // Arrange
        var delivery = CreateDelivery(steps: [CreateStep(StepType.Pickup, completed: false)]);
        var sut = new DeliveryStatusMachine(delivery);

        // Act
        var act = () => sut.Fire(DeliveryStatusTrigger.Start);

        // Assert
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Fire_Start_WhenPendingAndNoSteps_Throws()
    {
        // Arrange
        var delivery = CreateDelivery(steps: []);
        var sut = new DeliveryStatusMachine(delivery);

        // Act
        var act = () => sut.Fire(DeliveryStatusTrigger.Start);

        // Assert
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Fire_Cancel_WhenPending_TransitionsToCancelled()
    {
        // Arrange
        var delivery = CreateDelivery();
        var sut = new DeliveryStatusMachine(delivery);

        // Act
        sut.Fire(DeliveryStatusTrigger.Cancel);

        // Assert
        delivery.Status.Should().Be(DeliveryStatus.Cancelled);
    }

    [Fact]
    public void Fire_Complete_WhenStartedAndAllStepsCompleted_TransitionsToCompleted()
    {
        // Arrange
        var delivery = CreateDelivery(
            status: DeliveryStatus.Started,
            steps: [CreateStep(StepType.Pickup, completed: true), CreateStep(StepType.Dropoff, completed: true)]);
        var sut = new DeliveryStatusMachine(delivery);

        // Act
        sut.Fire(DeliveryStatusTrigger.Complete);

        // Assert
        delivery.Status.Should().Be(DeliveryStatus.Completed);
    }

    [Fact]
    public void Fire_Complete_WhenStartedAndNotAllStepsCompleted_Throws()
    {
        // Arrange
        var delivery = CreateDelivery(
            status: DeliveryStatus.Started,
            steps: [CreateStep(StepType.Pickup, completed: true), CreateStep(StepType.Dropoff, completed: false)]);
        var sut = new DeliveryStatusMachine(delivery);

        // Act
        var act = () => sut.Fire(DeliveryStatusTrigger.Complete);

        // Assert
        act.Should().Throw<InvalidOperationException>();
    }

    [Fact]
    public void Fire_Cancel_WhenStarted_TransitionsToCancelled()
    {
        // Arrange
        var delivery = CreateDelivery(status: DeliveryStatus.Started);
        var sut = new DeliveryStatusMachine(delivery);

        // Act
        sut.Fire(DeliveryStatusTrigger.Cancel);

        // Assert
        delivery.Status.Should().Be(DeliveryStatus.Cancelled);
    }

    [Fact]
    public void Fire_Complete_WhenCompleted_IsIgnored()
    {
        // Arrange
        var delivery = CreateDelivery(status: DeliveryStatus.Completed);
        var sut = new DeliveryStatusMachine(delivery);

        // Act
        sut.Fire(DeliveryStatusTrigger.Complete);

        // Assert
        delivery.Status.Should().Be(DeliveryStatus.Completed);
    }

    [Fact]
    public void Fire_Start_WhenCompleted_IsIgnored()
    {
        // Arrange
        var delivery = CreateDelivery(status: DeliveryStatus.Completed);
        var sut = new DeliveryStatusMachine(delivery);

        // Act
        sut.Fire(DeliveryStatusTrigger.Start);

        // Assert
        delivery.Status.Should().Be(DeliveryStatus.Completed);
    }

    [Fact]
    public void Fire_Complete_WhenCancelled_IsIgnored()
    {
        // Arrange
        var delivery = CreateDelivery(status: DeliveryStatus.Cancelled);
        var sut = new DeliveryStatusMachine(delivery);

        // Act
        sut.Fire(DeliveryStatusTrigger.Complete);

        // Assert
        delivery.Status.Should().Be(DeliveryStatus.Cancelled);
    }

    [Fact]
    public void Fire_Cancel_WhenCancelled_IsIgnored()
    {
        // Arrange
        var delivery = CreateDelivery(status: DeliveryStatus.Cancelled);
        var sut = new DeliveryStatusMachine(delivery);

        // Act
        sut.Fire(DeliveryStatusTrigger.Cancel);

        // Assert
        delivery.Status.Should().Be(DeliveryStatus.Cancelled);
    }
}
