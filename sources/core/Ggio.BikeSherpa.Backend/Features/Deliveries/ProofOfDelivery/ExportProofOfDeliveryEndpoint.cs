using FastEndpoints;
using Ggio.BikeSherpa.Backend.Features.Reports.Customer;
using Ggio.BikeSherpa.Backend.Infrastructure.Storage;
using Mediator;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.ProofOfDelivery;

public record ExportProofOfDeliveryRequest(Guid DeliveryId);

public class ExportProofOfDeliveryEndpoint(
     IMediator mediator,
     IExportSaveService exportSaveService,
     ILogger<ExportReportEndpoint> logger
     ): Endpoint<ExportProofOfDeliveryRequest, string>
{
     public override void Configure()
     {
          Get("/delivery/{deliveryId:guid}/proofOfDelivery");
          Policies("read:deliveries");
          Description(x => x.WithTags("delivery")
               .WithName("ExportProofOfDelivery")
               .Produces<string>());
     }

     public override async Task HandleAsync(ExportProofOfDeliveryRequest request, CancellationToken cancellationToken)
     {
          var command = new ExportProofOfDeliveryCommand(
               request.DeliveryId
          );

          var result = await mediator.Send(command, cancellationToken);

          var url = await exportSaveService.SaveProofOfDeliveryAsync(
               result.FileName,
               result.Content,
               result.ContentType,
               cancellationToken
          );

          logger.LogDebug("Preuve de livraison {FileName} enregistré avec succès à l'url {Url}", result.FileName, url);
          await Send.OkAsync(url, cancellationToken);
     }
}
