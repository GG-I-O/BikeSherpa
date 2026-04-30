namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Services.Step;

public interface IDeliveryChangeTimeService
{
     Task ChangeTime(DeliveryStep step, DateTimeOffset date, CancellationToken cancellationToken);
     Task ChangeOrder(DeliveryStep step, MoveDirection moveDirection, CancellationToken cancellationToken);
}
