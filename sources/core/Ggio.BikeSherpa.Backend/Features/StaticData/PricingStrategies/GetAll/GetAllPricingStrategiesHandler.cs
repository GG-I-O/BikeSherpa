using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;
using Ggio.BikeSherpa.Backend.Features.StaticData.PricingStrategies.Model;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Features.StaticData.PricingStrategies.GetAll;

public record GetAllPricingStrategiesQuery : IQuery<List<PricingStrategyDto>>;

public class GetAllPricingStrategiesHandler : IQueryHandler<GetAllPricingStrategiesQuery, List<PricingStrategyDto>>
{
     public async ValueTask<List<PricingStrategyDto>> Handle(GetAllPricingStrategiesQuery query, CancellationToken cancellationToken)
     {
          var strategies = new List<PricingStrategyDto>
          {
               new()
               {
                    Label = "Personnalisé",
                    Value = PricingStrategy.CustomStrategy
               },
               new()
               {
                    Label = "Course",
                    Value = PricingStrategy.SimpleDeliveryStrategy
               },
               new()
               {
                    Label = "Tournée",
                    Value = PricingStrategy.TourDeliveryStrategy
               }
          };

          return await Task.FromResult(strategies);
     }
}
