using Domain.Common;

namespace WebApi;

public interface IDomainEventDispatcher
{
    Task Dispatch(Entity entity);
}