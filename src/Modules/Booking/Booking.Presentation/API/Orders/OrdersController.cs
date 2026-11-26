using Booking.Application.Features.Orders.Common;
using Booking.Application.Features.Orders.CreateOrder;
using Booking.Application.Features.Orders.GetOrder;
using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Domain.Results;
using BuildingBlocks.Presentation.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Presentation.API.Orders;

[Route("[controller]")]
[ApiController]
public class OrdersController : ControllerBase
{
    [HttpGet("{orderId:long}")]
    public async Task<ActionResult<OrderDto>> Get(
        [FromRoute] long orderId,
        [FromServices] IQueryHandler<GetOrderQuery, OrderDto> queryHandler,
        CancellationToken cancellationToken
    )
    {
        GetOrderQuery query = new(orderId);
        Result<OrderDto> result = await queryHandler.HandleAsync(query, cancellationToken);

        return result.IsSuccess ? result.Value : this.ToProblem(result.Error);
    }

    [HttpPost]
    public async Task<ActionResult<OrderDto>> Create(
        [FromBody] CreateOrderCommand command,
        [FromServices] ICommandHandler<CreateOrderCommand, OrderDto> handler,
        CancellationToken cancellationToken
    )
    {
        Result<OrderDto> result = await handler.HandleAsync(command, cancellationToken);

        return result.IsSuccess ? result.Value : this.ToProblem(result.Error);
    }
}
