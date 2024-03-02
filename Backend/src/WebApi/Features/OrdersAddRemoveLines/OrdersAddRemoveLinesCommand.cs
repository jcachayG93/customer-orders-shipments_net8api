using MediatR;
using WebApi.Persistence;

namespace WebApi.Features.OrdersAddRemoveLines;

public class OrdersAddRemoveLinesCommand : IRequest
{
    public required Guid OrderId { get; init; }

    public required ICollection<OrderLineDto> Lines { get; init; }
        = Array.Empty<OrderLineDto>();

    public class OrderLineDto
    {
        public required Guid OrderLineId { get; init; }

        public required string Product { get; init; }
        
        public required int Quantity { get; init; }

        public required decimal UnitPrice { get; init; }
    }

    public class Handler : IRequestHandler<OrdersAddRemoveLinesCommand>
    {
        public Handler(ISalesOrdersRepository repository)
        {
            
        }
        
        public Task Handle(OrdersAddRemoveLinesCommand request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}