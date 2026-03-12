namespace Ggio.BikeSherpa.Backend.Domain;

public sealed class ItineraryServiceException(string message, Exception? exception) : Exception(message, exception);

