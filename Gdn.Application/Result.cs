namespace Gdn.Application;

public class Result
{
    public bool IsSuccess { get; set; }
    public string? Message { get; set; }

    public static Result Success()
    {
        return new Result()
        {
            IsSuccess = true,
            Message = string.Empty
        };
    }

    public static Result<TData> Success<TData>(TData data) where TData : class
    {
        return new Result<TData>()
        {
            IsSuccess = true,
            Message = string.Empty,
            Data = data
        };
    }

    public static Result Error(string errorMessage)
    {
        return new Result()
        {
            IsSuccess = false,
            Message = errorMessage
        };
    }
}

public class Result<TData> : Result
    where TData : class
{
    public TData? Data { get; set; }
}
