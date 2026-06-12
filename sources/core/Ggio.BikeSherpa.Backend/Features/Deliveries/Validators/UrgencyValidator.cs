using FluentValidation;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Spi;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.Validators;

public class UrgencyValidator : AbstractValidator<string>
{
     public UrgencyValidator(IUrgencyRepository urgencies)
     {
          RuleFor(x => x).NotEmpty()
               .Must(urgency => urgencies.GetByName(urgency) != null)
               .WithMessage("Valeur d'urgence saisie invalide.");
     }
}
