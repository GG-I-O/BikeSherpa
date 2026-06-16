using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.PricingStrategies;
using Ggio.BikeSherpa.Backend.Domain.SharedKernel;
using Ggio.BikeSherpa.Backend.Features.Reports.Model;

namespace Ggio.BikeSherpa.Backend.Features.Reports.Services;

public class ReportService(
     IEnumerable<IPricingStrategy> strategies,
     IDelayService delayService,
     IVatService vatService
) : IReportService
{
     public async Task<Report> GenerateReportAsync(string customerName, DateTimeOffset startDate, DateTimeOffset endDate, List<Delivery> deliveries)
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
                    DeliveryPriceWithVat = await vatService.GetPriceWithVatAsync(delivery.TotalPrice ?? 0),
                    Details = []
               };

               var strategy = strategies.SingleOrDefault(s => s.ImplementedStrategy == delivery.PricingStrategy);

               if (strategy is null)
               {
                    continue;
               }

               if (strategy.ImplementedStrategy == PricingStrategy.CustomStrategy)
               {
                    var stepReport = GetCustomStrategyStepDetail(delivery);

                    deliveryReport.Details.Add(stepReport);
               }
               else
               {
                    delivery.Steps = delivery.Steps.Where(s => !s.NotBilled).OrderBy(s => s.Order).ToList();
                    foreach (var deliveryStep in delivery.Steps)
                    {
                         var description = "";

                         if (strategy.ImplementedStrategy == PricingStrategy.SimpleDeliveryStrategy)
                         {
                              description = await GetSimpleDeliveryStepDescription(deliveryStep, delivery);
                         }
                         else if (strategy.ImplementedStrategy == PricingStrategy.TourDeliveryStrategy)
                         {
                              description = await GetTourDeliveryStepDescription(deliveryStep, delivery);
                         }

                         var stepReport = new DeliveryReportDetail
                         {
                              Description = description,
                              Address = deliveryStep.StepAddress,
                              Price = await strategy.GetStepPrice(delivery, deliveryStep),
                              Quantity = 1
                         };

                         deliveryReport.Details.Add(stepReport);
                    }

                    if (delivery.ExtraCost != 0)
                    {
                         var discountReport = new DeliveryReportDetail
                         {
                              Description = $"Options {delivery.ExtraCostReason ?? ""}",
                              Address = null,
                              Price = delivery.ExtraCost ?? 0,
                              Quantity = 1
                         };

                         deliveryReport.Details.Add(discountReport);
                    }
                    
                    if (delivery.Discount != 0)
                    {
                         var discountReport = new DeliveryReportDetail
                         {
                              Description = $"Remise : {delivery.DiscountReason ?? ""}",
                              Address = null,
                              Price = -1 * (delivery.Discount ?? 0),
                              Quantity = 1
                         };

                         deliveryReport.Details.Add(discountReport);
                    }

               }

               report.Deliveries.Add(deliveryReport);
               report.TotalPrice += deliveryReport.DeliveryPrice;
          }

          report.TotalPriceWithVat = await vatService.GetPriceWithVatAsync(report.TotalPrice);
          return report;
     }

     private async Task<string> GetTourDeliveryStepDescription(DeliveryStep deliveryStep, Delivery delivery)
     {
          var description = "";

          if (deliveryStep.StepType == StepType.Pickup)
          {
               description += "Ramassage ";
               description += $"{(await delayService.CalculateDelay(delivery.StartDate, delivery.ContractDate)).Label} ";
               description += $"{deliveryStep.StepAddress.City} ";
          }
          else // StepType.Dropoff
          {
               description += "Livraison ";
               description += $"(Colis {deliveryStep.PackingSize.Name}) ";
               description += $"{deliveryStep.StepAddress.City} ";
          }

          return description;
     }

     private async Task<string> GetSimpleDeliveryStepDescription(DeliveryStep deliveryStep, Delivery delivery)
     {
          var description = "";
          if (deliveryStep.StepType == StepType.Pickup)
          {
               var dropSteps = delivery.Steps.Where(s => s is { StepType: StepType.Dropoff, NotBilled: false }).ToList();
               var endStep = dropSteps[0];
               description += "Livraison ";
               description += $"{(await delayService.CalculateDelay(delivery.StartDate, delivery.ContractDate)).Label} ";
               description += $"(Colis {endStep?.PackingSize.Name}) : ";
               description += $"{deliveryStep.StepAddress.City} > ";
               description += endStep?.StepAddress.City;
          }
          else // StepType.Dropoff
          {
               description += "Transport ";
               description += $"{delivery.Urgency.Name} / ";
               description += $"{deliveryStep.Distance} km";
          }

          return description;
     }

     private static DeliveryReportDetail GetCustomStrategyStepDetail(Delivery delivery)
     {

          var stepReport = new DeliveryReportDetail
          {
               Description = "Prix fixe",
               Address = null,
               Price = delivery.TotalPrice ?? 0,
               Quantity = 1
          };

          return stepReport;
     }
}
