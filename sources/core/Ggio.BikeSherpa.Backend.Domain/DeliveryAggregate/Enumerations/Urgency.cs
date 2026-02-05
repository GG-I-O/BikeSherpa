namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;

public sealed class Urgency : Enumeration
{
     public double PriceCoefficient { get; }

     private readonly static Urgency Eco = new(1, "Eco", 0.75);
     private readonly static Urgency Standard = new(2, "Standard", 1.25);
     private readonly static Urgency Urgent = new(3, "Urgent", 2);

     private Urgency(int id, string name, double priceCoefficient) : base(id, name)
     {
          PriceCoefficient = priceCoefficient;
     }
     
     public double CalculatePrice(double distance)
     {
          if (distance <= 0)
          {
               throw new Exception("La distance ne peut pas être nulle ou négative.");
          }

          return distance * PriceCoefficient;
     }

}
