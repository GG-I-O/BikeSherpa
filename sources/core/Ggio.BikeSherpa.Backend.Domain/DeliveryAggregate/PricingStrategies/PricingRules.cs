namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.PricingStrategies;

public static class PricingRules
{
     private const double SameDayDeliveryExtraCost = 2;
     private const double EarlyOrderLimitInHours = 18;
     private const double LastMinuteOrderLimitInHours = 2;
     private const double EarlyOrderDiscount = -2;
     private const double LastMinuteOrderExtraCost = 3;
     private const double StandardCost = 0;

     // Check if there is an extra cost for a delivery on the same day as the contract
     // A delivery for the next day ordered after 5PM is considered a "same day delivery"
     public static double CalculateSameDayDeliveryExtraCost(DateTimeOffset startDate, DateTimeOffset contractDate)
     {
          if (contractDate.Hour >= 17 && contractDate.Date.AddDays(1) == startDate.Date || startDate.Date == contractDate.Date)
          {
               return SameDayDeliveryExtraCost;
          }
          else
          {
               return StandardCost;
          }
     }

     // Check if the delivery delay generates a discount or an extra cost
     public static double CalculateDelayCost(DateTimeOffset startDate, DateTimeOffset contractDate)
     {
          var hours = CalculateDelayInHours(startDate, contractDate);
          return hours switch
          {
               > EarlyOrderLimitInHours => EarlyOrderDiscount,
               <= LastMinuteOrderLimitInHours => LastMinuteOrderExtraCost,
               _ => StandardCost
          };
     }

     private static double CalculateDelayInHours(DateTimeOffset startDate, DateTimeOffset contractDate)
     {
          return (contractDate - startDate).TotalHours;
     }
}
