namespace Ggio.BikeSherpa.Backend.Infrastructure.Services;

public class DeliveryStepAttachmentSaveService: IDeliveryStepAttachmentSaveService
{
     public Task<string> StoreFileAsync(Stream content, string fileName, string contentType, CancellationToken cancellationToken = default)
     {
          // TO DO: File save service
          return Task.FromResult(fileName);
     }
}
