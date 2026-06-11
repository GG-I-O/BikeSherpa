namespace Ggio.BikeSherpa.Backend.Domain.Spi;

public interface IParameterRepository
{
     /// <summary>
     /// Get the VAT rate as a percentage
     /// </summary>
     /// <returns>a value between 0 and 100</returns>
     ValueTask<double> GetVatRateAsync();
     
     /// <summary>
     /// Get the last hour to order
     /// </summary>
     /// <returns>a value between 0 and 24</returns>
     ValueTask<int> GetLastHourToOrderAsync();

     /// <summary>
     /// Get the start of the working day
     /// </summary>
     /// <returns>DateTime with working time</returns>
     ValueTask<DateTimeOffset> GetWorkStartDateAsync();

     /// <summary>
     /// Get the end of the working day
     /// </summary>
     /// <returns>DateTime with working time</returns>
     ValueTask<DateTimeOffset> GetWorkEndDateAsync();

     ValueTask<string> GetSimpleDeliveryMailTemplateContent();
     ValueTask<string> GetSimpleDeliveryMailSubjectAsync();
     ValueTask<string> GetTourDeliveryMailTemplateContent();
     ValueTask<string> GetTourDeliveryMailSubjectAsync();
}
