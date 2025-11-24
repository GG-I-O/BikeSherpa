using Microsoft.EntityFrameworkCore;

namespace Ggio.BikeSherpa.Backend.Infrastructure;

public class BackendDbContext(DbContextOptions<BackendDbContext> options) : DbContext(options);
