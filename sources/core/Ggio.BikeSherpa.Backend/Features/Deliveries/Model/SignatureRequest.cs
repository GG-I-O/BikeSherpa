using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Ggio.BikeSherpa.Backend.Features.Deliveries.Model;

public class SignatureRequest
{
     [FromRoute]
     public Guid DeliveryId { get; set; }
     
     [FromRoute]
     public Guid StepId { get; set; }
     
     public required IFormFile Signature { get; set; }
     
     public required string DomainType { get; set; }
     
     public required string Receiver { get; set; }
}
