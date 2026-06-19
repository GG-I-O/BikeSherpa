using ClosedXML.Excel;
using FluentValidation;
using Ggio.BikeSherpa.Backend.Domain.CourierAggregate.Specification;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Specification;
using Ggio.BikeSherpa.Backend.Features.Reports.Services;
using Ggio.DddCore;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Features.Reports.Courier;

public record ReportFile(byte[] Content, string FileName, string ContentType);

public record GetReportQuery(
     Guid CourierId,
     DateTimeOffset From,
     DateTimeOffset To
) : IQuery<ReportFile>;

public class GetReportQueryValidator : AbstractValidator<GetReportQuery>
{
     public GetReportQueryValidator(IReadRepository<Domain.CourierAggregate.Courier> customerRepository)
     {
          RuleFor(x => x.CourierId)
               .NotEmpty()
               .MustAsync(async (customerId, cancellationToken) =>
                    await customerRepository.FirstOrDefaultAsync(new CourierByIdSpecification(customerId), cancellationToken) is not null)
               .WithMessage("Courier does not exist");

          RuleFor(x => x.From).NotEmpty();
          RuleFor(x => x.To).NotEmpty();
          RuleFor(x => x.From).LessThanOrEqualTo(x => x.To);
     }
}

public class GetReportHandler(
     IReadRepository<Delivery> repository,
     IReadRepository<Domain.CourierAggregate.Courier> customerRepository,
     IValidator<GetReportQuery> validator,
     IReportService service
) : IQueryHandler<GetReportQuery, ReportFile>
{
     public async ValueTask<ReportFile> Handle(GetReportQuery query, CancellationToken cancellationToken)
     {
          await validator.ValidateAndThrowAsync(query, cancellationToken);

          var courier = await customerRepository.FirstOrDefaultAsync(new CourierByIdSpecification(query.CourierId), cancellationToken);

          var deliveries = await repository
               .ListAsync(
                    new DeliveryByCourierAndDateRangeSpecification(
                         query.CourierId,
                         query.From,
                         query.To
                    )
                    , cancellationToken
               );

          var report = await service.GenerateDeliveryReportAsync(
               courier!.GetFullName(),
               query.From,
               query.To,
               deliveries
          );

          using var workbook = new XLWorkbook();
          var worksheet = workbook.Worksheets.Add("Report");

          var currentRow = 1;
          worksheet.Cell(currentRow, 1).Value = "Courier Name";
          worksheet.Cell(currentRow, 2).Value = "Delivery Code";
          worksheet.Cell(currentRow, 3).Value = "Delivery Date";
          worksheet.Cell(currentRow, 4).Value = "Description";
          worksheet.Cell(currentRow, 5).Value = "Price";
          worksheet.Cell(currentRow, 6).Value = "Quantity";
          worksheet.Cell(currentRow, 7).Value = "Address";

          foreach (var delivery in report.Deliveries)
          {
               foreach (var detail in delivery.Details)
               {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = detail.CourierName;
                    worksheet.Cell(currentRow, 2).Value = delivery.DeliveryCode;
                    worksheet.Cell(currentRow, 3).Value = delivery.DeliveryDate.DateTime;
                    worksheet.Cell(currentRow, 4).Value = detail.Description;
                    worksheet.Cell(currentRow, 5).Value = detail.Price;
                    worksheet.Cell(currentRow, 6).Value = detail.Quantity;
                    worksheet.Cell(currentRow, 7).Value = detail.Address?.ToString() ?? string.Empty;
               }
          }

          worksheet.Columns().AdjustToContents();

          using var stream = new MemoryStream();
          workbook.SaveAs(stream);
          var content = stream.ToArray();

          return new ReportFile(
               content,
               $"Report_{courier.LastName}_{query.From:yyyyMMdd}_{query.To:yyyyMMdd}.xlsx",
               "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
          );
     }
}
