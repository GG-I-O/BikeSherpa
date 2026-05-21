using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Features.Reports.Model;

namespace Ggio.BikeSherpa.Backend.Features.Reports.Services;

public interface IReportService
{
     Report GenerateReport(Guid customerId, DateTimeOffset startDate, DateTimeOffset endDate, List<Delivery> deliveries);
}
