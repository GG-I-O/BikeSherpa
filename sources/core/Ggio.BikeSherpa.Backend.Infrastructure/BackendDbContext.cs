using Ggio.BikeSherpa.Backend.Domain.CourierAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.CustomerAggregate;
using JetBrains.Annotations;
using Microsoft.EntityFrameworkCore;

namespace Ggio.BikeSherpa.Backend.Infrastructure;

public class BackendDbContext(DbContextOptions<BackendDbContext> options) : DbContext(options)
{
     [UsedImplicitly]
     public virtual DbSet<Delivery> Deliveries { get; set; }
     [UsedImplicitly]
     public virtual DbSet<Customer> Customers { get; set; }
     [UsedImplicitly]
     public virtual DbSet<Courier> Couriers { get; set; }
     [UsedImplicitly]
     public virtual DbSet<PackingSize> PackingSizes { get; set; }
     [UsedImplicitly]
     public virtual DbSet<DeliveryZone> DeliveryZones { get; set; }
     [UsedImplicitly]
     public virtual DbSet<Urgency> Urgencies { get; set; }

     override protected void OnModelCreating(ModelBuilder modelBuilder)
     {
          modelBuilder.ApplyConfigurationsFromAssembly(typeof(BackendDbContext).Assembly);
     }

}
