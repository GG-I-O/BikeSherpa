using Ggio.BikeSherpa.Backend.Domain.Spi;

namespace Ggio.BikeSherpa.Backend.Domain.SharedKernel;

public interface IVatService
{
     ValueTask<double> GetPriceWithVatAsync(double price);
     
     ValueTask<int> GetLastHourToOrderAsync();
}

public class VatService(IParameterRepository parameterRepository) : IVatService
{
     private readonly Lazy<ValueTask<double>> _vat = new(() => parameterRepository.GetVatRateAsync());
     
     private readonly Lazy<ValueTask<int>> _maxHourToOrder = new(() => parameterRepository.GetLastHourToOrderAsync());
 
     public async ValueTask<double> GetPriceWithVatAsync(double price)
     {
          var vatRate = (await _vat.Value) / 100;
          return price * (1 + vatRate);
     }
     
     public async ValueTask<int> GetLastHourToOrderAsync()
     {
          return await _maxHourToOrder.Value;
     }
}
