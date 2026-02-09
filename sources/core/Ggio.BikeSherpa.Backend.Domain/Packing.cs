using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;

namespace Ggio.BikeSherpa.Backend.Domain;

public class Packing
{
    public Guid Id { get; private set; }
    public PackingSize Size { get; private set; }

    private Packing() { }

    public Packing(double weight, int length)
    {
        Id = Guid.NewGuid();
        Size = PackingSize.FromMeasurements(weight, length) ?? throw new Exception("Impossible de prendre en charge cette course.");
    }
}

