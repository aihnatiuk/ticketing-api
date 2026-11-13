using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Pagination;
using BuildingBlocks.Application.Results;
using BuildingBlocks.Presentation.Extensions;
using Catalog.Application.Features.Sections.Common;
using Catalog.Application.Features.Sections.GetSections;
using Catalog.Application.Features.Venues.Common;
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
        Result<PagedResult<VenueDto>> result = await handler.HandleAsync(
            query,
            cancellationToken
        );

        return result.IsSuccess ? result.Value : this.ToProblem(result.Error);
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
