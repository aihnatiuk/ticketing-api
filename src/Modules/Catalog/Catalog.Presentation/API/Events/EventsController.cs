using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Pagination;
using BuildingBlocks.Application.Results;
using BuildingBlocks.Presentation.Extensions;
using Catalog.Application.Features.Events.Common;
using Catalog.Application.Features.Events.CreateEvent;
using Catalog.Application.Features.Events.GetEventById;
using Catalog.Application.Features.Events.GetEvents;
using Catalog.Application.Features.Events.PublishEvent;
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

    [HttpGet("{eventId:long}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<EventDto>> GetById(
        [FromRoute] long eventId,
        [FromServices] IQueryHandler<GetEventByIdQuery, EventDto> handler,
        CancellationToken cancellationToken
    )
    {
        Result<EventDto> result = await handler.HandleAsync(
            new GetEventByIdQuery(eventId),
            cancellationToken
        );

        return result.IsSuccess ? result.Value : this.ToProblem(result.Error);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<long>> Create(
        [FromBody] CreateEventCommand command,
        [FromServices] ICommandHandler<CreateEventCommand, long> handler,
        CancellationToken cancellationToken
    )
    {
        Result<long> result = await handler.HandleAsync(command, cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { eventId = result.Value }, result.Value)
            : this.ToProblem(result.Error);
    }

    [HttpPost("{eventId:long}/publish")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult> Publish(
        [FromRoute] long eventId,
        [FromServices] ICommandHandler<PublishEventCommand> handler,
        CancellationToken cancellationToken
    )
    {
        Result result = await handler.HandleAsync(
            new PublishEventCommand(eventId),
            cancellationToken
        );

        return result.IsSuccess ? NoContent() : this.ToProblem(result.Error);
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
