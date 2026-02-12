using System.Text.Json.Serialization;
using Account.Application.Common.Errors;

namespace Account.Application.Common.Results;

public class Result
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingDefault)]
    public Error? Error { get; }
    public bool IsSuccess { get; }

    protected Result()
    {
        IsSuccess = true;
        Error = default;
    }

    protected Result(Error error)
    {
        IsSuccess = false;
        Error = error;
    }

    public static implicit operator Result(Error error) =>
        new(error);

    public static Result Success() =>
        new();

    public static Result Failure(Error error) =>
        new(error);
}
