using JetBrains.Annotations;

namespace Ggio.BikeSherpa.Backend.Model;

[UsedImplicitly]
public record AddResult<TId>(TId Id);
