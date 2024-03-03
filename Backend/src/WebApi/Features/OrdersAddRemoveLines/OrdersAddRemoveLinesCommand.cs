using Domain.Entities.OrderAggregate;
using Domain.ValueObjects;
using MediatR;
using WebApi.Exceptions;
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
        private readonly ISalesOrdersRepository _repository;

        public Handler(ISalesOrdersRepository repository)
        {
            _repository = repository;
        }
        
        public async Task Handle(OrdersAddRemoveLinesCommand request, CancellationToken cancellationToken)
        {
            // Load the aggregate
            var aggregate = await _repository.GetByIdAsync(new(request.OrderId));

            if (aggregate is null)
            {
                throw new EntityNotFoundException($"SalesOrder with Id: {request.OrderId} was not found.");
            }
            
            // Delete
            DeleteAggregateLinesMissingFromTheCommand(request, aggregate);

            // Upsert
            UpsertLines(request, aggregate);
            
            // commit changes
            await _repository.CommitChangesAsync();
        }

        private static void UpsertLines(OrdersAddRemoveLinesCommand request, ISalesOrderRoot aggregate)
        {
            foreach (var line in request.Lines)
            {
                if (aggregate.DoesLineExists(new(line.OrderLineId)))
                {
                    // update
                    aggregate.UpdateLine(
                        new(line.OrderLineId),
                        new(line.Product),
                        new(line.Quantity),
                        Money.CreateInDollars(line.UnitPrice));
                }
                else
                {
                    // insert
                    aggregate.AddLine(
                        new(line.OrderLineId),
                        new(line.Product),
                        new(line.Quantity),
                        Money.CreateInDollars(line.UnitPrice));
                }
            }
        }

        private static void DeleteAggregateLinesMissingFromTheCommand(OrdersAddRemoveLinesCommand request,
            ISalesOrderRoot aggregate)
        {
            var existingLineIds = aggregate.GetLineIds();
            var linesInTheAggregateButNotInTheCommand = existingLineIds.Where(lid => request.Lines.All(x =>
                x.OrderLineId != lid.Value)).ToArray();
            foreach (var toDeleteId in linesInTheAggregateButNotInTheCommand)
            {
                aggregate.RemoveLine(toDeleteId);
            }
        }
    }
}