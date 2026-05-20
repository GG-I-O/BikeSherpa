using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.PricingStrategies;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Specification;
using Ggio.BikeSherpa.Backend.Features.Reports.Model;
using Ggio.DddCore;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Features.Reports.GetAll;

public record GetAllReportsQuery(
     Guid CustomerId,
     DateTimeOffset From,
     DateTimeOffset To
) : IQuery<List<Report>>;

public class GetAllReportsHandler(
     IReadRepository<Delivery> repository,
     IEnumerable<IPricingStrategy> strategies
) : IQueryHandler<GetAllReportsQuery, List<Report>>
{
     public async ValueTask<List<Report>> Handle(GetAllReportsQuery query, CancellationToken cancellationToken)
     {
          var deliveries = await repository
               .ListAsync(
                    new DeliveryByCustomerAndDateRangeSpecification(
                         query.CustomerId,
                         query.From,
                         query.To
                    )
                    , cancellationToken
               );

          var reports = new List<Report>();
          foreach (var delivery in deliveries)
          {
               var report = new Report
               {
                    DeliveryCode = delivery.Code,
                    DeliveryDate = delivery.StartDate,
                    DeliveryPrice = delivery.TotalPrice ?? 0,
                    Details = []
               };

               var strategy = strategies.SingleOrDefault(s => s.ImplementedStrategy == delivery.PricingStrategy);

               var sameDayExtraCost = strategy?.SameDayExtraCost(delivery.StartDate, delivery.ContractDate) ?? 0;
               if (sameDayExtraCost != 0)
                    report.Details.Add(new ReportDetail
                    {
                         Description = "Demande le même jour",
                         Price = sameDayExtraCost,
                         Quantity = 1
                    });

               var delayCost = strategy?.DelayCost(delivery.StartDate, delivery.ContractDate) ?? 0;
               if (delayCost != 0)
                    report.Details.Add(new ReportDetail
                    {
                         Description = "Délai de la demande",
                         Price = delayCost,
                         Quantity = 1
                    });

               var pickupCount = delivery.Steps.Count(s => s.StepType == StepType.Pickup);
               var pickupCost = strategy?.PickupCost(pickupCount) ?? 0;
               if (pickupCost > 0)
                    report.Details.Add(new ReportDetail
                    {
                         Description = "Prise en charge",
                         Price = pickupCost,
                         Quantity = pickupCount
                    });

               var dropOffsInCore = delivery.Steps.Count(s => s.StepZone.Name == StepZone.InCore && s.StepType == StepType.Dropoff);
               var inCoreCost = strategy?.DropOffInCoreCost(dropOffsInCore) ?? 0;
               if (inCoreCost > 0)
                    report.Details.Add(new ReportDetail
                    {
                         Description = "Livraison en centre-ville",
                         Price = inCoreCost,
                         Quantity = dropOffsInCore
                    });

               var dropOffsInBorder = delivery.Steps.Count(s => s.StepZone.Name == StepZone.InBorder && s.StepType == StepType.Dropoff);
               var inBorderCost = strategy?.DropOffInBorderCost(dropOffsInBorder) ?? 0;
               if (inBorderCost > 0)
                    report.Details.Add(new ReportDetail
                    {
                         Description = "Livraison en bordure",
                         Price = inBorderCost,
                         Quantity = dropOffsInBorder
                    });

               var dropOffsInPeriphery = delivery.Steps.Count(s => s.StepZone.Name == StepZone.InPeriphery && s.StepType == StepType.Dropoff);
               var inPeripheryCost = strategy?.DropOffInPeripheryCost(dropOffsInPeriphery) ?? 0;
               if (inPeripheryCost > 0)
                    report.Details.Add(new ReportDetail
                    {
                         Description = "Livraison en périphérie",
                         Price = inPeripheryCost,
                         Quantity = dropOffsInPeriphery
                    });

               var dropOffsOutside = delivery.Steps.Count(s => s.StepZone.Name == StepZone.Outside && s.StepType == StepType.Dropoff);
               var outsideCost = strategy?.DropOffOutsideCost(dropOffsOutside) ?? 0;
               if (outsideCost > 0)
                    report.Details.Add(new ReportDetail
                    {
                         Description = "Livraison en zone externe",
                         Price = outsideCost,
                         Quantity = dropOffsOutside
                    });

               var distance = delivery.Distance ?? 0;
               if (distance == 0)
                    distance = delivery.Steps.Where(s => !s.NotBilled).Sum(s => s.Distance);
               var distanceCost = strategy?.TotalDistanceCost(distance, delivery.Urgency) ?? 0;
               if (distanceCost > 0)
                    report.Details.Add(new ReportDetail
                    {
                         Description = "Cout distance",
                         Price = Math.Round(distanceCost *100) / 100,
                         Quantity = 1
                    });
               
               if (delivery.PricingStrategy == PricingStrategy.SimpleDeliveryStrategy)
                    report.Details.Add(new ReportDetail
                    {
                         Description = "Colisage",
                         Price = delivery.PackingSize.Price,
                         Quantity = 1
                    });
               if (delivery.PricingStrategy == PricingStrategy.TourDeliveryStrategy)
                    report.Details.Add(new ReportDetail
                    {
                         Description = "Colisage",
                         Price = delivery.PackingSize.TourPrice,
                         Quantity = 1
                    });
               
               if (delivery.Discount is not null && delivery.Discount > 0)
                    report.Details.Add(new ReportDetail
                    {
                         Description = "Remise",
                         Price = -(delivery.Discount ?? 0),
                         Quantity = 1
                    });
               
               if (delivery.ExtraCost is not null && delivery.ExtraCost > 0)
                    report.Details.Add(new ReportDetail
                    {
                         Description = "Surcout",
                         Price = delivery.ExtraCost ?? 0,
                         Quantity = 1
                    });

               reports.Add(report);
          }

          return reports;
     }
}
