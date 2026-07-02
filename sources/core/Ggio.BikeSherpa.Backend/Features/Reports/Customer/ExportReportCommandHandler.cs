using FluentValidation;
using Ggio.BikeSherpa.Backend.Domain.CustomerAggregate.Specifications;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Specification;
using Ggio.BikeSherpa.Backend.Domain.Spi;
using Ggio.BikeSherpa.Backend.Features.Reports.Services;
using Ggio.DddCore;
using Mediator;
using QuestPDF.Fluent;

namespace Ggio.BikeSherpa.Backend.Features.Reports.Customer;

public record ExportReportResult(byte[] Content, string FileName, string ContentType);

public record ExportReportCommand(
     Guid CustomerId,
     DateTimeOffset From,
     DateTimeOffset To
) : ICommand<ExportReportResult>;

public class ExportReportCommandValidator : AbstractValidator<ExportReportCommand>
{
     public ExportReportCommandValidator(IReadRepository<Domain.CustomerAggregate.Customer> customerRepository)
     {
          RuleFor(x => x.CustomerId)
               .NotEmpty()
               .MustAsync(async (customerId, cancellationToken) =>
                    await customerRepository.FirstOrDefaultAsync(new CustomerByIdSpecification(customerId), cancellationToken) is not null)
               .WithMessage("Customer does not exist");

          RuleFor(x => x.From).NotEmpty();
          RuleFor(x => x.To).NotEmpty();
          RuleFor(x => x.From).LessThanOrEqualTo(x => x.To);
     }
}

public class ExportReportCommandHandler(
     IReadRepository<Delivery> repository,
     IReadRepository<Domain.CustomerAggregate.Customer> customerRepository,
     IValidator<ExportReportCommand> validator,
     IReportService service,
     IParameterRepository parameterRepository
) : ICommandHandler<ExportReportCommand, ExportReportResult>
{
     public async ValueTask<ExportReportResult> Handle(ExportReportCommand command, CancellationToken cancellationToken)
     {
          await validator.ValidateAndThrowAsync(command, cancellationToken);

          var customer = await customerRepository.FirstOrDefaultAsync(new CustomerByIdSpecification(command.CustomerId), cancellationToken);

          var deliveries = await repository
               .ListAsync(
                    new DeliveryByCustomerAndDateRangeSpecification(
                         command.CustomerId,
                         command.From,
                         command.To
                    )
                    , cancellationToken
               );

          var report = await service.GenerateDeliveryReportAsync(
               customer!.Name,
               command.From,
               command.To,
               deliveries
          );

          var stackHolderInfo = await parameterRepository.GetStackHolderInfoAsync();

          var document = new CustomerReportDocument(report, stackHolderInfo, customer);
          var content = document.GeneratePdf();

          return new ExportReportResult(
               content,
               $"Report_{customer.Name}_{command.From:yyyyMMdd}_{command.To:yyyyMMdd}.pdf",
               "application/pdf"
          );
     }
}
