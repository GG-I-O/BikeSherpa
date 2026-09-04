using Ggio.BikeSherpa.Backend.Domain.SharedKernel;

namespace Ggio.BikeSherpa.Backend.Infrastructure.SuggestionService;

public interface IAddressSuggestionService
{
     public ValueTask<List<Address>> GetSuggestedAddresses(string query);
}
