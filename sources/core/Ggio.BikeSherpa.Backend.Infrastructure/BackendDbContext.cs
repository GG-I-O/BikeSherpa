using Ggio.BikeSherpa.Backend.Domain.CourseAggregate;
using Ggio.BikeSherpa.Backend.Domain.CustomerAggregate;
using Ggio.BikeSherpa.Backend.Infrastructure.Interceptors;
using Microsoft.EntityFrameworkCore;

namespace Ggio.BikeSherpa.Backend.Infrastructure;

public class BackendDbContext(DbContextOptions<BackendDbContext> options) : DbContext(options)
{
     public virtual DbSet<Course> Courses { get; set; }
     public virtual DbSet<Customer> Customers { get; set; }
     
     private readonly DateInterceptor _dateInterceptor = new();

     override protected void OnModelCreating(ModelBuilder modelBuilder)
     {
          modelBuilder.ApplyConfigurationsFromAssembly(typeof(BackendDbContext).Assembly);
     }

     override protected void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
     {
          base.OnConfiguring(optionsBuilder);
          optionsBuilder.AddInterceptors(_dateInterceptor);
     }
}
