using FastEndpoints;
using JetBrains.Annotations;

namespace Ggio.BikeSherpa.Backend.Features.Reports.Courier;

[UsedImplicitly]
public record GetReportRequest
{
     [RouteParam]
     public Guid CourierId { get; set; }
     [QueryParam]
     public DateTimeOffset StartDate { get; set; }
     [QueryParam]
     public DateTimeOffset EndDate { get; set; }
}
