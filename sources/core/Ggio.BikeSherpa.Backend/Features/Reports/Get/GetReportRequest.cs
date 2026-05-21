using FastEndpoints;
using JetBrains.Annotations;

namespace Ggio.BikeSherpa.Backend.Features.Reports.Get;

[UsedImplicitly]
public record GetReportRequest
{
     [QueryParam]
     public Guid CustomerId { get; set; }
     [QueryParam]
     public DateTimeOffset StartDate { get; set; }
     [QueryParam]
     public DateTimeOffset EndDate { get; set; }
}
