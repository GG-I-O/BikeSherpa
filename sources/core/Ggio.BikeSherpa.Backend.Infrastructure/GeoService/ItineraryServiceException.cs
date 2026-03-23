namespace Ggio.BikeSherpa.Backend.Infrastructure.GeoService;

public sealed class ItineraryServiceException(string message, Exception? innerException = null) : Exception(message, innerException)
{
     public static ItineraryServiceException NoIdenticalCoordinates()
     {
          return new ItineraryServiceException("Coordonnées de départ et d’arrivée identiques");
     }

     public static ItineraryServiceException NoNullResult()
     {
          return new ItineraryServiceException("Aucune réponse de l'API");
     }
}
