namespace Ggio.DddCore;

public interface IApplicationTransaction
{
  Task CommitAsync(CancellationToken cancellationToken = default);
}
