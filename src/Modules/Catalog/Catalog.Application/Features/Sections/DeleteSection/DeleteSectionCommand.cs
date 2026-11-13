using BuildingBlocks.Application.Abstractions;

namespace Catalog.Application.Features.Sections.DeleteSection;

public sealed record DeleteSectionCommand(long Id) : ICommand;
