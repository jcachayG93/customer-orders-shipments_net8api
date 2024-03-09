namespace WebApi.Features.PackingLists.Features;

public record PackingListDetailsViewModel
{
    public required Guid OrderId { get; init; }

    public required IEnumerable<PackingListLineDto> Lines { get; init; }

    public record PackingListLineDto
    {
        public required string ProductName { get; init; }

        public required int Quantity { get; init; }
    }
}