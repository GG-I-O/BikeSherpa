using Ardalis.Result;
using FluentValidation;
using Ggio.BikeSherpa.Backend.Domain;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Specification;
using Ggio.DddCore;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.Update;

public record UpdateDeliveryCommand(
     Guid Id,
     PricingStrategy PricingStrategy,
     DeliveryStatus Status,
     string Code,
     Guid CustomerId,
     Urgency Urgency,
     double TotalPrice,
     Guid ReportId,
     List<DeliveryStep> Steps,
     string[] Details,
     double Weight,
     int Length,
     Packing Packing,
     DateTimeOffset ContractDate,
     DateTimeOffset StartDate
) : ICommand<Result>;

public class UpdateDeliveryCommandValidator : AbstractValidator<UpdateDeliveryCommand>
{
     public UpdateDeliveryCommandValidator(IReadRepository<Delivery> repository)
     {
          RuleFor(x => x.Id).NotEmpty();
          RuleFor(x => x.PricingStrategy).NotEmpty();
          RuleFor(x => x.Status).NotEmpty();
          RuleFor(x => x.Code).NotEmpty();
          RuleFor(x => x.CustomerId).NotNull();
          RuleFor(x => x.Urgency).NotNull();
          RuleFor(x => x.TotalPrice).NotEmpty();
          RuleFor(x => x.ReportId).NotEmpty();
          RuleFor(x => x.Steps).NotEmpty();
          RuleFor(x => x.Details).NotEmpty();
          RuleFor(x => x.Weight).NotEmpty();
          RuleFor(x => x.Length).NotEmpty();
          RuleFor(x => x.Packing).NotEmpty();
          RuleFor(x => x.ContractDate).NotEmpty();
          RuleFor(x => x.StartDate).NotEmpty();
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
         
          entity.PricingStrategy = command.PricingStrategy;
          entity.Status = command.Status;
          entity.Code = command.Code;
          entity.CustomerId = command.CustomerId;
          entity.Urgency = command.Urgency;
          entity.TotalPrice = command.TotalPrice;
          entity.ReportId = command.ReportId;
          entity.Steps = command.Steps;
          entity.Details = command.Details;
          entity.Weight = command.Weight;
          entity.Length = command.Length;
          entity.Packing = command.Packing;
          entity.ContractDate = command.ContractDate;
          entity.StartDate = command.StartDate;
          await transaction.CommitAsync(cancellationToken);
          return Result.Success();
     }
}