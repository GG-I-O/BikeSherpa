using FastEndpoints;
using JetBrains.Annotations;

namespace Ggio.BikeSherpa.Backend.Features.Reports.Customer;

[UsedImplicitly]
public record GetReportRequest
{
     [RouteParam]
     public Guid CustomerId { get; set; }
     [QueryParam]
     public DateTimeOffset StartDate { get; set; }
     [QueryParam]
     public DateTimeOffset EndDate { get; set; }
}
