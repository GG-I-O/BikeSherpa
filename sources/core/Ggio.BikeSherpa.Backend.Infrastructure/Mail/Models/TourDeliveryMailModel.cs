namespace Ggio.BikeSherpa.Backend.Infrastructure.Mail.Models;

public record TourDeliveryMailModel
{
     public string? DeliveryCode { get; set; }
     public string? PickupAddress { get; set; }
     public required DestinationInfo[] Destinations { get; set; }
     public string? PickupDate { get; set; }
     public string? TimeSlot { get; set; }
     public string? LoadingSlot { get; set; }
     public string? Options { get; set; }
     public string? Comments { get; set; }

     public record DestinationInfo(string Address, string BoxType);
}
