using Domain.Common;
using MediatR;

namespace Domain.Entities.OrderAggregate.DomainEvents;

public record OrderOrdered : IDomainEvent
{
    public required Guid OrderId { get; init; }

    public required IEnumerable<OrderOrderedLine> Lines { get; init; }
    public record OrderOrderedLine
    {
        public required Guid LineId { get; init; }

        public required string Product { get; init; }

        public required int Quantity { get; init; }
    }
}

