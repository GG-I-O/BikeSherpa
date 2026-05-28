namespace Ggio.BikeSherpa.Backend.Infrastructure.Storage;

public interface IDeliveryStepAttachmentSaveService
{
     Task<string> StoreFileAsync(
          Stream content,
          string fileName,
          string contentType,
          CancellationToken cancellationToken = default);
}
