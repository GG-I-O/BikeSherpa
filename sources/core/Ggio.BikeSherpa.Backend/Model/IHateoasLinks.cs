namespace Ggio.BikeSherpa.Backend.Model;

public class Link
{
     public required string Href { get; set; }
     public required string Rel { get; set; }
     public required string Method { get; set; }
}

public interface IHateoasLinks
{
     public List<Link>? Links { get; set; }
}