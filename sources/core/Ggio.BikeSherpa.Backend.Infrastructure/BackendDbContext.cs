using Ggio.BikeSherpa.Backend.Domain.ClientAggregate;
using Ggio.BikeSherpa.Backend.Domain.CourseAggregate;
using Microsoft.EntityFrameworkCore;

namespace Ggio.BikeSherpa.Backend.Infrastructure;

public class BackendDbContext(DbContextOptions<BackendDbContext> options) : DbContext(options)
{
     public virtual DbSet<Course> Courses { get; set; }
     public virtual DbSet<Client> Clients { get; set; }

     override protected void OnModelCreating(ModelBuilder modelBuilder)
     {
          modelBuilder.ApplyConfigurationsFromAssembly(typeof(BackendDbContext).Assembly);
     }
}
