using Ggio.BikeSherpa.Backend.Domain.Spi;

namespace Ggio.BikeSherpa.Backend.Infrastructure.Repositories;

public class Parameter
{
     public required string Key { get; set; }
     public required string Value { get; set; }
}

public class ParameterRepository(BackendDbContext dbContext) : IParameterRepository
{
     public const string VatRateKey = "VAT_RATE";
     public const string LastHourToOrder = "LAST_HOUR_TO_ORDER";
     public const string WorkStartDate = "WORK_START_DATE";
     public const string WorkEndDate = "WORK_END_DATE";
     public const string SimpleDeliveryMailTemplateKey = "SIMPLE_DELIVERY_MAIL_TEMPLATE";
     public const string SimpleDeliveryMailSubjectKey = "SIMPLE_DELIVERY_MAIL_SUBJECT";
     public const string TourDeliveryMailTemplateKey = "TOUR_DELIVERY_MAIL_TEMPLATE";
     public const string TourDeliveryMailSubjectKey = "TOUR_DELIVERY_MAIL_SUBJECT";

     public const string EarlyOrderLimitInHours = "EARLY_ORDER_LIMIT_IN_HOURS";
     public const string LastMinuteOrderLimitInHours = "LAST_MINUTE_ORDER_LIMIT_IN_HOURS";

     public async ValueTask<double> GetVatRateAsync()
     {
          return Convert.ToDouble((await dbContext.Parameters.FindAsync(VatRateKey))!.Value);
     }
     
     public async ValueTask<int> GetLastHourToOrderAsync()
     {
          return Convert.ToInt16((await dbContext.Parameters.FindAsync(LastHourToOrder))!.Value);
     }
     
     public async ValueTask<DateTimeOffset> GetWorkStartDateAsync()
     {
          return Convert.ToDateTime((await dbContext.Parameters.FindAsync(WorkStartDate))!.Value);
     }
     
     public async ValueTask<DateTimeOffset> GetWorkEndDateAsync()
     {
          return Convert.ToDateTime((await dbContext.Parameters.FindAsync(WorkEndDate))!.Value);
     }

     public async ValueTask<string> GetSimpleDeliveryMailTemplateContent() => (await dbContext.Parameters.FindAsync(SimpleDeliveryMailTemplateKey))!.Value;
     public async ValueTask<string> GetSimpleDeliveryMailSubjectAsync() => (await dbContext.Parameters.FindAsync(SimpleDeliveryMailSubjectKey))!.Value;
     public async ValueTask<string> GetTourDeliveryMailTemplateContent() => (await dbContext.Parameters.FindAsync(TourDeliveryMailTemplateKey))!.Value;
     public async ValueTask<string> GetTourDeliveryMailSubjectAsync() => (await dbContext.Parameters.FindAsync(TourDeliveryMailSubjectKey))!.Value;

     public async ValueTask<int> GetEarlyOrderLimitInHoursAsync() =>
          int.Parse((await dbContext.Parameters.FindAsync(EarlyOrderLimitInHours))!.Value);
     public async ValueTask<int> GetLastMinuteOrderLimitInHoursAsync() =>
          int.Parse((await dbContext.Parameters.FindAsync(LastMinuteOrderLimitInHours))!.Value);
}
