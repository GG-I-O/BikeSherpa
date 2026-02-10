using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;

namespace Ggio.BikeSherpa.Backend.Domain;

public class Packing
{
    public PackingSize Size { get; private set; }

    private Packing() { }

    public Packing(double weight, int length)
    {
        Size = PackingSize.FromMeasurements(weight, length) ?? throw new Exception("Impossible de prendre en charge cette course.");
    }
}

