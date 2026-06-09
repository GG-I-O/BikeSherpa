using Ardalis.Result;
using FastEndpoints;
using Ggio.BikeSherpa.Backend.Domain.CustomerAggregate;
using Ggio.BikeSherpa.Backend.Domain.CustomerAggregate.Specifications;
using Ggio.BikeSherpa.Backend.Extensions;
using Ggio.BikeSherpa.Backend.Features.Customers.Add;
using Ggio.BikeSherpa.Backend.Features.Customers.Delete;
using Ggio.BikeSherpa.Backend.Features.Customers.Model;
using Ggio.BikeSherpa.Backend.Features.Deliveries.Add;
using Ggio.BikeSherpa.Backend.Features.Deliveries.Get;
using Ggio.BikeSherpa.Backend.Features.Deliveries.Mails;
using Ggio.BikeSherpa.Backend.Features.Deliveries.Model;
using Ggio.BikeSherpa.Backend.Model;
using Ggio.DddCore;
using Mediator;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.AddByCustomer;

public record AddDeliveryByCustomerRequest(CustomerCrud Customer, DeliveryCrud Delivery);

/// <summary>
///      This endpoint is implemented as composition of handlers to be able to mutualize at use-case level functionnalities
///      and so avoid code duplication and
///      refactoring at domain level
/// </summary>
/// <param name="logger">Dedicated logger for this endpoint</param>
/// <param name="serviceProvider"></param>
public class AddDeliveryByCustomerEndpoint(
     ILogger<AddDeliveryByCustomerEndpoint> logger,
     IServiceProvider serviceProvider
) : Endpoint<AddDeliveryByCustomerRequest>
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

          var resultCreateDelivery = await CreateDeliveryAsync(req, customerId, ct);
          if (!resultCreateDelivery.IsSuccess)
          {
               await Send.ErrorsAsync(500, ct);
               return;
          }

          var mailResult = await SendCreationMailToCustomer(resultCreateDelivery, ct);
          if (!mailResult.IsSuccess)
          {
               await Send.ToEndpointResult(mailResult, ct);
               return;
          }

          await Send.CreatedAtAsync<GetDeliveryEndpoint>(resultCreateDelivery.Value, new AddResult<Guid>(resultCreateDelivery.Value), cancellation: ct);
     }

     private async Task<Result> SendCreationMailToCustomer(Result<Guid> resultCreateDelivery, CancellationToken ct)
     {

          var mediator = serviceProvider.GetRequiredService<IMediator>();
          var mailResult = await mediator.Send(new SendDeliveryCreationMailToCustomerCommand(resultCreateDelivery.Value), ct);
          return mailResult;
     }

     private async Task<Result<Guid>> CreateDeliveryAsync(AddDeliveryByCustomerRequest req, Guid customerId, CancellationToken ct)
     {
          var serviceScope = serviceProvider.CreateScope();
          var mediator = serviceScope.ServiceProvider.GetRequiredService<IMediator>();
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

               var deliveryResult = await mediator.Send(createDeliveryCommand, ct);

               foreach (
                    var createStepCommand in req.Delivery.Steps
                         .Select(stepDto => stepDto.Data)
                         .Select(step => new AddDeliveryStepCommand(
                                   req.Delivery.Id,
                                   step.StepType,
                                   step.StepAddress,
                                   step.Comment,
                                   step.NotBilled
                              )
                         )
               )
               {
                    await mediator.Send(createStepCommand, ct);
               }

               return deliveryResult;
          }
          catch (Exception e)
          {
               await mediator.Send(new DeleteCustomerCommand(customerId), ct);
               logger.LogError(e, "Error adding delivery by customer");
               return Result.Error(e.Message);
          }
     }

     private async Task<Guid> ManageCreationOrGetCustomer(AddDeliveryByCustomerRequest req, CancellationToken ct)
     {

          using var serviceScope = serviceProvider.CreateScope();
          var customerRepository = serviceScope.ServiceProvider.GetRequiredService<IReadRepository<Customer>>();
          var mediator = serviceScope.ServiceProvider.GetRequiredService<IMediator>();

          Guid? customerId;
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
