using Ardalis.Result;
using FluentValidation;
using Ggio.BikeSherpa.Backend.Domain;
using Ggio.BikeSherpa.Backend.Domain.CustomerAggregate;
using Ggio.DddCore;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Features.Customers.Add;

public record AddCustomerCommand(
     string Name,
     string Code,
     string? Siret,
     string Email,
     string PhoneNumber,
     Address Address
) : ICommand<Result<Guid>>;

public class AddCustomerCommandValidator : AbstractValidator<AddCustomerCommand>
{
     public AddCustomerCommandValidator()
     {
          RuleFor(x => x.Name).NotEmpty();
          RuleFor(x => x.Code).NotEmpty();
          RuleFor(x => x.Email).NotEmpty();
          RuleFor(x => x.PhoneNumber).NotEmpty();
          RuleFor(x => x.Address).NotEmpty();
     }
}

public class AddCustomerHandler(
     ICustomerFactory factory,
     IValidator<AddCustomerCommand> validator,
     IApplicationTransaction transaction)  : ICommandHandler<AddCustomerCommand, Result<Guid>>
{
     public async ValueTask<Result<Guid>> Handle(AddCustomerCommand command, CancellationToken cancellationToken)
     {
          await validator.ValidateAndThrowAsync(command, cancellationToken);
          var customer = await factory.CreateCustomerAsync(command.Name, command.Code, command.Siret, command.Email, command.PhoneNumber, command.Address);
          await transaction.CommitAsync(cancellationToken);
          return Result.Created(customer.Id);
     }
}
