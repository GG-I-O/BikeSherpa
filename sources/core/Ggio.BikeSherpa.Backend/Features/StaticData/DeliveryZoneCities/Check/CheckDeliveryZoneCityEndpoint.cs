using FastEndpoints;
using FluentValidation;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Spi;
using Microsoft.AspNetCore.Http;

namespace Ggio.BikeSherpa.Backend.Features.StaticData.DeliveryZoneCities.Check;

public record CheckDeliveryZoneCityQuery
{
     [QueryParam]
     public required string City { get; init; }
}

public record CheckDeliveryZoneCityResponse
{
     public required bool IsInZone { get; init; }
}

public class CheckDeliveryZoneCityQueryValidator : AbstractValidator<CheckDeliveryZoneCityQuery>
{
     public CheckDeliveryZoneCityQueryValidator()
     {
          RuleFor(x => x.City).NotEmpty();
     }
}

public class CheckDeliveryZoneCityEndpoint
(
     IDeliveryZoneRepository repository,
     IValidator<CheckDeliveryZoneCityQuery> validator
     ) : Endpoint<CheckDeliveryZoneCityQuery, CheckDeliveryZoneCityResponse>
{
     public override void Configure()
     {
          AllowAnonymous();
          Post("/deliveryZoneCities/check");
          Description(x => x.WithTags("deliveryZoneCities")
               .WithDescription("Check if a city exists in possible zones")
               .Produces<CheckDeliveryZoneCityResponse>()
          );
     }
     
     public override async Task HandleAsync(CheckDeliveryZoneCityQuery req, CancellationToken ct)
     {
          await validator.ValidateAndThrowAsync(req, ct);
          
          var isInZone = repository.IsInZone(req.City);

          await Send.OkAsync(new CheckDeliveryZoneCityResponse
          {
               IsInZone = isInZone
          }, ct);
     }
}
