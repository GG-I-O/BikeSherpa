using Ggio.DddCore;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Domain.StepAggregate;

public interface IStepFactory
{
     Task<Step> CreateStepAsync(
          string type,
          Address address,
          double distance,
          double price,
          string courierId,
          string comment,
          string[] filePaths,
          DateTime contractDate,
          DateTime estimatedDeliveryDate,
          DateTime realDeliveryDate
     );
}

public class StepFactory(IMediator mediator) : FactoryBase(mediator), IStepFactory
{

     public async Task<Step> CreateStepAsync(string type, Address address, double distance, double price, string courierId, string comment, string[] filePaths, DateTime contractDate, DateTime estimatedDeliveryDate, DateTime realDeliveryDate)
     {
          var step = new Step
          {
               Type = type,
               Address = address,
               Distance = distance,
               Price = price,
               CourierId = courierId,
               Comment = comment,
               filePaths = filePaths,
               ContractDate = contractDate,
               EstimatedDeliveryDate = estimatedDeliveryDate,
               RealDeliveryDate = realDeliveryDate
          };
          
          await NotifyNewEntityAdded(step);
          
          return step;
     }
}
