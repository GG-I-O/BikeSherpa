using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Spi;

namespace Ggio.BikeSherpa.Backend.Infrastructure.Repositories;

public class DelayRepository(BackendDbContext context) : IDelayRepository
{
     public const string Earlyorderdiscount = "EarlyOrderDiscount";
     public const string LastMinuteOrderExtraCost = "LastMinuteOrderExtraCost";
     public const string StandardCost = "StandardCost";
     public Delay GetEarlyOrderDiscount() => context.Delays.Single(d => d.Id == Earlyorderdiscount);

     public Delay GetLastMinuteOrderExtraCost() => context.Delays.Single(d => d.Id == LastMinuteOrderExtraCost);

     public Delay GetStandardCost() => context.Delays.Single(d => d.Id == StandardCost);
}
