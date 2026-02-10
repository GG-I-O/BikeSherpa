using FastEndpoints;
using Ggio.BikeSherpa.Backend.Features.Deliveries.Get;
using Ggio.BikeSherpa.Backend.Features.Deliveries.Model;
using Ggio.BikeSherpa.Backend.Model;
using Mediator;
using Microsoft.AspNetCore.Http;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.Add;

public class AddDeliveryEndpoint(IMediator mediator) : Endpoint<DeliveryCrud, AddResult<Guid>>
{
     public override void Configure()
     {
          Post("/delivery");
          Policies("write:deliveries");
          Description(x => x.WithTags("delivery"));
     }

     public override async Task HandleAsync(DeliveryCrud req, CancellationToken ct)
     {
          var command = new AddDeliveryCommand(
               req.PricingStrategy,
               req.Status,
               req.Code,
               req.CustomerId,
               req.Urgency,
               req.TotalPrice,
               req.ReportId,
               req.Steps,
               req.Details,
               req.Weight,
               req.Length,
               req.Packing,
               req.ContractDate,
               req.StartDate
               );
          
          var result = await mediator.Send(command, ct);
         
          await Send.CreatedAtAsync<GetDeliveryEndpoint>(result.Value, new AddResult<Guid>(result.Value), cancellation: ct);
     }
}
