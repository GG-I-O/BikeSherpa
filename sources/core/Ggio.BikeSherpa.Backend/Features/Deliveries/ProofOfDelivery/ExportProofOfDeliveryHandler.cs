using FluentValidation;
using Ggio.BikeSherpa.Backend.Domain.CustomerAggregate.Specifications;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Specification;
using Ggio.BikeSherpa.Backend.Domain.Spi;
using Ggio.BikeSherpa.Backend.Features.Deliveries.Model;
using Ggio.DddCore;
using Mediator;
using QuestPDF.Fluent;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.ProofOfDelivery;

public record ExportProofOfDeliveryResult(
     byte[] Content,
     string FileName,
     string ContentType
);

public record ExportProofOfDeliveryCommand(
     Guid DeliveryId
) : ICommand<ExportProofOfDeliveryResult>;

public class ExportProofOfDeliveryValidator : AbstractValidator<ExportProofOfDeliveryCommand>
{
     public ExportProofOfDeliveryValidator(
          IReadRepository<Delivery> deliveryRepository
     )
     {
          RuleFor(x => x.DeliveryId)
               .NotEmpty()
               .MustAsync(async (deliveryId, cancellationToken) =>
                    await deliveryRepository.FirstOrDefaultAsync(new DeliveryByIdSpecification(deliveryId), cancellationToken) is not null)
               .WithMessage("Delivery does not exist");
     }
}

public class ExportProofOfDeliveryHandler(
     IReadRepository<Delivery> deliveryRepository,
     IReadRepository<Domain.CustomerAggregate.Customer> customerRepository,
     IValidator<ExportProofOfDeliveryCommand> validator,
     IParameterRepository parameterRepository
) : ICommandHandler<ExportProofOfDeliveryCommand, ExportProofOfDeliveryResult>
{
     public async ValueTask<ExportProofOfDeliveryResult> Handle(ExportProofOfDeliveryCommand command, CancellationToken cancellationToken)
     {
          await validator.ValidateAndThrowAsync(command, cancellationToken);

          var delivery = await deliveryRepository.FirstOrDefaultAsync(new DeliveryByIdSpecification(command.DeliveryId), cancellationToken);

          var customer = await customerRepository.FirstOrDefaultAsync(new CustomerByIdSpecification(delivery!.CustomerId), cancellationToken);
          
          var proofOfDelivery = new Model.ProofOfDelivery()
          {
               DeliveryLabel = delivery!.DeliveryLabel,
               Steps =
               [
                    .. delivery.Steps.Select(step => new ProofOfDeliveryStep()
                    {
                         Address = step.StepAddress,
                         DeliveryDate = step.RealDeliveryDate,
                         Receiver = step.Receiver,
                         AttachmentFiles = step.AttachmentFiles ?? []
                    })
               ]
          };

          var stackHolderInfo = await parameterRepository.GetStackHolderInfoAsync();

          var document = new ProofOfDeliveryDocument(proofOfDelivery, stackHolderInfo, customer!);
          var content = document.GeneratePdf();

          return new ExportProofOfDeliveryResult(
               content,
               $"ProofOfDelivery_{delivery.Code}.pdf",
               "application/pdf"
          );
     } 
}
