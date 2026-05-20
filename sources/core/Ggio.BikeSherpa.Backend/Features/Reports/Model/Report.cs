namespace Ggio.BikeSherpa.Backend.Features.Reports.Model;

public record Report
{
     public required string DeliveryCode { get; set; }
     public required DateTimeOffset DeliveryDate { get; set; }
     public required double DeliveryPrice { get; set; }
     
     public required List<ReportDetail> Details { get; set; }
}

public record ReportDetail
{
     public required string Description { get; set; }
     public required double Price { get; set; }
     public required int Quantity { get; set; }
}
