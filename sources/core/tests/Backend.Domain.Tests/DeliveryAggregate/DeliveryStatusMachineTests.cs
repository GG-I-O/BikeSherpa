using AwesomeAssertions;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;
using AutoFixture;
using AutoFixture.AutoMoq;

namespace Backend.Domain.Tests.DeliveryAggregate;

public class DeliveryStatusMachineTests
{
    private readonly static IFixture Fixture = new Fixture().Customize(new AutoMoqCustomization());

    private static Delivery CreateDelivery(
        DeliveryStatus status = DeliveryStatus.Pending,
        List<DeliveryStep>? steps = null)
    {
        var delivery = Fixture.Build<Delivery>()
            .With(x => x.Status, status)
            .With(x => x.Steps, steps ?? [])
            .Create();

        return delivery;
    }

    private static DeliveryStep CreateStep(StepType type, bool completed = false)
    {
        var deliveryStep = Fixture.Build<DeliveryStep>()
            .Without(s => s.ParentDelivery)
            .With(x => x.StepType, type)
            .With(x => x.Completed, completed)
            .Create();

        return deliveryStep;
    }

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
