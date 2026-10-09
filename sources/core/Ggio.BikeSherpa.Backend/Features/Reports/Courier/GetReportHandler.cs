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
     DateTimeOffset From,
     DateTimeOffset To
) : IQuery<ReportFile>;

public class GetReportQueryValidator : AbstractValidator<GetReportQuery>
{
     public GetReportQueryValidator()
     {
          RuleFor(x => x.From).NotEmpty();
          RuleFor(x => x.To).NotEmpty();
          RuleFor(x => x.From).LessThanOrEqualTo(x => x.To);
     }
}

public class GetReportHandler(
     IReadRepository<Delivery> repository,
     IValidator<GetReportQuery> validator,
     IReportService service
) : IQueryHandler<GetReportQuery, ReportFile>
{
     public async ValueTask<ReportFile> Handle(GetReportQuery query, CancellationToken cancellationToken)
     {
          await validator.ValidateAndThrowAsync(query, cancellationToken);

          var deliveries = await repository
               .ListAsync(
                    new DeliveryByDateRangeSpecification(
                         query.From,
                         query.To
                    )
                    , cancellationToken
               );

          var report = await service.GenerateDeliveryReportAsync(
               string.Empty,
               query.From,
               query.To,
               deliveries
          );

          using var workbook = new XLWorkbook();
          var worksheet = workbook.Worksheets.Add("Rapport coursier");

          var currentRow = 1;
          worksheet.Cell(currentRow, 1).Value = $"Rapport pour les coursiers du {query.From} au {query.To}";
          worksheet.Cell(currentRow, 1).Value = "Nom du coursier";
          worksheet.Cell(currentRow, 2).Value = "Livraison";
          worksheet.Cell(currentRow, 3).Value = "Description";
          worksheet.Cell(currentRow, 4).Value = "Prix";
          worksheet.Cell(currentRow, 5).Value = "Quantité";
          worksheet.Cell(currentRow, 6).Value = "Adresse";

          foreach (var delivery in report.Deliveries)
          {
               foreach (var detail in delivery.Details)
               {
                    currentRow++;
                    worksheet.Cell(currentRow, 1).Value = detail.CourierName;
                    worksheet.Cell(currentRow, 2).Value = delivery.DeliveryLabel;
                    worksheet.Cell(currentRow, 3).Value = detail.Description;
                    worksheet.Cell(currentRow, 4).Value = detail.Price;
                    worksheet.Cell(currentRow, 5).Value = detail.Quantity;
                    worksheet.Cell(currentRow, 6).Value = detail.Address?.ToString() ?? string.Empty;
               }
          }

          worksheet.Columns().AdjustToContents();

          using var stream = new MemoryStream();
          workbook.SaveAs(stream);
          var content = stream.ToArray();

          return new ReportFile(
               content,
               $"Report_Couriers_{query.From:yyyyMMdd}_{query.To:yyyyMMdd}.xlsx",
               "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet"
          );
     }
}
