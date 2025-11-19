using Lab4.Features.Orders.Requests;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Lab4.Features.Orders;

[ApiController]
[Route("api/orders")]
public class OrderController : ControllerBase
{
    private readonly IMediator _mediator;

    public OrderController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("create")]
    public async Task<IActionResult> CreateOrder([FromBody] CreateOrderProfileRequest request)
    {
        var result = await _mediator.Send(request);
        return Ok(result);
    }
}