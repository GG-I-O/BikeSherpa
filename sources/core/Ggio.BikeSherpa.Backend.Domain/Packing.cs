using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;

namespace Ggio.BikeSherpa.Backend.Domain;

public class Packing
{
    public Guid Id { get; private set; }
    public DeliveryZone PickupZone { get; private set; }
    public DeliveryZone DropoffZone { get; private set; }
    public PackingSize Size { get; private set; }

    private Packing() { }

    public Packing(double weight, int length, DeliveryZone pickupZone, DeliveryZone dropoffZone)
    {
        Id = Guid.NewGuid();
        PickupZone = pickupZone ?? throw new Exception("Indiquez la zone géographique de collecte.");
        DropoffZone = dropoffZone ?? throw new Exception("Indiquez la zone géographique de livraison.");
        Size = PackingSize.FromMeasurements(weight, length) ?? throw new Exception("Impossible de prendre en charge cette course.");
    }
}

