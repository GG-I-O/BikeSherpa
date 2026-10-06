using Ardalis.Result;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Specification;
using Ggio.BikeSherpa.Backend.Infrastructure.Storage;
using Ggio.DddCore;
using Mediator;
using Microsoft.AspNetCore.Http;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.Sign;

public record SignDeliveryStepCommand(
     Guid DeliveryId,
     Guid StepId,
     IFormFile Signature,
     string DomainType,
     string Receiver
) : ICommand<Result>;

public class SignDeliveryStepHandler(
     IReadRepository<Delivery> deliveryRepository,
     IDeliveryStepAttachmentSaveService attachmentSaveService,
     IApplicationTransaction transaction
     ) : ICommandHandler<SignDeliveryStepCommand, Result>
{
     public async ValueTask<Result> Handle(SignDeliveryStepCommand command, CancellationToken cancellationToken)
     {
          var delivery = await deliveryRepository.FirstOrDefaultAsync(new DeliveryByIdSpecification(command.DeliveryId), cancellationToken);
          if (delivery is null)
          {
               return Result.NotFound();
          }

          var step = delivery.Steps.FirstOrDefault(s => s.Id == command.StepId);
          if (step is null)
          {
               return Result.NotFound();
          }

          await using var stream = command.Signature.OpenReadStream();

          var url = await attachmentSaveService.StoreFileAsync(stream, command.Signature.FileName, command.Signature.ContentType, command.DomainType, cancellationToken);

          step.AddSignature(url, command.DomainType, command.Receiver);

          await transaction.CommitAsync(cancellationToken);
          return Result.Success();
     }
}
