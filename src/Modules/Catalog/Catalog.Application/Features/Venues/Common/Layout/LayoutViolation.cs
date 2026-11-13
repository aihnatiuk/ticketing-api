namespace Catalog.Application.Features.Venues.Common.Layout;

/// <summary>
/// A single kind of problem found in a venue layout.
/// </summary>
/// <param name="Code">Stable identifier of the problem.</param>
/// <param name="Labels">
/// Labels of the offending layout structures, truncated to a limit.
/// Empty when the problem applies to the venue as a whole.
/// </param>
public sealed record LayoutViolation(LayoutViolationCode Code, IReadOnlyList<string> Labels);
