using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Features.Reports.Model;

namespace Ggio.BikeSherpa.Backend.Features.Reports.Services;

public interface IReportService
{
     Task<Report> GenerateReport(string customerName, DateTimeOffset startDate, DateTimeOffset endDate, List<Delivery> deliveries);
}
