using Domain.Entities.OrderAggregate.DomainEvents;
using Domain.Entities.PackingListAggregate;
using Domain.ValueObjects;
using MediatR;
using WebApi.Features.PackingLists.Common;

namespace WebApi.Features.PackingLists.Features.PackingListCreate;

public class PackingListCreateWhenOrderOrderedDomainEventHandler
: INotificationHandler<OrderOrdered>
{
    private readonly IPackingListRepository _packingListRepository;
    /*
     * When an order is ordered, a Packing list is created.
     * OrderOrdered is a domain event raised by the SalesOrder aggregate.
     */


    public PackingListCreateWhenOrderOrderedDomainEventHandler(IPackingListRepository packingListRepository)
    {
        _packingListRepository = packingListRepository;
    }
    public async Task Handle(OrderOrdered notification, CancellationToken cancellationToken)
    {
        var packingList = new PackingList(EntityIdentity.Random, new(notification.OrderId));

        foreach (var line in notification.Lines)
        {
            packingList.AddLine(
                new(line.LineId),
                new(line.Product),
                new(line.Quantity));
        }

        await _packingListRepository.AddAsync(packingList);
        /*
         * IMPORTANT! we are not committing here, why?
         * 1. The domain event is raised in the SalesOrderAggregate
         * 2. The domain event is published in the AppDbContext, just before the data is saved to the database.
         * 3. Then, this code runs, but it does not commit changes.
         * 4. Then, the DbContext finishes committing the changes.
         */
    }
}