using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.SharedKernel;
using Google.Api.Gax.Grpc;
using Google.Maps.Places.V1;

namespace Ggio.BikeSherpa.Backend.Infrastructure.SuggestionService;

public class AddressSuggestionService(PlacesClient placesClient) : IAddressSuggestionService
{
     public async ValueTask<List<Address>> GetSuggestedAddresses(string query)
     {
          var callSettings = CallSettings.FromHeader(
               "X-Goog-FieldMask", 
               "places.displayName,places.formattedAddress,places.postalAddress,places.location"
          );
          var response = await placesClient.SearchTextAsync(
               new SearchTextRequest
               {
                    TextQuery = query,
                    LanguageCode = "fr",
                    MaxResultCount = 5
               },
               callSettings
          );
          
          var addresses = response.Places.ToList();
          List<Address> formattedAddresses =
          [
               .. addresses
                    .Where(suggestion => suggestion.PostalAddress is not null
                                         && suggestion.PostalAddress.AddressLines.Count > 0)
                    .Select(suggestion =>
                    {
                         var streetInfo = suggestion.PostalAddress.AddressLines[0];
                         var displayName = suggestion.DisplayName?.Text ?? "";

                         return new Address
                         {
                              Name = displayName != streetInfo ? displayName : "",
                              StreetInfo = streetInfo,
                              City = suggestion.PostalAddress.Locality,
                              Coordinates = new GeoPoint(suggestion.Location.Longitude, suggestion.Location.Latitude),
                              Postcode = suggestion.PostalAddress.PostalCode
                         };
                    })
          ];

          return formattedAddresses;
     }
}

