using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Ggio.DddCore.Infrastructure;

public static class ServiceExtension
{
     public static IServiceCollection AddDbContext<TDbContext>(this IServiceCollection services) where TDbContext : DbContext
     {
          //TODO ajouter les intercepteurs
          return services;
     }
}
