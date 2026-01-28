using FastEndpoints;
using Ggio.BikeSherpa.Backend.Features.Courses.Get;
using Ggio.BikeSherpa.Backend.Model;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Features.Courses.Add;

public class AddDeliveryEndpoint(IMediator mediator) : Endpoint<DeliveryCrud, AddResult<Guid>>
{
     public override void Configure()
     {
          Post("/api/course");
          Permissions("write:customers");
     }

     public override async Task HandleAsync(DeliveryCrud req, CancellationToken ct)
     {
          var command = new AddCourseCommand(req.StartDate);
          var result = await mediator.Send(command, ct);
          if (result.IsSuccess)
          {
               await Send.CreatedAtAsync<GetDeliveryEndpoint>(new AddResult<Guid>(result.Value), cancellation: ct);
          }
         
     }
}
