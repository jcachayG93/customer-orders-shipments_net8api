using MediatR;

namespace Domain.Common;

/// <summary>
/// Base class for an entity that can emit domain events.
/// </summary>
public abstract class Entity
{
    private readonly List<IDomainEvent> _domainEvents = new();

    protected void AddDomainEvent(IDomainEvent domainEvent)
    {
        _domainEvents.Add(domainEvent);
    }

    public IEnumerable<IDomainEvent> DomainEvents => _domainEvents;
}