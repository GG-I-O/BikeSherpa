using Microsoft.EntityFrameworkCore;

namespace DddCore.Tests.Integration;

public class TestDbContext(DbContextOptions<TestDbContext> options) : DbContext(options)
{
     public DbSet<MyAggregateRoot> MyAggregateRoots { get; set; }
}
