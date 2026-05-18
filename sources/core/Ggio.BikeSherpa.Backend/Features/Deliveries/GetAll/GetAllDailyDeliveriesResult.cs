using Ggio.BikeSherpa.Backend.Features.Deliveries.Model;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.GetAll;

public abstract record GetAllDailyDeliveriesResult
{
     public record Success(List<DeliveryCrud> Deliveries) : GetAllDailyDeliveriesResult;
     public record CourierNotFound : GetAllDailyDeliveriesResult;
}
