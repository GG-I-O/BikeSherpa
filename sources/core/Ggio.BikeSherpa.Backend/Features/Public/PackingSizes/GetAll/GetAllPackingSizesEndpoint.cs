using FastEndpoints;
using Ggio.BikeSherpa.Backend.Features.Public.PackingSizes.Model;
using Mediator;
using Microsoft.AspNetCore.Http;

namespace Ggio.BikeSherpa.Backend.Features.Public.PackingSizes.GetAll;

public class GetAllPackingSizesEndpoint(IMediator mediator) : EndpointWithoutRequest<List<PackingSizeDto>>
{
     public override void Configure()
     {
          Get("/public/packingSizes");
          AllowAnonymous(Http.GET);
          Description(x => x.WithTags("public"));
     }

     public override async Task HandleAsync(CancellationToken ct)
     {
          var query = new GetAllPackingSizesQuery();
          var packingSizes = await mediator.Send(query, ct);
          await Send.OkAsync(packingSizes, ct);
     }
}
