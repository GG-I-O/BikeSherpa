using Ggio.DddCore;
using Mediator;

namespace DddCore.Tests.Integration;

public class MyAggragateRootFactory(IMediator mediator) : FactoryBase(mediator)
{
     public async Task<MyAggregateRoot> CreateAsync(string name)
     {
          var entity = new MyAggregateRoot { Name = name };
          await NotifyNewEntityAdded(entity);
          return entity;
     }
}
