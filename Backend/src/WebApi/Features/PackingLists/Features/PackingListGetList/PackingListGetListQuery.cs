using MediatR;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Features.PackingLists.Features.PackingListGet;

public class PackingListGetListQuery
: IRequest<IEnumerable<PackingListLookup>>
{
    public class Handler : IRequestHandler<PackingListGetListQuery, IEnumerable<PackingListLookup>>
    {
        private readonly AppDbContext _dbContext;

        public Handler(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IEnumerable<PackingListLookup>> Handle(PackingListGetListQuery request, CancellationToken cancellationToken)
        {
            return await _dbContext
                .PackingLists
                .AsNoTracking()
                .Select(x => new PackingListLookup()
                {
                    OrderId = x.OrderId
                }).ToArrayAsync();
        }
    }
}

public record PackingListLookup
{
    public required Guid OrderId { get; init; }
}