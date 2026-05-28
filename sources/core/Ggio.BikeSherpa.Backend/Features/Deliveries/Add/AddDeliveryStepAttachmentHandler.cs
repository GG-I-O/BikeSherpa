using Ardalis.Result;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Specification;
using Ggio.BikeSherpa.Backend.Infrastructure.Storage;
using Ggio.DddCore;
using Mediator;
using Microsoft.AspNetCore.Http;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.Add;

public record AddDeliveryStepAttachmentCommand(
     Guid DeliveryId,
     Guid StepId,
     IFormFile File
) : ICommand<Result>;

public class AddDeliveryStepAttachmentHandler(
     IReadRepository<Delivery> deliveryRepository,
     IDeliveryStepAttachmentSaveService attachmentSaveService,
     IApplicationTransaction transaction
) : ICommandHandler<AddDeliveryStepAttachmentCommand, Result>
{
     public async ValueTask<Result> Handle(AddDeliveryStepAttachmentCommand command, CancellationToken cancellationToken)
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

          await using var stream = command.File.OpenReadStream();

          var url = await attachmentSaveService.StoreFileAsync(stream, command.File.FileName, command.File.ContentType, cancellationToken);

          step.AddAttachment(url);

          await transaction.CommitAsync(cancellationToken);
          return Result.Success();
     }
}
