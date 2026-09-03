using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Model;
using Google.Api.Gax.Grpc;
using Google.Maps.Places.V1;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Features.Addresses.GetSuggestion;

public record GetAddressSuggestionQuery(string Address) : IQuery<List<AddressCrud>>;

public class GetAddressSuggestionHandler(PlacesClient placesClient) : IQueryHandler<GetAddressSuggestionQuery, List<AddressCrud>>
{
     public async ValueTask<List<AddressCrud>> Handle(GetAddressSuggestionQuery query, CancellationToken cancellationToken)
     {
          var callSettings = CallSettings.FromHeader(
               "X-Goog-FieldMask", 
               "places.displayName,places.formattedAddress,places.postalAddress,places.location"
               );
          var response = await placesClient.SearchTextAsync(
               new SearchTextRequest
               {
                    TextQuery = query.Address
               },
               callSettings
          );

          var addresses = response.Places.ToList();
          List<AddressCrud> formattedAddresses =
          [
               .. addresses.Select(suggestion => new AddressCrud()
               {
                    Name = suggestion.DisplayName.Text,
                    StreetInfo = suggestion.PostalAddress.AddressLines[0],
                    City = suggestion.PostalAddress.Locality,
                    Coordinates = new GeoPoint(suggestion.Location.Longitude, suggestion.Location.Latitude),
                    Postcode = suggestion.PostalAddress.PostalCode
               })
          ];

          return formattedAddresses;
     }
}