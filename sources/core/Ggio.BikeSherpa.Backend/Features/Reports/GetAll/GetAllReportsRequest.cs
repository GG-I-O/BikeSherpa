using FastEndpoints;
using JetBrains.Annotations;

namespace Ggio.BikeSherpa.Backend.Features.Reports.GetAll;

[UsedImplicitly]
public record GetAllReportsRequest
{
     [QueryParam]
     public Guid CustomerId { get; set; }
     [QueryParam]
     public DateTimeOffset StartDate { get; set; }
     [QueryParam]
     public DateTimeOffset EndDate { get; set; }
}
