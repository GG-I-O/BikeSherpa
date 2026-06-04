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

     public async ValueTask<double> GetVatRateAsync()
     {
          return Convert.ToDouble((await dbContext.Parameters.FindAsync(VatRateKey))!.Value);
     }
}
