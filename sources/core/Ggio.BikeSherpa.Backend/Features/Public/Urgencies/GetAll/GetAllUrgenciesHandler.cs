using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Services.Repositories;
using Ggio.BikeSherpa.Backend.Features.Public.Urgencies.Model;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Features.Public.Urgencies.GetAll;

public record GetAllUrgenciesQuery() : IQuery<List<UrgencyDto>>;

public class GetAllUrgenciesHandler(IUrgencyRepository repository) : IQueryHandler<GetAllUrgenciesQuery, List<UrgencyDto>>
{
     public async ValueTask<List<UrgencyDto>> Handle(GetAllUrgenciesQuery query, CancellationToken cancellationToken)
     {
          var urgencies = repository.GetAll();

          return urgencies.Select(urgency => new UrgencyDto
               {
                    Label = urgency.Name,
                    Value = urgency.Name
               }
          ).ToList();
     }
}
