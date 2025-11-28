using Ggio.DddCore.Infrastructure.ApplicationTransaction;
using Ggio.DddCore.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Ggio.DddCore.Infrastructure;

public static class ServiceExtension
{
     extension(IServiceCollection services)
     {
          public IServiceCollection AddDddDbContext<TDbContext>(Action<IServiceProvider, DbContextOptionsBuilder> dbBuilder) where TDbContext : DbContext
          {
               services.AddScoped<IApplicationTransaction, ApplicationTransaction<TDbContext>>();

               services.AddDbContext<TDbContext>(dbBuilder);

               return services;
          }

          public IServiceCollection AddInfrastructureServices()
          {
               services.AddScoped(typeof(IReadRepository<>), typeof(EfCoreReadRepository<>))
                    .AddScoped<IApplicationTransactionContext, ApplicationTransactionContext>()
                    .AddScoped<IDomainEventDispatcher, MediatorDomainEventDispatcher>();
               return services;
          }
     }
}
