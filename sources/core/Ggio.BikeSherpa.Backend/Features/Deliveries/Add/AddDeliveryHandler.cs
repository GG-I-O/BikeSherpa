using Ardalis.Result;
using FluentValidation;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.DddCore;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.Add;

public record AddDeliveryCommand(
     string Code,
     string CustomerId,
     double TotalPrice,
     string ReportId,
     string[] Steps,
     string[] Details,
     string Packing
     ) : ICommand<Result<Guid>>;

public class AddDeliveryCommandValidator : AbstractValidator<AddDeliveryCommand>
{
     public AddDeliveryCommandValidator(IReadRepository<Delivery> repository)
     {
          RuleFor(x => x.Code).NotEmpty();
          RuleFor(x => x.CustomerId).NotNull();
          RuleFor(x => x.TotalPrice).NotEmpty();
          RuleFor(x => x.ReportId).NotEmpty();
          RuleFor(x => x.Steps).NotEmpty();
          RuleFor(x => x.Details).NotEmpty();
          RuleFor(x => x.Packing).NotEmpty();
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
               command.Code,
               command.CustomerId,
               command.TotalPrice,
               command.ReportId,
               command.Steps,
               command.Details,
               command.Packing
               );
          
         await transaction.CommitAsync(cancellationToken);
          return Result<Guid>.Success(delivery.Id);
     }
}
