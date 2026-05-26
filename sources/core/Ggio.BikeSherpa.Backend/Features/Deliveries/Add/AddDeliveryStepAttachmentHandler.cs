using Ardalis.Result;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Specification;
using Ggio.DddCore;
using Mediator;
using Microsoft.AspNetCore.Http;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.Add;

public record AddDeliveryStepAttachmentCommand(
    Guid DeliveryId,
    Guid StepId,
    IFormFile File
): ICommand<Result>;

public class AddDeliveryStepAttachmentHandler(
    IReadRepository<Delivery> deliveryRepository,
    IApplicationTransaction transaction
    ): ICommandHandler<AddDeliveryStepAttachmentCommand, Result>
{
    public async ValueTask<Result> Handle(AddDeliveryStepAttachmentCommand command, CancellationToken cancellationToken)
    {
        var delivery = await deliveryRepository.FirstOrDefaultAsync(new DeliveryByIdSpecification(command.DeliveryId), cancellationToken);
        if (delivery is null)
            return Result.NotFound();
        
        var step = delivery.Steps.FirstOrDefault(s => s.Id == command.StepId);
        if (step is null)
            return Result.NotFound();
        
        await using var stream = command.File.OpenReadStream();
        
        // TODO: Save file to storage service and get name

        step.AttachmentFilePaths = (step.AttachmentFilePaths ?? [])
            .Append(command.File.Name) // Temporary, to change with file URL once stored
            .ToArray();
        
        await transaction.CommitAsync(cancellationToken);
        return Result.Success();
    }
}
