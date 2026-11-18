using System.Text.Json.Serialization;

namespace Catalog.Application.Features.Venues.Common.Layout;

[JsonConverter(typeof(JsonStringEnumConverter))]
public enum LayoutViolationCode
{
    NoSections,
    SeatedSectionWithoutRows,
    RowWithoutSeats,
}
