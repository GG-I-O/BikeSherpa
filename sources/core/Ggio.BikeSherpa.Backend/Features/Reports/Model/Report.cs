using Ggio.BikeSherpa.Backend.Domain.SharedKernel;

namespace Ggio.BikeSherpa.Backend.Features.Reports.Model;

public record Report
{
     public required string CustomerName { get; set; }
     public required DateTimeOffset StartDate { get; set; }
     public required DateTimeOffset EndDate { get; set; }
     public required double TotalPrice { get; set; }
     public required double TotalPriceWithVat { get; set; }
     public required List<DeliveryReport> Deliveries { get; set; }
}

public record DeliveryReport
{
     public required string DeliveryCode { get; set; }
     public required DateTimeOffset DeliveryDate { get; set; }
     public required double DeliveryPrice { get; set; }
     public required double DeliveryPriceWithVat { get; set; }
     public required List<DeliveryReportDetail> Details { get; set; }
}

public record DeliveryReportDetail
{
     public required string Description { get; set; }
     public Address? Address { get; set; }
     public required double Price { get; set; }
     public required int Quantity { get; set; }

     public required string? CourierName { get; set; }
}
