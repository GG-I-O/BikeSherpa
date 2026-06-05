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
     public const string SimpleDeliveryMailTemplateKey = "SIMPLE_DELIVERY_MAIL_TEMPLATE";
     public const string SimpleDeliveryMailSubjectKey = "SIMPLE_DELIVERY_MAIL_SUBJECT";
     public const string TourDeliveryMailTemplateKey = "TOUR_DELIVERY_MAIL_TEMPLATE";
     public const string TourDeliveryMailSubjectKey = "TOUR_DELIVERY_MAIL_SUBJECT";

     public async ValueTask<double> GetVatRateAsync() => Convert.ToDouble((await dbContext.Parameters.FindAsync(VatRateKey))!.Value);

     public async ValueTask<string> GetSimpleDeliveryMailTemplateContent() => (await dbContext.Parameters.FindAsync(SimpleDeliveryMailTemplateKey))!.Value;
     public async ValueTask<string> GetSimpleDeliveryMailSubjectAsync() => (await dbContext.Parameters.FindAsync(SimpleDeliveryMailSubjectKey))!.Value;
     public async ValueTask<string> GetTourDeliveryMailTemplateContent() => (await dbContext.Parameters.FindAsync(TourDeliveryMailTemplateKey))!.Value;
     public async ValueTask<string> GetTourDeliveryMailSubjectAsync() => (await dbContext.Parameters.FindAsync(TourDeliveryMailSubjectKey))!.Value;
}
