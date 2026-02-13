using Ggio.BikeSherpa.Backend.Domain.CourierAggregate;
using Ggio.BikeSherpa.Backend.Domain.DeliveryAggregate;
using Ggio.BikeSherpa.Backend.Domain.CustomerAggregate;
using Microsoft.EntityFrameworkCore;

namespace Ggio.BikeSherpa.Backend.Infrastructure;

public class BackendDbContext(DbContextOptions<BackendDbContext> options) : DbContext(options)
{
     public virtual DbSet<Delivery> Deliveries { get; set; }
     public virtual DbSet<Customer> Customers { get; set; }
     public virtual DbSet<Courier> Couriers { get; set; }
     public virtual DbSet<PackingSizeEntity> PackingSizes { get; set; }
     public virtual DbSet<DeliveryZoneEntity> DeliveryZones { get; set; }
     public virtual DbSet<UrgencyEntity> Urgencies { get; set; }

     override protected void OnModelCreating(ModelBuilder modelBuilder)
     {
          modelBuilder.ApplyConfigurationsFromAssembly(typeof(BackendDbContext).Assembly);
     }

}
