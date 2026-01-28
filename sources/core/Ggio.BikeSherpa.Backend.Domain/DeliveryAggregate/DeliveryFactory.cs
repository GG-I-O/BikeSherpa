using Ggio.DddCore;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;

public interface ICourseFactory
{
     Task<Delivery> CreateCourseAsync(DateTimeOffset startDate) //on n'initialise que ce qui est required
          ;
}

public class DeliveryFactory(IMediator mediator) : FactoryBase(mediator), ICourseFactory
{
     public async Task<Delivery> CreateCourseAsync(DateTimeOffset startDate) //on n'initialise que ce qui est required
     {
          var newEntity= new Delivery
          {
               StartDate = startDate
          };
          await NotifyNewEntityAdded(newEntity);
          return newEntity;
     }
}
