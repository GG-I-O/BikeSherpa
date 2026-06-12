namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Spi;

public interface IDelayRepository
{
     public Delay GetEarlyOrderDiscount();
     public Delay GetLastMinuteOrderExtraCost();
     public Delay GetStandardCost();
}
