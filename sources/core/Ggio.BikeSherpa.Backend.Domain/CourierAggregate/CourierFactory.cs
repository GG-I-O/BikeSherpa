using Ggio.DddCore;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Domain.CourierAggregate;

public interface ICourierFactory
{
     Task<Courier> CreateCourierAsync(
          string firstName,
          string lastName,
          string code,
          string email,
          string phoneNumber,
          Address address
     );
}

public class CourierFactory(IMediator mediator) : FactoryBase(mediator), ICourierFactory
{
     public async Task<Courier> CreateCourierAsync(string firstName, string lastName, string code,  string email, string phoneNumber, Address address)
     {
          var courier = new Courier
          {
               LastName = lastName,
               FirstName = firstName,
               Code = code,
               Email = email,
               PhoneNumber = phoneNumber,
               Address = address
          };

          await NotifyNewEntityAdded(courier);
          return courier;
     }
}