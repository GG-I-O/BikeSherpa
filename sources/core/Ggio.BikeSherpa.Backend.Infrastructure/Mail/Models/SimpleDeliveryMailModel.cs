namespace Ggio.BikeSherpa.Backend.Infrastructure.Mail.Models;

public record SimpleDeliveryMailModel
{
     public string? DeliveryCode { get; set; }
     public string? PickupAddress { get; set; }
     public string? DestinationAddress { get; set; }
     public string? PickupDate { get; set; }
     public string? TimeSlot { get; set; }
     public string? LoadingSlot { get; set; }
     public string? Options { get; set; }
     public string? Price { get; set; }
     public string? Comments { get; set; }
}
