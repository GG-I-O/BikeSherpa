using Microsoft.EntityFrameworkCore;

namespace DddCore.Tests.Integration;

public class TestDbContext : DbContext
{
     public TestDbContext(DbContextOptions<TestDbContext> options) : base(options)
     {

     }

     public DbSet<MyAggregateRoot> MyAggregateRoots { get; set; }
}
