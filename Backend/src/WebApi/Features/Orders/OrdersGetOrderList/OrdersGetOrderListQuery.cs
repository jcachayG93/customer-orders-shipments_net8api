using MediatR;
using Microsoft.EntityFrameworkCore;

namespace WebApi.Features.Orders.OrdersGetOrderList;

public class OrdersGetOrderListQuery : IRequest<ICollection<SalesOrderLookup>>
{
    public class Handler : IRequestHandler<OrdersGetOrderListQuery, ICollection<SalesOrderLookup>>
    {
        private readonly AppDbContext _dbContext;

        public Handler(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<ICollection<SalesOrderLookup>> Handle(OrdersGetOrderListQuery request, CancellationToken cancellationToken)
        {
            var orders = await _dbContext
                .SalesOrders.AsNoTracking()
                .Include(e=>e.SalesOrderLines)
                .ToArrayAsync();

            var result = orders.Select(o =>
                new SalesOrderLookup()
                {
                    OrderId = o.Id,
                    Total = o.Total
                }).ToArray();
            
            return result;
        }
    }
}