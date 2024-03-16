using MediatR;

namespace AgriTech.Infrastructure.Events;

public interface IDomainEventHandler<TDomainEvent> : INotificationHandler<TDomainEvent> where TDomainEvent : IDomainEvent
{
}

public interface IDomainEvent : INotification
{
}