using FastEndpoints;
using Ggio.BikeSherpa.Backend.Features.StaticData.PackingSizes.Model;
using Mediator;
using Microsoft.AspNetCore.Http;

namespace Ggio.BikeSherpa.Backend.Features.StaticData.PackingSizes.GetAll;

public class GetAllPackingSizesEndpoint(IMediator mediator) : EndpointWithoutRequest<List<PackingSizeDto>>
{
     public override void Configure()
     {
          Get("/general/packingSizes");
          Description(x => x.WithTags("general"));
          AllowAnonymous();
     }

     public override async Task HandleAsync(CancellationToken ct)
     {
          var query = new GetAllPackingSizesQuery();
          var packingSizes = await mediator.Send(query, ct);
          await Send.OkAsync(packingSizes, ct);
     }
}
