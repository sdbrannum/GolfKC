namespace Web.Dtos;

public sealed class Result<T>
{
    public T? Data { get; init; }
    public string? Error { get; init; }
    public bool Failed => !string.IsNullOrWhiteSpace(Error);

    public static Result<T> Ok(T data)
    {
        return new Result<T>()
        {
            Data = data
        };
    }

    public static Result<T> Fail(string? error = "An error occurred")
    {
        return new Result<T>()
        {
            Data = default(T),
            Error = error
        };
    }
}