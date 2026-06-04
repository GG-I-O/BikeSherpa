using Ardalis.Result;
using FastEndpoints;
using Ggio.BikeSherpa.Backend.Domain.CustomerAggregate;
using Ggio.BikeSherpa.Backend.Domain.CustomerAggregate.Specifications;
using Ggio.BikeSherpa.Backend.Features.Customers.Add;
using Ggio.BikeSherpa.Backend.Features.Customers.Check;
using Ggio.BikeSherpa.Backend.Features.Customers.Delete;
using Ggio.BikeSherpa.Backend.Features.Customers.Model;
using Ggio.BikeSherpa.Backend.Features.Deliveries.Add;
using Ggio.BikeSherpa.Backend.Features.Deliveries.Get;
using Ggio.BikeSherpa.Backend.Features.Deliveries.Model;
using Ggio.BikeSherpa.Backend.Model;
using Ggio.DddCore;
using Mediator;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.AddByCustomer;

public record AddDeliveryByCustomerRequest(CustomerCrud Customer, DeliveryCrud Delivery);

public class AddDeliveryByCustomerEndpoint(
     ILogger<AddDeliveryByCustomerEndpoint> logger,
     IMediator mediator,
     IReadRepository<Customer> customerRepository) : Endpoint<AddDeliveryByCustomerRequest>
{
     public override void Configure()
     {
          Post("/deliveries/by_customer");
          AllowAnonymous();
          Description(x => x.WithTags("delivery")
               .WithDescription("Add a delivery by a customer")
          );
     }

     public override async Task HandleAsync(AddDeliveryByCustomerRequest req, CancellationToken ct)
     {
          logger.LogInformation("Adding delivery by customer");

          var customerId = await ManageCreationOrGetCustomer(req, ct);

          var resultCreateDelivery = await CreateDeliveryAsync(req, ct, customerId);
          
          await Send.CreatedAtAsync<GetDeliveryEndpoint>(resultCreateDelivery.Value, new AddResult<Guid>(resultCreateDelivery.Value), cancellation: ct);
     }

     private async Task<Result<Guid>> CreateDeliveryAsync(AddDeliveryByCustomerRequest req, CancellationToken ct, Guid customerId)
     {

          try
          {
               var createDeliveryCommand = new AddDeliveryCommand(
                    req.Delivery.PricingStrategy,
                    customerId,
                    req.Delivery.Urgency,
                    req.Delivery.TotalPrice,
                    req.Delivery.Discount,
                    req.Delivery.ExtraCost,
                    req.Delivery.Details,
                    req.Delivery.PackingSize,
                    req.Delivery.InsulatedBox,
                    req.Delivery.ContractDate,
                    req.Delivery.StartDate,
                    req.Delivery.NeedEstimate
               );

              return await mediator.Send(createDeliveryCommand, ct);

          }
          catch (Exception e)
          {
               await mediator.Send(new DeleteCustomerCommand(customerId), ct);
               logger.LogError(e, "Error adding delivery by customer");
               throw;
          }
     }

     private async Task<Guid> ManageCreationOrGetCustomer(AddDeliveryByCustomerRequest req, CancellationToken ct)
     {

          Guid? customerId = null;
          var checkCustomerResult = await customerRepository.SingleOrDefaultAsync(new CustomerByCodeAndEmailSpecification(req.Customer.Code, req.Customer.Email), ct);

          if (checkCustomerResult is null)
          {
               var result = await mediator.Send(new AddTemporaryCustomerCommand(req.Customer.Name,
                    req.Customer.Siret,
                    req.Customer.VatNumber,
                    req.Customer.Email,
                    req.Customer.PhoneNumber,
                    req.Customer.Address), ct);

               customerId = result.Value;
          }
          else
          {
               customerId = checkCustomerResult.Id;
          }

          return customerId.Value;
     }
}
