namespace BuildingBlocks.Domain.Results;

/// <summary>
/// An expected failure within the application.
/// </summary>
/// <param name="Code">Stable machine-readable identifier.</param>
/// <param name="Description">Description of a failure.</param>
/// <param name="Type">Category of an error.</param>
/// <param name="Details">Optional data about the error.</param>
public readonly record struct Error(
    string Code,
    string Description,
    ErrorType Type,
    object? Details = null
)
{
    public static Error NotFound(string code, string description, object? details = null) =>
        new(code, description, ErrorType.NotFound, details);

    public static Error Conflict(string code, string description, object? details = null) =>
        new(code, description, ErrorType.Conflict, details);

    public static Error Validation(string code, string description, object? details = null) =>
        new(code, description, ErrorType.Validation, details);
}
