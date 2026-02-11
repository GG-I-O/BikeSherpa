namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;

public record DeliveryZone(
     string Name,
     HashSet<string> Cities,
     double TourPrice,
     double Price
     );