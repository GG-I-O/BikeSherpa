using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Services.Repositories;
using Ggio.BikeSherpa.Backend.Features.Public.PackingSizes.Model;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Features.Public.PackingSizes.GetAll;

public record GetAllPackingSizesQuery() : IQuery<List<PackingSizeDto>>;

public class GetAllPackingSizesHandler(IPackingSizeRepository repository) : IQueryHandler<GetAllPackingSizesQuery, List<PackingSizeDto>>
{
     public async ValueTask<List<PackingSizeDto>> Handle(GetAllPackingSizesQuery query, CancellationToken cancellationToken)
     {
          var packingSizes = repository.GetAll();

          return packingSizes.Select(packingSize => new PackingSizeDto
               {
                    Label = packingSize.Name,
                    Value = packingSize.Name
               }
          ).ToList();
     }
}
