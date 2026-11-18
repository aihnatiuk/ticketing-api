using BuildingBlocks.Application.Abstractions;
using BuildingBlocks.Application.Pagination;
using BuildingBlocks.Application.Results;
using BuildingBlocks.Presentation.Extensions;
using Catalog.Application.Features.Rows.Common;
using Catalog.Application.Features.Rows.CreateRows;
using Catalog.Application.Features.Rows.GetRows;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.Presentation.API.Sections;

[Route("[controller]")]
[ApiController]
public class SectionsController : ControllerBase
{
    [HttpPost("{sectionId:long}/rows")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<IReadOnlyList<long>>> CreateRows(
        [FromRoute] long sectionId,
        [FromBody] CreateRowsCommand command,
        [FromServices] ICommandHandler<CreateRowsCommand, IReadOnlyList<long>> handler,
        CancellationToken cancellationToken
    )
    {
        command = command with { SectionId = sectionId };
        Result<IReadOnlyList<long>> result = await handler.HandleAsync(command, cancellationToken);

        return result.IsSuccess
            ? StatusCode(StatusCodes.Status201Created, result.Value)
            : this.ToProblem(result.Error);
    }

    [HttpGet("{sectionId:long}/rows")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PagedResult<RowDto>>> GetRows(
        [FromRoute] long sectionId,
        [FromQuery] GetRowsQuery query,
        [FromServices] IQueryHandler<GetRowsQuery, PagedResult<RowDto>> handler,
        CancellationToken cancellationToken
    )
    {
        query = query with { SectionId = sectionId };
        Result<PagedResult<RowDto>> result = await handler.HandleAsync(query, cancellationToken);

        return result.IsSuccess ? result.Value : this.ToProblem(result.Error);
    }
}
