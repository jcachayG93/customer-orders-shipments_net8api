using Domain.Common;
using MediatR;
using WebApi.Exceptions;
using WebApi.Features.Orders.Common;

namespace WebApi.Features.Orders.OrderMarkAsOrdered;

public class OrderMarkAsOrderedCommand : IRequest
{
    public required Guid OrderId { get; init; }

    public class Handler : IRequestHandler<OrderMarkAsOrderedCommand>
    {
        private readonly ISalesOrdersRepository _repository;
        private readonly IMediator? _mediator;

        public Handler(ISalesOrdersRepository repository, IMediator? mediator = null)
        {
            _repository = repository;
            _mediator = mediator;
        }
        public async Task Handle(OrderMarkAsOrderedCommand request, CancellationToken cancellationToken)
        {
            var salesOrder = await _repository.GetByIdAsync(new(request.OrderId));

            if (salesOrder is null)
            {
                throw new EntityNotFoundException($"SalesOrder with Id: {request.OrderId} was not found.");
            }
            
            salesOrder.MarkAsOrdered();

            await _repository.CommitChangesAsync();
        }
    }
}