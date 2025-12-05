using Ggio.DddCore;
using Ggio.DddCore.Infrastructure.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Xunit.Internal;

namespace DddCore.Tests.Integration;

public class MediatorTestFixture : IDisposable
{
     private readonly static IServiceCollection ServiceCollectionInternal = new ServiceCollection();

     static MediatorTestFixture()
     {
          ServiceCollectionInternal.AddMediator(options =>
          {
               options.Assemblies = [typeof(MyAggregateRoot).Assembly, typeof(IDomainEvent), typeof(EfCoreDomainEntityAddedEventHandler)];
               options.ServiceLifetime = ServiceLifetime.Scoped;
          });
     }

     public IServiceCollection GetServiceCollection()
     {
          var collection = new ServiceCollection();
          ServiceCollectionInternal.ForEach(x => collection.Add(x));
          return collection;
     }

     public void Dispose()
     {
          GC.SuppressFinalize(this);
     }
}
