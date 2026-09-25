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
                         table.ColumnsDefinition(columns => { columns.RelativeColumn(); });

                         table.Header(header => { header.Cell().Text("Description").Bold(); });

                         table.Cell().Text(delivery.DeliveryLabel).Bold();

                         table.Cell().Column(c =>
                         {
                              foreach (var step in delivery.Steps)
                              {
                                   if (step.Address != null)
                                   {
                                        c.Item().PaddingLeft(10).Text(step.Address.GetFullAddress()).FontSize(8).Italic();
                                   }

                                   if (step.DeliveryDate != null)
                                   {
                                        c.Item().Text($"Heure de passage : {step.DeliveryDate.Value.Hour}:{step.DeliveryDate.Value.Minute}").FontSize(9);
                                   }

                                   if (step.Receiver != null)
                                   {
                                        c.Item().Text($"Réceptionné par {step.Receiver}");
                                   }

                                   foreach (var attachment in step.AttachmentFiles)
                                   {
                                        c.Item().Text($"{attachment.DomainType} : {attachment.Path}");
                                   }
                              }
                         });
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
