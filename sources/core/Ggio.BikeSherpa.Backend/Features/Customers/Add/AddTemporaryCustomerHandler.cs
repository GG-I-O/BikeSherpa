using Ardalis.Result;
using FluentValidation;
using Ggio.BikeSherpa.Backend.Domain.CustomerAggregate;
using Ggio.BikeSherpa.Backend.Domain.CustomerAggregate.Specifications;
using Ggio.BikeSherpa.Backend.Model;
using Ggio.DddCore;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Features.Customers.Add;

public record AddTemporaryCustomerCommand(
     string Name,
     string? Siret,
     string? VatNumber,
     string Email,
     string PhoneNumber,
     AddressCrud Address
) : ICommand<Result<Guid>>;

public class AddTemporaryCustomerCommandValidator : AbstractValidator<AddTemporaryCustomerCommand>
{
     public AddTemporaryCustomerCommandValidator(IReadRepository<Customer> repository)
     {
          RuleFor(x => x.Name).NotEmpty();

          RuleFor(x => x.Siret).NotEmpty().CustomAsync(async (siret, context, cancellationToken) =>
          {
               var siretIsValid = !await repository.AnyAsync(new CustomerBySiretSpecification(siret!), cancellationToken);
               if (!siretIsValid)
               {
                    context.AddFailure("Siret déjà utilisé pour un autre client");
               }
          }).When(x => x.Siret != null);

          RuleFor(x => x.VatNumber).NotEmpty().CustomAsync(async (vatNumber, context, cancellationToken) =>
          {
               var vatNumberIsValid = !await repository.AnyAsync(new CustomerByVatNumberSpecification(vatNumber!), cancellationToken);
               if (!vatNumberIsValid)
               {
                    context.AddFailure("Numéro de TVA déjà utilisé pour un autre client");
               }
          }).When(x => x.VatNumber != null);

          RuleFor(x => x.Email).NotEmpty();
          RuleFor(x => x.PhoneNumber).NotEmpty();
          RuleFor(x => x.Address).NotEmpty();
     }
}

public class AddTemporaryCustomerHandler(ICustomerFactory customerFactory, 
     IValidator<AddTemporaryCustomerCommand> validator,
     IApplicationTransaction applicationTransaction) : ICommandHandler<AddTemporaryCustomerCommand, Result<Guid>>
{
     public async ValueTask<Result<Guid>> Handle(AddTemporaryCustomerCommand command, CancellationToken cancellationToken)
     {
          await validator.ValidateAndThrowAsync(command, cancellationToken);
          var customer = await customerFactory.CreateTemporaryCustomerAsync(new TemporaryCustomerFactoryParameters
               (command.Name, command.Siret, command.VatNumber, command.Email, command.PhoneNumber, command.Address.ToSource()));
          await applicationTransaction.CommitAsync(cancellationToken);
          return Result<Guid>.Success(customer.Id);
     }
}
