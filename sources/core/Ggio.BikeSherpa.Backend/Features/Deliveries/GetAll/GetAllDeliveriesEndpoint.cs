using FastEndpoints;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Features.Courses.GetAll;

public class GetAllDeliveriesEndpoint(IMediator mediator) : EndpointWithoutRequest<List<DeliveryCrud>>
{
     public override void Configure()
     {
          Get("/api/courses");
          Claims("scope", "read:customers");
     } 

     public override async Task HandleAsync(CancellationToken ct)
     {
          var courses = await mediator.Send(new GetAllCoursesQuery(), ct);
          await Send.OkAsync(courses, cancellation: ct);
     }
}
