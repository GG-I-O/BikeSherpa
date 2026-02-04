using Ardalis.Result;
using FluentValidation;
using Ggio.BikeSherpa.Backend.Domain.CustomerAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Specification;
using Ggio.DddCore;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.Update;

public record UpdateDeliveryCommand(
     Guid Id,
     string Code,
     string CustomerId,
     double TotalPrice,
     string ReportId,
     string[] StepIds,
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
          RuleFor(x => x.StepIds).NotEmpty();
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
         
          entity.Code = command.Code;
          entity.CustomerId = command.CustomerId;
          entity.TotalPrice = command.TotalPrice;
          entity.ReportId = command.ReportId;
          entity.StepIds = command.StepIds;
          entity.Details = command.Details;
          entity.Packing = command.Packing;
          await transaction.CommitAsync(cancellationToken);
          return Result.Success();
     }
}