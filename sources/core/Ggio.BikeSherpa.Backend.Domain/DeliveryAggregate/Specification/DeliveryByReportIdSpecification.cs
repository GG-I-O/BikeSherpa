using Ardalis.Specification;

namespace Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate.Specification;

public class DeliveryByReportIdSpecification : SingleResultSpecification<Delivery>
{
     public DeliveryByReportIdSpecification(string reportId)
     {
          Query.Where(x => x.ReportId == reportId);
     }
}
