using FastEndpoints;
using Ggio.BikeSherpa.Backend.Features.Courses.Get;
using Ggio.BikeSherpa.Backend.Model;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Features.Courses.Add;

public class AddCourseEndpoint(IMediator mediator) : Endpoint<CourseCrud, AddResult<Guid>>
{
     public override void Configure()
     {
          Post("/api/course");
          Permissions("write:customers");
     }

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
