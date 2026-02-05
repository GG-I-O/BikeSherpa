using Ardalis.Result;
using FluentValidation;
using Ggio.BikeSherpa.Backend.Domain.CustomerAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Specification;
using Ggio.DddCore;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.Update;

public record UpdateDeliveryCommand(
     Guid Id,
     DeliveryStatus Status,
     string Code,
     Guid CustomerId,
     double TotalPrice,
     Guid ReportId,
     List<DeliveryStep> Steps,
     string[] Details,
     string Packing
) : ICommand<Result>;

public class UpdateDeliveryCommandValidator : AbstractValidator<UpdateDeliveryCommand>
{
     public UpdateDeliveryCommandValidator(IReadRepository<Delivery> repository)
     {
          RuleFor(x => x.Id).NotEmpty();
          RuleFor(x => x.Code).NotEmpty();
          RuleFor(x => x.CustomerId).NotNull();
          RuleFor(x => x.TotalPrice).NotEmpty();
          RuleFor(x => x.ReportId).NotEmpty();
          RuleFor(x => x.Steps).NotEmpty();
          RuleFor(x => x.Details).NotEmpty();
          RuleFor(x => x.Packing).NotEmpty();
     }
}

public class UpdateDeliveryHandler(
     IReadRepository<Delivery> repository,
     IValidator<UpdateDeliveryCommand> validator,
     IApplicationTransaction transaction
) : ICommandHandler<UpdateDeliveryCommand, Result>
{
     public async ValueTask<Result> Handle(UpdateDeliveryCommand command, CancellationToken cancellationToken)
     {
          await validator.ValidateAndThrowAsync(command, cancellationToken);
          var entity = await repository.FirstOrDefaultAsync(new DeliveryByIdSpecification(command.Id), cancellationToken);
          if (entity is null)
               return Result.NotFound();
         
          entity.Status = command.Status;
          entity.Code = command.Code;
          entity.CustomerId = command.CustomerId;
          entity.TotalPrice = command.TotalPrice;
          entity.ReportId = command.ReportId;
          entity.Steps = command.Steps;
          entity.Details = command.Details;
          entity.Packing = command.Packing;
          await transaction.CommitAsync(cancellationToken);
          return Result.Success();
     }
}