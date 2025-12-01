using FastEndpoints;
using Ggio.BikeSherpa.BackendSaaS.Features.Courses.Get;
using Ggio.BikeSherpa.BackendSaaS.Model;
using Mediator;

namespace Ggio.BikeSherpa.BackendSaaS.Features.Courses.Add;

public class AddCourseEndpoint(IMediator mediator) : Endpoint<CourseCrud, AddResult<Guid>>
{
     public override void Configure() => Post("/api/course");

     public override async Task HandleAsync(CourseCrud req, CancellationToken ct)
     {
          var command = new AddCourseCommand(req.StartDate);
          var result = await mediator.Send(command, ct);
          if (result.IsSuccess)
          {
               await Send.CreatedAtAsync<GetEndpoint>(new AddResult<Guid>(result.Value), cancellation: ct);
          }
         
     }
}
