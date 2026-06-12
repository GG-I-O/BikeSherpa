using Ggio.BikeSherpa.Backend.Domain.CustomerAggregate;
using Ggio.BikeSherpa.Backend.Domain.CustomerAggregate.Specifications;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Services.PricingStrategy;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Specification;
using Ggio.DddCore;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;

public record DeliveryFactoryParameters(
     PricingStrategy PricingStrategy,
     Guid CustomerId,
     Urgency Urgency,
     double? TotalPrice,
     double? Discount,
     double? ExtraCost,
     string[] Details,
     bool InsulatedBox,
     DateTimeOffset ContractDate,
     DateTimeOffset StartDate,
     bool NeedEstimate,
     string? DiscountReason,
     string? ExtraCostReason);

public interface IDeliveryFactory
{
     Task<Delivery> CreateDeliveryAsync(DeliveryFactoryParameters parameters);
}

public class DeliveryFactory(
     IMediator mediator,
     IReadRepository<Customer> customerRepository,
     IReadRepository<Delivery> deliveryRepository,
     IPricingStrategyService pricingStrategyService
) : FactoryBase(mediator), IDeliveryFactory
{
     public async Task<Delivery> CreateDeliveryAsync(DeliveryFactoryParameters parameters)
     {
          var delivery = new Delivery
          {
               Id = Guid.NewGuid(),
               PricingStrategy = parameters.PricingStrategy,
               Code = "",
               CustomerId = parameters.CustomerId,
               Urgency = parameters.Urgency,
               TotalPrice = parameters.TotalPrice,
               Discount = parameters.Discount,
               ExtraCost = parameters.ExtraCost,
               Details = parameters.Details,
               Steps = [],
               InsulatedBox = parameters.InsulatedBox,
               ContractDate = parameters.ContractDate,
               StartDate = parameters.StartDate,
               NeedEstimate = parameters.NeedEstimate,
               DiscountReason = parameters.DiscountReason,
               ExtraCostReason = parameters.ExtraCostReason
          };

          var customer = await customerRepository.FirstOrDefaultAsync(new CustomerByIdSpecification(parameters.CustomerId));

          if (customer is not null)
          {
               delivery.GenerateReportId(customer);

               delivery.GenerateCode(customer, 0);
               var splitCode = delivery.Code.Split('-');
               var codeStart = $"{splitCode[0]}-{splitCode[1]}";
               var deliveries = await deliveryRepository.ListAsync(new DeliveryByCodeStartsWithSpecification(codeStart));
               delivery.GenerateCode(customer, deliveries.Count + 1);
          }

          delivery.TotalPrice = pricingStrategyService.CalculateDeliveryPriceWithoutVat(delivery);

          await NotifyNewAggregateRootAdded(delivery);

          return delivery;
     }
}
