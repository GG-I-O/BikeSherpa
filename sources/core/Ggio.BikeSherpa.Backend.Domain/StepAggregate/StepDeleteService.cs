using Ggio.DddCore;
using Mediator;

namespace Ggio.BikeSherpa.Backend.Domain.StepAggregate;

public interface IStepDeleteEventHandler
{
     Task DeleteStepAsync(Step step, CancellationToken cancellationToken);
}

public class StepDeleteService(IMediator mediator) : DeleteService(mediator), IStepDeleteEventHandler
{
     public async Task DeleteStepAsync(Step step, CancellationToken cancellationToken = default)
     {
          await NotifyEntityDeleted(step, cancellationToken);
     }
}