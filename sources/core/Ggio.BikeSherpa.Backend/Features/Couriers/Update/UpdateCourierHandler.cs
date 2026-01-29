using Ardalis.Result;
using Facet.Extensions;
using FluentValidation;
using Ggio.BikeSherpa.Backend.Domain;
using Ggio.BikeSherpa.Backend.Domain.CourierAggregate;
using Ggio.BikeSherpa.Backend.Domain.CourierAggregate.Specification;
using Ggio.BikeSherpa.Backend.Model;
using Ggio.DddCore;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Features.Couriers.Update;

public record UpdateCourierCommand(
     Guid Id,
     string FirsName,
     string LastName,
     string Code,
     string? Email,
     string PhoneNumber,
     AddressCrud Address
) : ICommand<Result>;

public class UpdateCourierCommandValidator : AbstractValidator<UpdateCourierCommand>
{
     public UpdateCourierCommandValidator(IReadRepository<Courier> repository)
     {
          RuleFor(x => x.Id).NotEmpty();
          RuleFor(x => x.FirsName).NotEmpty();
          RuleFor(x => x.LastName).NotEmpty();
          RuleFor(x => x.Code).NotEmpty().CustomAsync(async (code, context, cancellationToken) =>
          {
               var command = context.InstanceToValidate;
               var existingCourier = await repository.FirstOrDefaultAsync(new CourierByCodeSpecification(code), cancellationToken);

               if (existingCourier != null && existingCourier.Id != command.Id)
               {
                    context.AddFailure("Code livreur déjà utilisé");
               }
          });
          RuleFor(x => x.Email).NotEmpty().When(x => x.Email != null);
          RuleFor(x => x.PhoneNumber).NotEmpty();
          RuleFor(x => x.Address).NotEmpty();
     }
}

public class UpdateCourierHandler(
     IReadRepository<Courier> repository,
     IValidator<UpdateCourierCommand> validator,
     IApplicationTransaction transaction
) : ICommandHandler<UpdateCourierCommand, Result>
{
     public async ValueTask<Result> Handle(UpdateCourierCommand command, CancellationToken cancellationToken)
     {
          await validator.ValidateAndThrowAsync(command, cancellationToken);
          var entity = await repository.FirstOrDefaultAsync(new CourierByIdSpecification(command.Id), cancellationToken);
          if (entity is null)
               return Result.NotFound();

          entity.FirstName = command.FirsName;
          entity.LastName = command.LastName;
          entity.Code = command.Code;
          entity.Email = command.Email;
          entity.PhoneNumber = command.PhoneNumber;
          entity.Address = command.Address.ToSource<AddressCrud, Address>();
          await transaction.CommitAsync(cancellationToken);
          return Result.Success();
     }
}
