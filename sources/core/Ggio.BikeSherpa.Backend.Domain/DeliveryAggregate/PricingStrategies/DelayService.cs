using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Spi;
using Ggio.BikeSherpa.Backend.Domain.Spi;

namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.PricingStrategies;

public interface IDelayService
{
     Delay CalculateDelay(DateTimeOffset startDate, DateTimeOffset contractDate);
     double CalculateDelayInHours(DateTimeOffset startDate, DateTimeOffset contractDate);
}

public class DelayService(IDelayRepository delayRepository, IParameterRepository parameterRepository) : IDelayService
{
     //TODO mettre en paramètre
     private const double EarlyOrderLimitInHours = 18;
     private const double LastMinuteOrderLimitInHours = 2;

     public double CalculateDelayInHours(DateTimeOffset startDate, DateTimeOffset contractDate) => (startDate - contractDate).TotalHours;



     // Check if the delivery delay generates a discount or an extra cost
     public Delay CalculateDelay(DateTimeOffset startDate, DateTimeOffset contractDate)
     {
          var hours = CalculateDelayInHours(startDate, contractDate);
          return hours switch
          {
               > EarlyOrderLimitInHours => delayRepository.GetEarlyOrderDiscount(),
               <= LastMinuteOrderLimitInHours => delayRepository.GetLastMinuteOrderExtraCost(),
               _ => delayRepository.GetStandardCost()
          };
     }
}
