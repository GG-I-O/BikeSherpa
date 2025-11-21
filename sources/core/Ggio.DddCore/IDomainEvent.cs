using Mediator;

namespace Ggio.DddCore;

public interface IDomainEvent : INotification
{
  DateTime DateOccurred { get; }
}
