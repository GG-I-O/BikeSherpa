using Facet.Extensions;
using Ggio.BikeSherpa.Backend.Domain.SharedKernel;
using Ggio.BikeSherpa.Backend.Infrastructure.SuggestionService;
using Ggio.BikeSherpa.Backend.Model;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Features.Addresses.GetSuggestion;

public record GetAddressSuggestionQuery(string Address) : IQuery<List<AddressCrud>>;

public class GetAddressSuggestionHandler(IAddressSuggestionService suggestionService) : IQueryHandler<GetAddressSuggestionQuery, List<AddressCrud>>
{
     public async ValueTask<List<AddressCrud>> Handle(
          GetAddressSuggestionQuery query,
          CancellationToken cancellationToken
          )
     {
          var addresses = await suggestionService.GetSuggestedAddresses(query.Address);

          return
          [
               .. addresses
                    .Select(address => address.ToFacet<Address, AddressCrud>())
          ];
     }
}