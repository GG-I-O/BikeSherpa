using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.Model;

public class AttachmentRequest
{
     [FromRoute]
     public Guid DeliveryId { get; set; }
     
     [FromRoute]
     public Guid StepId { get; set; }
     
     public required IFormFile File { get; set; }
}
