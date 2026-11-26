using Booking.Application.Features.Orders.AddOrderItems;
using Booking.Application.Features.Orders.Common;
using Booking.Application.Features.Orders.CreateOrder;
using Booking.Application.Features.Orders.GetOrder;
using Booking.Application.Features.Orders.RemoveOrderItem;
using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Domain.Results;
using BuildingBlocks.Presentation.Extensions;
using Microsoft.AspNetCore.Http;
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

    [HttpPost("{orderId:long}/items")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<OrderDto>> AddItems(
        [FromRoute] long orderId,
        [FromBody] OrderItemsRequestDto items,
        [FromServices] ICommandHandler<AddOrderItemsCommand, OrderDto> handler,
        CancellationToken cancellationToken
    )
    {
        AddOrderItemsCommand command = new(orderId, items);
        Result<OrderDto> result = await handler.HandleAsync(command, cancellationToken);

        return result.IsSuccess ? result.Value : this.ToProblem(result.Error);
    }

    [HttpDelete("{orderId:long}/items/{itemId:long}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<OrderDto>> RemoveItem(
        [FromRoute] long orderId,
        [FromRoute] long itemId,
        [FromServices] ICommandHandler<RemoveOrderItemCommand, OrderDto> handler,
        CancellationToken cancellationToken
    )
    {
        RemoveOrderItemCommand command = new(orderId, itemId);
        Result<OrderDto> result = await handler.HandleAsync(command, cancellationToken);

        return result.IsSuccess ? result.Value : this.ToProblem(result.Error);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
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
