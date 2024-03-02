using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebApi.Features.OrdersCreate;

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
}