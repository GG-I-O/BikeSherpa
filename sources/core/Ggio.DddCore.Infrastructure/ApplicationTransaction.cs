using Microsoft.EntityFrameworkCore;

namespace Ggio.DddCore.Infrastructure;

public class ApplicationTransaction<TDbContext>(TDbContext dbContext) : IApplicationTransaction where TDbContext : DbContext
{
  public async Task CommitAsync(CancellationToken cancellationToken = default)
  {
    await dbContext.SaveChangesAsync(cancellationToken);
  }
}
