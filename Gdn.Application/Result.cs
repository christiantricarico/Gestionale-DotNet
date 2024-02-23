namespace Gdn.Application;

public class Result<TData>
{
    public bool IsSuccess { get; }
    public TData? Data { get; }
    public Error? Error { get; }

    private Result(TData data)
    {
        IsSuccess = true;
        Data = data;
    }

    private Result(Error error)
    {
        IsSuccess = false;
        Error = error;
    }

    public static implicit operator Result<TData>(Error error) => new(error);
    public static implicit operator Result<TData>(TData data) => new(data);

    public static Result<TData> Success(TData data) => new(data);
    public static Result<TData> Failure(Error error) => new(error);

    public TReturn Match<TReturn>(Func<TData?, TReturn> OnSuccess, Func<Error?, TReturn> OnFailure)
    {
        return IsSuccess ? OnSuccess(Data) : OnFailure(Error);
    }
}

public sealed record Error(string Code, string Description)
{
    public static readonly Error None = new(string.Empty, string.Empty);
}