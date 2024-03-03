namespace WebApi.Features.Orders.OrdersGetOrderList;

public class SalesOrderLookup
{
    public required Guid OrderId { get; init; }

    public required decimal Total { get; init; }
}