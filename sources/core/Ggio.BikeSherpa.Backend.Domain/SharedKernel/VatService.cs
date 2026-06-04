using Ggio.BikeSherpa.Backend.Domain.Spi;

namespace Ggio.BikeSherpa.Backend.Domain.SharedKernel;

public interface IVatService
{
     ValueTask<double> GetPriceWithVatAsync(double price);
}

public class VatService(IParameterRepository parameterRepository) : IVatService
{
     private readonly Lazy<ValueTask<double>> _vat = new(() => parameterRepository.GetVatRateAsync());

     public async ValueTask<double> GetPriceWithVatAsync(double price)
     {
          var vatRate = (await _vat.Value) / 100;
          return price * (1 + vatRate);
     }
}
