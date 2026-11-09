namespace BuildingBlocks.Application.Results;

public readonly record struct Result
{
    private Result(Error error)
    {
        Error = error;
    }

    public bool IsSuccess => !IsFailure;

    public bool IsFailure => Error.Code is not null;

    public Error Error { get; }

    public static Result Success() => default;

    public static Result Failure(Error error) => new(error);

    public static implicit operator Result(Error error) => Failure(error);
}

public readonly record struct Result<TValue>
{
    private readonly TValue? _value;

    private Result(TValue value)
    {
        _value = value;
    }

    private Result(Error error)
    {
        Error = error;
    }

    public bool IsSuccess => !IsFailure;

    public bool IsFailure => Error.Code is not null;

    public Error Error { get; }

    public TValue Value
    {
        get
        {
            if (IsFailure)
            {
                throw new InvalidOperationException(
                    "The value of a failed result is not available."
                );
            }

            return _value!;
        }
    }

    public static Result<TValue> Success(TValue value) => new(value);

    public static Result<TValue> Failure(Error error) => new(error);

    public static implicit operator Result<TValue>(TValue value) => Success(value);

    public static implicit operator Result<TValue>(Error error) => Failure(error);

    public static implicit operator Result(Result<TValue> result) =>
        result.IsSuccess ? Result.Success() : Result.Failure(result.Error);
}
