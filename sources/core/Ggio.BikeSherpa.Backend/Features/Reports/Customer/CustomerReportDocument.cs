using Ggio.BikeSherpa.Backend.Domain.SharedKernel;
using Ggio.BikeSherpa.Backend.Features.Reports.Model;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Ggio.BikeSherpa.Backend.Features.Reports.Customer;

public class CustomerReportDocument(Report report, StackHolderInfo stackHolderInfo, Domain.CustomerAggregate.Customer customer) : IDocument
{
     public void Compose(IDocumentContainer container)
     {
          container.Page(page =>
          {
               page.Margin(1, Unit.Centimetre);
               page.Header().Row(row =>
               {
                    row.RelativeItem().Column(col =>
                    {
                         col.Item().Text(stackHolderInfo.CompanyName).FontSize(20).Bold().FontColor(Colors.Blue.Medium);
                         col.Item().Text(stackHolderInfo.Adresse);
                         col.Item().Text(stackHolderInfo.Phone);
                         col.Item().Text(stackHolderInfo.Email);
                    });

                    row.RelativeItem().AlignRight().Column(col =>
                    {
                         col.Item().Text("RAPPORT DE LIVRAISONS").FontSize(16).Bold();
                         col.Item().Text($"Période du {report.StartDate:dd/MM/yyyy} au {report.EndDate:dd/MM/yyyy}");
                    });
               });

               page.Content().PaddingVertical(1, Unit.Centimetre).Column(col =>
               {
                    col.Item().BorderBottom(1).PaddingBottom(5).Row(row =>
                    {
                         row.RelativeItem().AlignRight().Column(c =>
                         {
                              c.Item().Text(customer.Name);
                              c.Item().Text(customer.Address.GetFullAddress());
                              c.Item().Text(customer.PhoneNumber);
                         });
                    });

                    col.Item().PaddingTop(1, Unit.Centimetre).Table(table =>
                    {
                         table.ColumnsDefinition(columns =>
                         {
                              columns.ConstantColumn(80);
                              columns.RelativeColumn();
                              columns.ConstantColumn(80);
                              columns.ConstantColumn(80);
                         });

                         table.Header(header =>
                         {
                              header.Cell().Text("Date").Bold();
                              header.Cell().Text("Détails").Bold();
                              header.Cell().AlignRight().Text("Prix HT").Bold();
                              header.Cell().AlignRight().Text("Prix TTC").Bold();
                         });

                         foreach (var delivery in report.Deliveries)
                         {
                              table.Cell().Text(delivery.DeliveryDate.ToString("dd/MM/yyyy"));
                              table.Cell().Column(c =>
                              {
                                   c.Item().Text($"Livraison n° : {delivery.DeliveryCode}").Bold();
                                   foreach (var detail in delivery.Details)
                                   {
                                        c.Item().Text($"{detail.Description}").FontSize(9);
                                        if (detail.Address != null)
                                        {
                                             c.Item().PaddingLeft(10).Text(detail.Address.GetFullAddress()).FontSize(8).Italic();
                                        }
                                        c.Item().Text($"{detail.Price:N2} € HT").FontSize(9);
                                   }
                              });

                              table.Cell().AlignRight().Text($"{delivery.DeliveryPrice:N2} €");
                              table.Cell().AlignRight().Text($"{delivery.DeliveryPriceWithVat:N2} €");
                         }
                    });

                    col.Item().AlignRight().PaddingTop(1, Unit.Centimetre).Column(c =>
                    {
                         c.Item().Text($"Total HT : {report.TotalPrice:N2} €").Bold();
                         c.Item().Text($"Total TTC : {report.TotalPriceWithVat:N2} €").FontSize(14).Bold().FontColor(Colors.Blue.Medium);
                    });
               });

               page.Footer().AlignCenter().Text(x =>
               {
                    x.Span("Page ");
                    x.CurrentPageNumber();
               });
          });
     }
}
