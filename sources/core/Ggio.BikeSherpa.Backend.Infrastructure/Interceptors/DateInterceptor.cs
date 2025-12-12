using Ggio.BikeSherpa.Backend.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;

namespace Ggio.BikeSherpa.Backend.Infrastructure.Interceptors;

public class DateInterceptor: SaveChangesInterceptor
{
     public override InterceptionResult<int> SavingChanges(
          DbContextEventData eventData,
          InterceptionResult<int> result)
     {
          UpdateTimestamps(eventData.Context);
          return base.SavingChanges(eventData, result);
     }

     public override ValueTask<InterceptionResult<int>> SavingChangesAsync(
          DbContextEventData eventData,
          InterceptionResult<int> result,
          CancellationToken cancellationToken = default)
     {
          UpdateTimestamps(eventData.Context);
          return base.SavingChangesAsync(eventData, result, cancellationToken);
     }

     private void UpdateTimestamps(DbContext? context)
     {
          if (context == null) return;

          var entries = context.ChangeTracker.Entries<IAuditEntity>()
               .Where(e => e.State is EntityState.Added or EntityState.Modified);

          var now = DateTime.UtcNow;

          foreach (var entry in entries)
          {
               if (entry.State == EntityState.Added)
               {
                    entry.Entity.CreatedAt = now;
               }
               entry.Entity.UpdatedAt = now;
          }
     }
}
