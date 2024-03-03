using Domain.Common;
using MediatR;

namespace WebApi;

public class DomainEventDispatcher : IDomainEventDispatcher
{
    private readonly IMediator _mediator;

    public DomainEventDispatcher(IMediator mediator)
    {
        _mediator = mediator;
    }
    
    public async Task Dispatch(Entity entity)
    {
        var domainEvents = entity.DomainEvents.ToArray();
        foreach (var domainEvent in domainEvents)
        {
            await _mediator.Publish(domainEvent);
        }
    }
}