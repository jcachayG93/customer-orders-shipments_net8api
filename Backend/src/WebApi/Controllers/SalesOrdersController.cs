using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebApi.Features.Orders.OrdersAddRemoveLines;
using WebApi.Features.Orders.OrdersCreate;
using WebApi.Features.Orders.OrdersGetOrderList;

namespace WebApi.Controllers;

[ApiController]
[Route("api/sale-orders")]
public class SalesOrdersController : ControllerBase
{
    private readonly IMediator _mediator;

    public SalesOrdersController(
        IMediator mediator)
    {
        _mediator = mediator;
    }
    
    /// <summary>
    /// Creates a sales order.
    /// </summary>
    [HttpPost]
    [Route("{orderId:guid}")]
    public async Task<ActionResult> CreateOrder(Guid orderId)
    {
        var command = new OrdersCreateOrderCommand()
        {
            OrderId = orderId
        };

        await _mediator.Send(command);

        return Created();
    }

    [HttpGet]
    public async Task<ActionResult<ICollection<SalesOrderLookup>>> GetAll()
    {
        var query = new OrdersGetOrderListQuery();

        var result = await _mediator.Send(query);

        return Ok(result);
    }

    [HttpPost("add-remove-lines")]
    public async Task<ActionResult> AddRemoveLines(
        OrdersAddRemoveLinesCommand command)
    {
        await _mediator.Send(command);
        return Ok();
    }
}