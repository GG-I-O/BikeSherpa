using JetBrains.Annotations;

namespace Ggio.BikeSherpa.Backend.Model;


public class Link
{
     [UsedImplicitly]
     public required string Href { get; set; }
     [UsedImplicitly]
     public required string Rel { get; set; }
     [UsedImplicitly]
     public required string Method { get; set; }
}

public interface IHateoasLinks
{
     public List<Link>? Links { get; set; }
}