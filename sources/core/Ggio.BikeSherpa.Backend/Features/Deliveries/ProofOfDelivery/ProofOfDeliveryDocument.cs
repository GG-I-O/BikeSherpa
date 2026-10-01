using Ggio.BikeSherpa.Backend.Domain.SharedKernel;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.ProofOfDelivery;

public class ProofOfDeliveryDocument(Model.ProofOfDelivery delivery, StackHolderInfo stackHolderInfo, Domain.CustomerAggregate.Customer customer) : IDocument
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

                    row.RelativeItem().AlignRight().Column(col => { col.Item().Text("Preuve de livraison").FontSize(16).Bold(); });
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
                              columns.RelativeColumn();
                              columns.ConstantColumn(80);
                         });

                         table.Cell().Text(delivery.DeliveryLabel).Bold();
                         table.Cell().Text("");

                         foreach (var step in delivery.Steps)
                         {
                              table.Cell().Column(c =>
                              {
                                   if (step.Address != null)
                                   {
                                        c.Item().PaddingLeft(10).PaddingTop(10).Text(step.Address.GetFullAddress()).FontSize(10).Italic();
                                   }

                                   if (!string.IsNullOrEmpty(step.Receiver))
                                   {
                                        c.Item().Text($"Réceptionné par {step.Receiver}").FontSize(10);
                                   }

                                   foreach (var attachment in step.AttachmentFiles)
                                   {
                                        c.Item().Text($"{attachment.DomainType} : {attachment.Path}").FontSize(8);
                                   }
                              });

                              if (step.DeliveryDate != null)
                              {
                                   table.Cell().Text($"Heure de passage {step.DeliveryDate.Value.LocalDateTime.Hour:00}:{step.DeliveryDate.Value.LocalDateTime.Minute:00}").FontSize(10);
                              }
                         }
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
