using Ardalis.Result;
using FluentValidation;
using Ggio.BikeSherpa.Backend.Domain.ClientAggregate;
using Ggio.DddCore;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Features.Clients.Add;

public record AddClientCommand(
     string Name,
     string Code,
     string? Siret,
     string Email,
     string PhoneNumber,
     string Address
) : ICommand<Result<Guid>>;

public class AddClientCommandValidator : AbstractValidator<AddClientCommand>
{
     public AddClientCommandValidator()
     {
          RuleFor(x => x.Name).NotEmpty();
          RuleFor(x => x.Code).NotEmpty();
          RuleFor(x => x.Email).NotEmpty();
          RuleFor(x => x.PhoneNumber).NotEmpty();
          RuleFor(x => x.Address).NotEmpty();
     }
}

public class AddClientHandler(
     IClientFactory factory,
     IValidator<AddClientCommand> validator,
     IApplicationTransaction transatcion)  : ICommandHandler<AddClientCommand, Result<Guid>>
{
     public async ValueTask<Result<Guid>> Handle(AddClientCommand command, CancellationToken cancellationToken)
     {
          await validator.ValidateAndThrowAsync(command, cancellationToken);
          var client = await factory.CreateClientAsync(command.Name, command.Code, command.Siret, command.Email, command.PhoneNumber, command.Address);
          await transatcion.CommitAsync(cancellationToken);
          return Result.Created(client.Id);
     }
}
