using MediatR;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Features.PackingLists.Features;

public class PackingListGetDetailsQuery
: IRequest<PackingListDetailsViewModel>
{
    public required Guid OrderId { get; init; }

    public class Handler : IRequestHandler<PackingListGetDetailsQuery, PackingListDetailsViewModel>
    {
        private readonly AppDbContext _dbContext;

        public Handler(
            AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }
        
        public async Task<PackingListDetailsViewModel> Handle(
            PackingListGetDetailsQuery request, 
            CancellationToken cancellationToken)
        {
            var packingList = await _dbContext
                .PackingLists
                .AsNoTracking()
                .Include(e=>e.Lines)
                .FirstAsync(p => p.OrderId == request.OrderId);

            return new PackingListDetailsViewModel
            {
                OrderId = packingList.OrderId,
                Lines = packingList.Lines.Select(l =>
                    new PackingListDetailsViewModel.PackingListLineDto
                    {
                        ProductName = l.Product,
                        Quantity = l.Quantity
                    }).ToArray()
            };
        }
    }
}