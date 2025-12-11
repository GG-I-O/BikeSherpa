namespace Ggio.BikeSherpa.Backend.Features.Hateoas;

public class Link
{
     public required string Href { get; set; }
     public required string Rel { get; set; }
     public required string Method { get; set; }
}
