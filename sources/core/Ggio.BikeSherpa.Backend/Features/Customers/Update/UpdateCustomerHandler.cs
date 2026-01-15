using Ardalis.Result;
using Facet.Extensions;
using FluentValidation;
using Ggio.BikeSherpa.Backend.Domain;
using Ggio.BikeSherpa.Backend.Domain.CustomerAggregate;
using Ggio.BikeSherpa.Backend.Domain.CustomerAggregate.Specification;
using Ggio.BikeSherpa.Backend.Model;
using Ggio.DddCore;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Features.Customers.Update;

public record UpdateCustomerCommand(
     Guid Id,
     string Name,
     string Code,
     string? Siret,
     string Email,
     string PhoneNumber,
     AddressCrud Address
) : ICommand<Result<Guid>>;

public class UpdateCustomerCommandValidator : AbstractValidator<UpdateCustomerCommand>
{
     public UpdateCustomerCommandValidator(IReadRepository<Customer> repository)
     {
          RuleFor(x => x.Id).NotEmpty();
          RuleFor(x => x.Name).NotEmpty();
          RuleFor(x => x.Code).NotEmpty().CustomAsync(async (code, context, cancellationToken) =>
          {
               var command = (UpdateCustomerCommand)context.InstanceToValidate;
               var existingCustomer = await repository.FirstOrDefaultAsync(new CustomerByCodeSpecification(code), cancellationToken);

               if (existingCustomer != null && existingCustomer.Id != command.Id)
               {
                    context.AddFailure("Code client déjà utilisé");
               }
          });
          RuleFor(x => x.Email).NotEmpty();
          RuleFor(x => x.PhoneNumber).NotEmpty();
          RuleFor(x => x.Address).NotEmpty();
     }
}

public class UpdateCustomerHandler(
     IReadRepository<Customer> repository,
     IValidator<UpdateCustomerCommand> validator,
     IApplicationTransaction transaction
) : ICommandHandler<UpdateCustomerCommand, Result<Guid>>
{
     public async ValueTask<Result<Guid>> Handle(UpdateCustomerCommand command, CancellationToken cancellationToken)
     {
          await validator.ValidateAndThrowAsync(command, cancellationToken);
          var entity = await repository.FirstOrDefaultAsync(new CustomerByIdSpecification(command.Id), cancellationToken);
          if (entity is null)
               return Result.NotFound();

          entity.Name = command.Name;
          entity.Code = command.Code;
          entity.Siret = command.Siret;
          entity.Email = command.Email;
          entity.PhoneNumber = command.PhoneNumber;
          entity.Address = command.Address.ToSource<AddressCrud, Address>();
          await transaction.CommitAsync(cancellationToken);
          return Result.Success(command.Id);
     }
}
