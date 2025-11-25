using FastEndpoints;
using Mediator;

namespace Ggio.BikeSherpa.BackendSaaS.Features.Courses.Get;

public class GetEndpoint(IMediator mediator) : EndpointWithoutRequest<CourseCrud>
{
     public override void Configure() => Get("/api/course/{Id:guid}");

     public override async Task HandleAsync(CancellationToken ct)
     {
          var entity = await mediator.Send(new GetCourseQuery(Query<Guid>("Id")), ct);
          if (entity is null)
          {
               await Send.NotFoundAsync(ct);
          }

          await Send.OkAsync(entity!, ct);
     }
}
