using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Results;
using BuildingBlocks.Presentation.Extensions;
using Catalog.Application.Features.Seats.Common;
using Catalog.Application.Features.Seats.CreateSeats;
using Catalog.Application.Features.Seats.GetSeats;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Presentation.API.Rows;

[Route("[controller]")]
[ApiController]
public class RowsController : ControllerBase
{
    [HttpPost("{rowId:long}/seats")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<IReadOnlyList<long>>> CreateSeats(
        [FromRoute] long rowId,
        [FromBody] CreateSeatsCommand command,
        [FromServices] ICommandHandler<CreateSeatsCommand, IReadOnlyList<long>> handler,
        CancellationToken cancellationToken
    )
    {
        command = command with { RowId = rowId };
        Result<IReadOnlyList<long>> result = await handler.HandleAsync(command, cancellationToken);

        return result.IsSuccess
            ? StatusCode(StatusCodes.Status201Created, result.Value)
            : this.ToProblem(result.Error);
    }

    [HttpGet("{rowId:long}/seats")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<SeatDto>>> GetSeats(
        [FromRoute] long rowId,
        [FromServices] IQueryHandler<GetSeatsByRowQuery, IReadOnlyList<SeatDto>> handler,
        CancellationToken cancellationToken
    )
    {
        Result<IReadOnlyList<SeatDto>> result = await handler.HandleAsync(
            new GetSeatsByRowQuery(rowId),
            cancellationToken
        );

        return result.IsSuccess ? Ok(result.Value) : this.ToProblem(result.Error);
    }
}
