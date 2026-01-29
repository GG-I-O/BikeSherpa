using Ardalis.Result;
using Facet.Extensions;
using FluentValidation;
using Ggio.BikeSherpa.Backend.Domain;
using Ggio.BikeSherpa.Backend.Domain.CourierAggregate;
using Ggio.BikeSherpa.Backend.Domain.CourierAggregate.Specification;
using Ggio.BikeSherpa.Backend.Model;
using Ggio.DddCore;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Features.Couriers.Add;

public record AddCourierCommand(
     string FirstName,
     string LastName,
     string Code,
     string? Email,
     string PhoneNumber,
     AddressCrud Address
) : ICommand<Result<Guid>>;

public class AddCourierCommandValidator : AbstractValidator<AddCourierCommand>
{
     public AddCourierCommandValidator(IReadRepository<Courier> repository)
     {
          RuleFor(x => x.FirstName).NotEmpty();
          RuleFor(x => x.LastName).NotEmpty();
          RuleFor(x => x.Code).NotEmpty().CustomAsync(async (code, context, cancellationToken) =>
          {
               var codeisValid = !await repository.AnyAsync(new CourierByCodeSpecification(code), cancellationToken);
               if (!codeisValid)
               {
                    context.AddFailure("Code livreur déjà utilisé");
               }
          });
          RuleFor(x => x.Email).NotEmpty().When(x => x.Email != null);
          RuleFor(x => x.PhoneNumber).NotEmpty();
          RuleFor(x => x.Address).NotEmpty();
     }
}

public class AddCourierHandler(
     ICourierFactory factory,
     IValidator<AddCourierCommand> validator,
     IApplicationTransaction transaction) : ICommandHandler<AddCourierCommand, Result<Guid>>
{
     public async ValueTask<Result<Guid>> Handle(AddCourierCommand command, CancellationToken cancellationToken)
     {
          await validator.ValidateAndThrowAsync(command, cancellationToken);
          
          var courier = await factory.CreateCourierAsync(
               command.FirstName,
               command.LastName,
               command.Code,
               command.Email,
               command.PhoneNumber,
               command.Address.ToSource<AddressCrud, Address>()
          );

          await transaction.CommitAsync(cancellationToken);
          return Result.Created(courier.Id);
     }
}