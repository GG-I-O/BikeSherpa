namespace Ggio.BikeSherpa.Backend.Domain.Spi;

public interface IParameterRepository
{
     /// <summary>
     ///      Get the VAT rate as a percentage
     /// </summary>
     /// <returns>a value between 0 and 100</returns>
     ValueTask<double> GetVatRateAsync();

     ValueTask<string> GetSimpleDeliveryMailTemplateContent();
     ValueTask<string> GetSimpleDeliveryMailSubjectAsync();
     ValueTask<string> GetTourDeliveryMailTemplateContent();
     ValueTask<string> GetTourDeliveryMailSubjectAsync();
}
