using Domain.Entities.OrderAggregate;
using MediatR;
using WebApi.Persistence;

namespace WebApi.Features.OrdersCreate;

/*
 * There is an integration test in WebApi.Tests, at the controller level.
 */
public class OrdersCreateOrderCommand
: IRequest
{
    /*
     * The decision on whether or not to set the Id from the client side depends on many factors:
     * 1. How disconnected is the front-end? will it be able to creat orders, add lines, and then commit when the user decides so,
     * or will it be making an API call each time.
     *
     * Important to consider, idempotency. The client may retry api calls, so the same api call may be received more than once. In
     * that case we should not create multiple orders. To avoid that we implement an idempotent command: If a command to create an
     * order that already exists is received, it is just ignored, this way it does not matter how many times we receive it, and
     * the client can implement any retry strategy it needs.
     */
    /// <summary>
    /// The order id.
    /// </summary>
    public required Guid OrderId { get; init; }

    public class Handler : IRequestHandler<OrdersCreateOrderCommand>
    {
        private readonly ISalesOrdersRepository _repository;

        public Handler(ISalesOrdersRepository repository)
        {
            _repository = repository;
        }
        public async Task Handle(OrdersCreateOrderCommand request, CancellationToken cancellationToken)
        {
            var order = new SalesOrder(new(request.OrderId));
            await _repository.AddAsync(order);
            await _repository.CommitChangesAsync();
        }
    }
}