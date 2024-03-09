using MediatR;
using Microsoft.AspNetCore.Mvc;
using WebApi.Features.PackingLists.Features.PackingListGet;

namespace WebApi.Controllers;

[ApiController]
[Route("api/packing-lists")]
public class PackingListController : ControllerBase
{
    private readonly IMediator _mediator;

    public PackingListController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<PackingListLookup>>> GetList()
    {
        var result = await _mediator.Send(new PackingListGetListQuery());

        return Ok(result);
    }
}