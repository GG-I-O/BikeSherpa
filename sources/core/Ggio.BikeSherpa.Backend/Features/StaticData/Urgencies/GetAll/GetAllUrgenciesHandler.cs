using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Spi;
using Ggio.BikeSherpa.Backend.Features.StaticData.Urgencies.Model;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Features.StaticData.Urgencies.GetAll;

public record GetAllUrgenciesQuery : IQuery<List<UrgencyDto>>;

public class GetAllUrgenciesHandler(IUrgencyRepository repository) : IQueryHandler<GetAllUrgenciesQuery, List<UrgencyDto>>
{
     public async ValueTask<List<UrgencyDto>> Handle(GetAllUrgenciesQuery query, CancellationToken cancellationToken)
     {
          var urgencies = repository.GetAll();

          return await Task.FromResult(
               urgencies.Select(urgency => new UrgencyDto
                    {
                         Label = urgency.Label,
                         Value = urgency.Name,
                         LastHourToOrder = urgency.LastHourToOrder
                    }
               ).ToList()
          );
     }
}
