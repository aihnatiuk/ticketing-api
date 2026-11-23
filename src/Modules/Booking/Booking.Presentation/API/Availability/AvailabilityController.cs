using Booking.Application.Features.Availability.GetAvailability;
using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Results;
using BuildingBlocks.Presentation.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Booking.Presentation.API.Availability;

[ApiController]
[Route("/events/{eventId:long}/availability")]
[Tags("Events")]
public class AvailabilityController : ControllerBase
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<AvailabilityDto>> GetAvailability(
        [FromRoute] long eventId,
        [FromServices] IQueryHandler<GetAvailabilityQuery, AvailabilityDto> handler,
        CancellationToken cancellationToken
    )
    {
        GetAvailabilityQuery query = new(eventId);
        Result<AvailabilityDto> result = await handler.HandleAsync(query, cancellationToken);

        return result.IsSuccess ? result.Value : this.ToProblem(result.Error);
    }
}
