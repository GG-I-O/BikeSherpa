using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Spi;
using Ggio.BikeSherpa.Backend.Domain.Spi;

namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.PricingStrategies;

public interface IDelayService
{
     Task<Delay> CalculateDelay(DateTimeOffset startDate, DateTimeOffset contractDate);
     double CalculateDelayInHours(DateTimeOffset startDate, DateTimeOffset contractDate);
}

public class DelayService(IDelayRepository delayRepository, IParameterRepository parameterRepository) : IDelayService
{
     private readonly Lazy<ValueTask<int>> _earlyOrderLimitInHours = new(parameterRepository.GetEarlyOrderLimitInHoursAsync);
     private readonly Lazy<ValueTask<int>> _lastMinuteOrderLimitInHours = new(parameterRepository.GetLastMinuteOrderLimitInHoursAsync);
     
     public double CalculateDelayInHours(DateTimeOffset startDate, DateTimeOffset contractDate) => (startDate - contractDate).TotalHours;

     // Check if the delivery delay generates a discount or an extra cost
     public async Task<Delay> CalculateDelay(DateTimeOffset startDate, DateTimeOffset contractDate)
     {
          var hours = CalculateDelayInHours(startDate, contractDate);

          if (hours > await _earlyOrderLimitInHours.Value)
               return delayRepository.GetEarlyOrderDiscount();

          if (hours <= await _lastMinuteOrderLimitInHours.Value)
               return delayRepository.GetLastMinuteOrderExtraCost();

          return delayRepository.GetStandardCost();
     }
}
