using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Pagination;
using BuildingBlocks.Application.Results;
using BuildingBlocks.Presentation.Extensions;
using Catalog.Application.Features.PricingZones.Common;
using Catalog.Application.Features.PricingZones.CreatePricingZone;
using Catalog.Application.Features.PricingZones.GetPricingZones;
using Catalog.Application.Features.Sections.Common;
using Catalog.Application.Features.Sections.CreateSections;
using Catalog.Application.Features.Sections.GetSections;
using Catalog.Application.Features.Venues.Common;
using Catalog.Application.Features.Venues.CreateVenue;
using Catalog.Application.Features.Venues.GetVenueById;
using Catalog.Application.Features.Venues.GetVenues;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Presentation.API.Venues;

[Route("[controller]")]
[ApiController]
public class VenuesController : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<VenueDto>>> Get(
        [FromQuery] GetVenuesQuery query,
        [FromServices] IQueryHandler<GetVenuesQuery, PagedResult<VenueDto>> handler,
        CancellationToken cancellationToken
    )
    {
        Result<PagedResult<VenueDto>> result = await handler.HandleAsync(query, cancellationToken);

        return result.IsSuccess ? result.Value : this.ToProblem(result.Error);
    }

    [HttpGet("{venueId:long}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<VenueDto>> GetById(
        [FromRoute] long venueId,
        [FromServices] IQueryHandler<GetVenueByIdQuery, VenueDto> handler,
        CancellationToken cancellationToken
    )
    {
        Result<VenueDto> result = await handler.HandleAsync(
            new GetVenueByIdQuery(venueId),
            cancellationToken
        );

        return result.IsSuccess ? result.Value : this.ToProblem(result.Error);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<ActionResult<long>> Create(
        [FromBody] CreateVenueCommand command,
        [FromServices] ICommandHandler<CreateVenueCommand, long> handler,
        CancellationToken cancellationToken
    )
    {
        Result<long> result = await handler.HandleAsync(command, cancellationToken);

        return result.IsSuccess
            ? CreatedAtAction(nameof(GetById), new { venueId = result.Value }, result.Value)
            : this.ToProblem(result.Error);
    }

    [HttpGet("{venueId:long}/pricing-zones")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<IReadOnlyList<PricingZoneDto>>> GetPricingZones(
        [FromRoute] long venueId,
        [FromServices] IQueryHandler<GetPricingZonesQuery, IReadOnlyList<PricingZoneDto>> handler,
        CancellationToken cancellationToken
    )
    {
        Result<IReadOnlyList<PricingZoneDto>> result = await handler.HandleAsync(
            new GetPricingZonesQuery(venueId),
            cancellationToken
        );

        return result.IsSuccess ? Ok(result.Value) : this.ToProblem(result.Error);
    }

    [HttpPost("{venueId:long}/pricing-zones")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<long>> CreatePricingZone(
        [FromRoute] long venueId,
        [FromBody] CreatePricingZoneCommand command,
        [FromServices] ICommandHandler<CreatePricingZoneCommand, long> handler,
        CancellationToken cancellationToken
    )
    {
        command = command with { VenueId = venueId };
        Result<long> result = await handler.HandleAsync(command, cancellationToken);

        return result.IsSuccess
            ? StatusCode(StatusCodes.Status201Created, result.Value)
            : this.ToProblem(result.Error);
    }

    [HttpPost("{venueId:long}/sections")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<IReadOnlyList<long>>> CreateSections(
        [FromRoute] long venueId,
        [FromBody] CreateSectionsCommand command,
        [FromServices] ICommandHandler<CreateSectionsCommand, IReadOnlyList<long>> handler,
        CancellationToken cancellationToken
    )
    {
        command = command with { VenueId = venueId };
        Result<IReadOnlyList<long>> result = await handler.HandleAsync(command, cancellationToken);

        return result.IsSuccess
            ? StatusCode(StatusCodes.Status201Created, result.Value)
            : this.ToProblem(result.Error);
    }

    [HttpGet("{venueId:long}/sections")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PagedResult<SectionDto>>> GetSections(
        [FromRoute] long venueId,
        [FromQuery] GetSectionsQuery query,
        [FromServices] IQueryHandler<GetSectionsQuery, PagedResult<SectionDto>> handler,
        CancellationToken cancellationToken
    )
    {
        query = query with { VenueId = venueId };
        var result = await handler.HandleAsync(query, cancellationToken);

        return result.IsSuccess ? result.Value : this.ToProblem(result.Error);
    }
}
