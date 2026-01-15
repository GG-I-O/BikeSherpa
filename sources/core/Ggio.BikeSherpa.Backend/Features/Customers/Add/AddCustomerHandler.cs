using Ardalis.Result;
using Facet.Extensions;
using FluentValidation;
using Ggio.BikeSherpa.Backend.Domain;
using Ggio.BikeSherpa.Backend.Domain.CustomerAggregate;
using Ggio.BikeSherpa.Backend.Domain.CustomerAggregate.Specification;
using Ggio.BikeSherpa.Backend.Model;
using Ggio.DddCore;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Features.Customers.Add;

public record AddCustomerCommand(
     string Name,
     string Code,
     string? Siret,
     string Email,
     string PhoneNumber,
     AddressCrud Address
) : ICommand<Result<Guid>>;

public class AddCustomerCommandValidator : AbstractValidator<AddCustomerCommand>
{
     public AddCustomerCommandValidator(IReadRepository<Customer> repository)
     {
          RuleFor(x => x.Name).NotEmpty();
          RuleFor(x => x.Code).NotEmpty().CustomAsync(async (code, context, cancellationToken) =>
          {
               var codeisValid = !await repository.AnyAsync(new CustomerByCodeSpecification(code), cancellationToken);
               if (!codeisValid)
               {
                    context.AddFailure("Code client déjà utilisé");
               }
          });
          RuleFor(x => x.Email).NotEmpty();
          RuleFor(x => x.PhoneNumber).NotEmpty();
          RuleFor(x => x.Address).NotEmpty();
     }
}

public class AddCustomerHandler(
     ICustomerFactory factory,
     IValidator<AddCustomerCommand> validator,
     IApplicationTransaction transaction) : ICommandHandler<AddCustomerCommand, Result<Guid>>
{
     public async ValueTask<Result<Guid>> Handle(AddCustomerCommand command, CancellationToken cancellationToken)
     {
          await validator.ValidateAndThrowAsync(command, cancellationToken);
          
          var customer = await factory.CreateCustomerAsync(
               command.Name,
               command.Code,
               command.Siret,
               command.Email,
               command.PhoneNumber,
               command.Address.ToSource<AddressCrud, Address>()
          );

          await transaction.CommitAsync(cancellationToken);
          return Result.Created(customer.Id);
     }
}
