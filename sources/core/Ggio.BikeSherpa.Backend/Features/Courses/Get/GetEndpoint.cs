using FastEndpoints;

namespace Ggio.BikeSherpa.BackendSaaS.Features.Courses.Get;

public class GetEndpoint : EndpointWithoutRequest<CourseDto>
{
     public override void Configure() => Get("/api/course/{Id:guid}");

     public override async Task HandleAsync(CancellationToken ct)
     {
          await Send.OkAsync(new CourseDto("id", DateTimeOffset.Now), ct);
     }
}
