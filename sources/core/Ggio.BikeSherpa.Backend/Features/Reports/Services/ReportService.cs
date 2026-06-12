using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.PricingStrategies;
using Ggio.BikeSherpa.Backend.Features.Reports.Model;

namespace Ggio.BikeSherpa.Backend.Features.Reports.Services;

public class ReportService(
     IEnumerable<IPricingStrategy> strategies,
     IDelayService delayService
) : IReportService
{
     public Report GenerateReport(string customerName, DateTimeOffset startDate, DateTimeOffset endDate, List<Delivery> deliveries)
     {
          var report = new Report
          {
               CustomerName = customerName,
               StartDate = startDate,
               EndDate = endDate,
               TotalPrice = 0,
               TotalPriceWithVat = 0,
               Deliveries = []
          };

          foreach (var delivery in deliveries)
          {
               var deliveryReport = new DeliveryReport
               {
                    DeliveryCode = delivery.Code,
                    DeliveryDate = delivery.StartDate,
                    DeliveryPrice = delivery.TotalPrice ?? 0,
                    Details = []
               };

               var strategy = strategies.SingleOrDefault(s => s.ImplementedStrategy == delivery.PricingStrategy);

               foreach (var deliveryStep in delivery.Steps)
               {

                    var stepReport = new DeliveryReportDetail
                    {
                         Description = GetDeliveryStepDescription(deliveryStep, delivery),
                         Price = deliveryStep.StepZone.TourPrice,
                         Quantity = 1
                    };

                    deliveryReport.Details.Add(stepReport);
               }

               // var delayCost = strategy?.DelayCost(delivery.StartDate, delivery.ContractDate) ?? 0;
               // if (delayCost != 0)
               // {
               //      deliveryReport.Details.Add(new DeliveryReportDetail
               //      {
               //           Description = "Délai de la demande",
               //           Price = delayCost,
               //           Quantity = 1
               //      });
               // }
               //
               // var dropOffsInCore = delivery.Steps.Count(s => s.StepZone.Name == StepZone.InCore && s.StepType == StepType.Dropoff);
               // var inCoreCost = strategy?.GetStepsInCoreCost(dropOffsInCore) ?? 0;
               // if (inCoreCost > 0)
               // {
               //      deliveryReport.Details.Add(new DeliveryReportDetail
               //      {
               //           Description = "Livraison en centre-ville",
               //           Price = inCoreCost,
               //           Quantity = dropOffsInCore
               //      });
               // }
               //
               // var dropOffsInBorder = delivery.Steps.Count(s => s.StepZone.Name == StepZone.InBorder && s.StepType == StepType.Dropoff);
               // var inBorderCost = strategy?.GetStepsInBorderCost(dropOffsInBorder) ?? 0;
               // if (inBorderCost > 0)
               // {
               //      deliveryReport.Details.Add(new DeliveryReportDetail
               //      {
               //           Description = "Livraison en bordure",
               //           Price = inBorderCost,
               //           Quantity = dropOffsInBorder
               //      });
               // }
               //
               // var dropOffsInPeriphery = delivery.Steps.Count(s => s.StepZone.Name == StepZone.InPeriphery && s.StepType == StepType.Dropoff);
               // var inPeripheryCost = strategy?.GetStepsInPeripheryCost(dropOffsInPeriphery) ?? 0;
               // if (inPeripheryCost > 0)
               // {
               //      deliveryReport.Details.Add(new DeliveryReportDetail
               //      {
               //           Description = "Livraison en périphérie",
               //           Price = inPeripheryCost,
               //           Quantity = dropOffsInPeriphery
               //      });
               // }
               //
               // var dropOffsOutside = delivery.Steps.Count(s => s.StepZone.Name == StepZone.Outside && s.StepType == StepType.Dropoff);
               // var outsideCost = strategy?.GetOutsideStepsCost(dropOffsOutside) ?? 0;
               // if (outsideCost > 0)
               // {
               //      deliveryReport.Details.Add(new DeliveryReportDetail
               //      {
               //           Description = "Livraison en zone externe",
               //           Price = outsideCost,
               //           Quantity = dropOffsOutside
               //      });
               // }
               //
               //
               // var distance = delivery.Steps.Where(s => !s.NotBilled).Sum(s => s.Distance);
               //
               // var distanceCost = strategy?.TotalDistanceCost(distance, delivery.Urgency) ?? 0;
               // if (distanceCost > 0)
               // {
               //      deliveryReport.Details.Add(new DeliveryReportDetail
               //      {
               //           Description = "Cout distance",
               //           Price = Math.Round(distanceCost * 100) / 100,
               //           Quantity = 1
               //      });
               // }
               //
               // if (delivery.PricingStrategy == PricingStrategy.SimpleDeliveryStrategy)
               // {
               //      deliveryReport.Details.Add(new DeliveryReportDetail
               //      {
               //           Description = "Colisage",
               //           Price = 0, //TODO delivery.PackingSize.Price,
               //           Quantity = 1
               //      });
               // }
               //
               // if (delivery.PricingStrategy == PricingStrategy.TourDeliveryStrategy)
               // {
               //      deliveryReport.Details.Add(new DeliveryReportDetail
               //      {
               //           Description = "Colisage",
               //           Price = 0, //TODO delivery.PackingSize.TourPrice,
               //           Quantity = 1
               //      });
               // }
               //
               // if (delivery.Discount is not null && delivery.Discount > 0)
               // {
               //      deliveryReport.Details.Add(new DeliveryReportDetail
               //      {
               //           Description = "Remise",
               //           Price = -(delivery.Discount ?? 0),
               //           Quantity = 1
               //      });
               // }
               //
               // if (delivery.ExtraCost is not null && delivery.ExtraCost > 0)
               // {
               //      deliveryReport.Details.Add(new DeliveryReportDetail
               //      {
               //           Description = "Surcout",
               //           Price = delivery.ExtraCost ?? 0,
               //           Quantity = 1
               //      });
               // }

               report.Deliveries.Add(deliveryReport);
               report.TotalPrice += deliveryReport.DeliveryPrice;
          }

          return report;
     }

     private string GetDeliveryStepDescription(DeliveryStep deliveryStep, Delivery delivery)
     {
          switch (deliveryStep.StepType)
          {
               case StepType.Pickup:

                    return $"Livraison {deliveryStep.PackingSize.Name} {delayService.CalculateDelay(delivery.StartDate, delivery.ContractDate).Label} {deliveryStep.PackingSize.Name} ";
               case StepType.Dropoff:
                    return $"Transport {deliveryStep.PackingSize.Name} {deliveryStep.Distance} km";
               default:
                    return "Undefined";
          }
     }
}
