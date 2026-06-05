using Ggio.BikeSherpa.Backend.Domain.CustomerAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Spi;
using Ggio.BikeSherpa.Backend.Domain.Spi;
using Ggio.BikeSherpa.Backend.Infrastructure.Mail.Models;
using Scriban;

namespace Ggio.BikeSherpa.Backend.Infrastructure.Mail;

public class MailService(IParameterRepository parameterService, IMailSender mailSender) : IMailService
{
     public async Task SendSimpleDeliveryMailToCustomer(Delivery delivery, Customer customer)
     {
          var model = MapToSimpleModel(delivery);
          var templateContent = await GetSimpleDeliveryMailTemplateContent();
          var template = Template.Parse(templateContent);
          var result = await template.RenderAsync(model, member => member.Name.ToLower());
          var mailSubject = await parameterService.GetSimpleDeliveryMailSubjectAsync();
          await mailSender.SendEmailAsync(customer.Email, mailSubject, result);
     }

     public async Task SendTourDeliveryMailToCustomer(Delivery delivery, Customer customer)
     {
          var model = MapToTourModel(delivery);
          var templateContent = await GetTourDeliveryMailTemplateContent();
          var template = Template.Parse(templateContent);
          var result = await template.RenderAsync(model, member => member.Name.ToLower());
          var mailSubject = await parameterService.GetTourDeliveryMailSubjectAsync();
          await mailSender.SendEmailAsync(customer.Email, mailSubject, result);
     }

     private SimpleDeliveryMailModel MapToSimpleModel(Delivery delivery)
     {
          var pickupStep = delivery.Steps.OrderBy(s => s.Order).FirstOrDefault(s => s.StepType == StepType.Pickup);
          var dropoffStep = delivery.Steps.OrderByDescending(s => s.Order).FirstOrDefault(s => s.StepType == StepType.Dropoff);

          return new SimpleDeliveryMailModel
          {
               DeliveryCode = delivery.Code,
               PickupAddress = pickupStep != null ? $"{pickupStep.StepAddress.StreetInfo}, {pickupStep.StepAddress.Postcode} {pickupStep.StepAddress.City}" : string.Empty,
               DestinationAddress = dropoffStep != null ? $"{dropoffStep.StepAddress.StreetInfo}, {dropoffStep.StepAddress.Postcode} {dropoffStep.StepAddress.City}" : string.Empty,
               PickupDate = delivery.StartDate.ToString("dd/MM/yyyy"),
               TimeSlot = delivery.StartDate.ToString("HH:mm"),
               LoadingSlot = delivery.Urgency.Label,
               Options = $"{delivery.PackingSize.Label}{(delivery.InsulatedBox ? " - Box Isotherme" : "")}",
               Price = delivery.TotalPrice?.ToString("C2") ?? "Sur devis",
               Comments = string.Join(", ", delivery.Details)
          };
     }

     private async Task<string> GetSimpleDeliveryMailTemplateContent() => await parameterService.GetSimpleDeliveryMailTemplateContent();

     private async Task<string> GetTourDeliveryMailTemplateContent() => await parameterService.GetTourDeliveryMailTemplateContent();

     private TourDeliveryMailModel MapToTourModel(Delivery delivery)
     {
          var pickupStep = delivery.Steps.OrderBy(s => s.Order).FirstOrDefault(s => s.StepType == StepType.Pickup);
          var dropoffSteps = delivery.Steps.OrderBy(s => s.Order).Where(s => s.StepType == StepType.Dropoff).ToList();

          return new TourDeliveryMailModel
          {
               DeliveryCode = delivery.Code,
               PickupAddress = pickupStep != null ? $"{pickupStep.StepAddress.StreetInfo}, {pickupStep.StepAddress.Postcode} {pickupStep.StepAddress.City}" : string.Empty,
               Destinations = dropoffSteps.Select(s => new TourDeliveryMailModel.DestinationInfo(
                    $"{s.StepAddress.StreetInfo}, {s.StepAddress.Postcode} {s.StepAddress.City}",
                    $"{delivery.PackingSize.Label}{(delivery.InsulatedBox ? " - Box Isotherme" : "")}"
               )).ToArray(),
               PickupDate = delivery.StartDate.ToString("dd/MM/yyyy"),
               TimeSlot = delivery.StartDate.ToString("HH:mm"),
               LoadingSlot = delivery.Urgency.Label,
               Options = $"{delivery.PackingSize.Label}{(delivery.InsulatedBox ? " - Box Isotherme" : "")}",
               Comments = string.Join(", ", delivery.Details)
          };
     }
}
