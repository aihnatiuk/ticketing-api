namespace BuildingBlocks.Application.Results;

/// <summary>
/// An expected failure of a use case.
/// </summary>
/// <param name="Code">Stable machine-readable identifier.</param>
/// <param name="Description">Description of a failure.</param>
/// <param name="Type">Category of an error.</param>
/// <param name="Details">Optional structured data the client can use to render the error.</param>
public readonly record struct Error(
    string Code,
    string Description,
    ErrorType Type,
    object? Details = null
)
{
    public static Error NotFound(string code, string description) =>
        new(code, description, ErrorType.NotFound);

    public static Error Conflict(string code, string description, object? details = null) =>
        new(code, description, ErrorType.Conflict, details);

    public static Error Validation(string code, string description) =>
        new(code, description, ErrorType.Validation);
}
