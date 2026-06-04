using FluentValidation;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Services.Repositories;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.Validators;

public class PackingSizeValidator : AbstractValidator<string>
{
     public PackingSizeValidator(IPackingSizeRepository packingSizes)
     {
          RuleFor(x => x).NotEmpty().Must(packingSize => packingSizes.GetByName(packingSize) != null)
               .WithMessage("Taille de colis saisie invalide.");
     }
}
