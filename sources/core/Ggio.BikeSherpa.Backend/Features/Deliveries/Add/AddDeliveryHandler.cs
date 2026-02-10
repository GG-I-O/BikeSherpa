using Ardalis.Result;
using FluentValidation;
using Ggio.BikeSherpa.Backend.Domain;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;
using Ggio.DddCore;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.Add;

public record AddDeliveryCommand(
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
     ) : ICommand<Result<Guid>>;

public class AddDeliveryCommandValidator : AbstractValidator<AddDeliveryCommand>
{
     public AddDeliveryCommandValidator(IReadRepository<Delivery> repository)
     {
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

public class AddDeliveryHandler(
     IDeliveryFactory factory,
     IValidator<AddDeliveryCommand> validator,
     IApplicationTransaction transaction) : ICommandHandler<AddDeliveryCommand, Result<Guid>>
{
     public async ValueTask<Result<Guid>> Handle(AddDeliveryCommand command, CancellationToken cancellationToken)
     {
          await validator.ValidateAndThrowAsync(command, cancellationToken);
          
          var delivery = await factory.CreateDeliveryAsync(
               command.PricingStrategy,
               command.Status,
               command.Code,
               command.CustomerId,
               command.Urgency,
               command.TotalPrice,
               command.ReportId,
               command.Steps,
               command.Details,
               command.Weight,
               command.Length,
               command.Packing,
               command.ContractDate,
               command.StartDate
               );
          
         await transaction.CommitAsync(cancellationToken);
          return Result<Guid>.Success(delivery.Id);
     }
}
