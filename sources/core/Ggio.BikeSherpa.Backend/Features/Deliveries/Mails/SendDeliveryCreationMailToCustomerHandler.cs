using Ardalis.Result;
using FluentValidation;
using Ggio.BikeSherpa.Backend.Domain.CustomerAggregate;
using Ggio.BikeSherpa.Backend.Domain.CustomerAggregate.Specifications;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Enumerations;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Specification;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Spi;
using Ggio.DddCore;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.Mails;

public record SendDeliveryCreationMailToCustomerCommand(Guid DeliveryId) : ICommand<Result>;

public class SendDeliveryCreationMailToCustomerCommandValidator : AbstractValidator<SendDeliveryCreationMailToCustomerCommand>
{
     public SendDeliveryCreationMailToCustomerCommandValidator()
     {
          RuleFor(x => x.DeliveryId).NotEmpty();
     }
}

public class SendDeliveryCreationMailToCustomerHandler(
     IReadRepository<Delivery> deliveryRepository,
     IReadRepository<Customer> customerRepository,
     IValidator<SendDeliveryCreationMailToCustomerCommand> validator,
     IMailService mailService) : ICommandHandler<SendDeliveryCreationMailToCustomerCommand, Result>
{
     public async ValueTask<Result> Handle(SendDeliveryCreationMailToCustomerCommand command, CancellationToken cancellationToken)
     {
          await validator.ValidateAndThrowAsync(command, cancellationToken);
          var delivery = await deliveryRepository.SingleOrDefaultAsync(new DeliveryByIdSpecification(command.DeliveryId), cancellationToken);
          if (delivery is null)
          {
               return Result.NotFound("Delivery not found");
          }

          var customer = await customerRepository.SingleOrDefaultAsync(new CustomerByIdSpecification(delivery.CustomerId), cancellationToken);
          if (customer is null)
          {
               return Result.NotFound("Customer not found");
          }

          switch (delivery.PricingStrategy)
          {
               case PricingStrategy.SimpleDeliveryStrategy:
                    await mailService.SendSimpleDeliveryMailToCustomer(delivery, customer);
                    return Result.Success();
               case PricingStrategy.TourDeliveryStrategy:
                    await mailService.SendTourDeliveryMailToCustomer(delivery, customer);
                    return Result.Success();
               case PricingStrategy.CustomStrategy:
                    return Result.Error("Custom strategy not implemented");
               default:
                    return Result.Error("Unknown pricing strategy");
          }
     }
}
