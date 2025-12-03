using FastEndpoints;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Features.Courses.GetAll;

public class GetAllEndpoint(IMediator mediator) : EndpointWithoutRequest<List<CourseCrud>>
{
     public override void Configure()
     {
          Get("/api/courses");
          Permissions("read:customers");
     } 

     public override async Task HandleAsync(CancellationToken ct)
     {
          var courses = await mediator.Send(new GetAllCoursesQuery(), ct);
          await Send.OkAsync(courses, cancellation: ct);
     }
}
