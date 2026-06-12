using FluentValidation;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Services.PricingStrategy;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Spi;
using Ggio.BikeSherpa.Backend.Domain.SharedKernel;
using Ggio.BikeSherpa.Backend.Features.Deliveries.Model;
using Ggio.BikeSherpa.Backend.Features.Deliveries.Validators;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.PriceCalculation;

public record CalculateDeliveryPriceQuery(DeliveryCrud Delivery) : IQuery<CalculateDeliveryPriceResult>;

public record CalculateDeliveryPriceResult(double Price, double PriceWithVat, double TotalDistance);

public class CalculateDeliveryPriceQueryValidator : AbstractValidator<CalculateDeliveryPriceQuery>
{
     public CalculateDeliveryPriceQueryValidator(IUrgencyRepository urgencyRepository)
     {
          RuleFor(x => x.Delivery).NotNull();
          RuleFor(x => x.Delivery.Urgency).SetValidator(new UrgencyValidator(urgencyRepository));
          RuleFor(x => x.Delivery.PricingStrategy)
               .Must(val => val is PricingStrategy.SimpleDeliveryStrategy or PricingStrategy.TourDeliveryStrategy)
               .WithMessage("Le type de tarification pour le calcul doit être 'Course' ou 'Tournée'");

          RuleFor(x => x.Delivery.Steps).NotEmpty()
               .Must(steps => steps.Any())
               .WithMessage("Les étapes de la livraison ne peuvent pas être vides");
     }
}

public class CalculateDeliveryPriceHandler(
     IPricingStrategyService pricingStrategyService,
     IUrgencyRepository urgencyRepository,
     IPackingSizeRepository packingSizeRepository,
     IValidator<CalculateDeliveryPriceQuery> validator,
     IItinerarySpi itineraryService,
     IVatService vatService) : IQueryHandler<CalculateDeliveryPriceQuery, CalculateDeliveryPriceResult>
{
     public async ValueTask<CalculateDeliveryPriceResult> Handle(CalculateDeliveryPriceQuery query, CancellationToken cancellationToken)
     {
          await validator.ValidateAndThrowAsync(query, cancellationToken);
          var urgency = urgencyRepository.GetByName(query.Delivery.Urgency)!;

          var delivery = new Delivery
          {
               PricingStrategy = query.Delivery.PricingStrategy,
               Code = "temp",
               CustomerId = Guid.NewGuid(),
               Urgency = urgency,
               Steps = query.Delivery.Steps
                    .Where(step => step.Data.StepAddress.StreetInfo != string.Empty)
                    .Select(step => DeliveryStepCrudMapper
                         .Map(step.Data, packingSizeRepository)
                    ).ToList(),
               InsulatedBox = query.Delivery.InsulatedBox,
               StartDate = query.Delivery.StartDate,
               ContractDate = query.Delivery.ContractDate
          };

          await delivery.RecalculateStepDistancesAsync(itineraryService);

          var price = pricingStrategyService.CalculateDeliveryPriceWithoutVat(delivery);
          var priceWithVat = await vatService.GetPriceWithVatAsync(price);
          var totalDistance = delivery.GetTotalDistance();

          return new CalculateDeliveryPriceResult(price, priceWithVat, totalDistance);
     }
}
