using BuildingBlocks.Application.Abstractions;
using Catalog.Application.Features.Rows.Common;

namespace Catalog.Application.Features.Rows.GetRowById;

public sealed record GetRowByIdQuery(long Id) : IQuery<RowDto>;
