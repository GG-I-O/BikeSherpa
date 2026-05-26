namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Services.Step;

public interface IDeliveryStepAttachmentSaveService
{
     Task<string> StoreFileAsync(
          Stream content,
          string fileName,
          string contentType,
          CancellationToken cancellationToken = default);
}
