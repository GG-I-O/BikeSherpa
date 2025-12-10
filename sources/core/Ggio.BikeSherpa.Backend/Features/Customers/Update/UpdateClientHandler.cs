using Ardalis.Result;
using FluentValidation;
using Ggio.BikeSherpa.Backend.Domain;
using Ggio.BikeSherpa.Backend.Domain.CustomerAggregate;
using Ggio.BikeSherpa.Backend.Domain.CustomerAggregate.Specification;
using Ggio.DddCore;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Features.Customers.Update;

public record UpdateClientCommand(
     Guid Id,
     string Name,
     string Code,
     string? Siret,
     string Email,
     string PhoneNumber,
     Address Address
) : ICommand<Result<Guid>>;

public class UpdateClientCommandValidator : AbstractValidator<UpdateClientCommand>
{
     public UpdateClientCommandValidator()
     {
          RuleFor(x => x.Id).NotEmpty();
          RuleFor(x => x.Name).NotEmpty();
          RuleFor(x => x.Code).NotEmpty();
          RuleFor(x => x.Email).NotEmpty();
          RuleFor(x => x.PhoneNumber).NotEmpty();
          RuleFor(x => x.Address).NotEmpty();
     }
}

public class UpdateClientHandler(
     IReadRepository<Customer> repository,
     IValidator<UpdateClientCommand> validator,
     IApplicationTransaction transaction
) : ICommandHandler<UpdateClientCommand, Result<Guid>>
{
     public async ValueTask<Result<Guid>> Handle(UpdateClientCommand command, CancellationToken ct)
     {
          await validator.ValidateAndThrowAsync(command, ct);
          var entity = await repository.FirstOrDefaultAsync(new ClientByIdSpecification(command.Id), ct);
          if (entity is null)
               return Result.NotFound();
          entity.Name = command.Name;
          // entity.Code = command.Code;
          entity.Siret = command.Siret;
          entity.Email = command.Email;
          entity.PhoneNumber = command.PhoneNumber;
          entity.Address = command.Address;
          await transaction.CommitAsync(ct);
          return Result.Success(command.Id);
     }
}
