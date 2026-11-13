using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Pagination;
using BuildingBlocks.Application.Results;
using BuildingBlocks.Presentation.Extensions;
using Catalog.Application.Features.Events.Common;
using Catalog.Application.Features.Events.GetEvents;
using Catalog.Application.Features.Sections.GetSectionSeatMap;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Presentation.API.Events;

[Route("[controller]")]
[ApiController]
public class EventsController : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<EventDto>>> Get(
        [FromQuery] GetEventsQuery query,
        [FromServices] IQueryHandler<GetEventsQuery, PagedResult<EventDto>> handler,
        CancellationToken cancellationToken
    )
    {
        var result = await handler.HandleAsync(query, cancellationToken);

        return result.IsSuccess ? result.Value : this.ToProblem(result.Error);
    }

    [HttpGet("{eventId:long}/sections/{sectionId:long}/seats")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<SectionSeatMapDto>> GetSectionSeatMap(
        [FromRoute] long eventId,
        [FromRoute] long sectionId,
        [FromServices] IQueryHandler<GetSectionSeatMapQuery, SectionSeatMapDto> handler,
        CancellationToken cancellationToken
    )
    {
        Result<SectionSeatMapDto> result = await handler.HandleAsync(
            new GetSectionSeatMapQuery(eventId, sectionId),
            cancellationToken
        );

        return result.IsSuccess ? result.Value : this.ToProblem(result.Error);
    }
}
