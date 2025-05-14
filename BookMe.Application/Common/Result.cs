using System.Text.Json.Serialization;
using BookMe.Application.Common.Errors;

namespace BookMe.Application.Common;

public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public IEnumerable<Error> Errors { get; }
    [JsonIgnore]
    public ErrorType ErrorType { get; }
    public ResultSuccessType SuccessType { get; }

    public Result()
    {
    }

    protected Result(bool isSuccess, IEnumerable<Error> errors, ErrorType errorType, ResultSuccessType successType = ResultSuccessType.Retrieved)
    {
        if ((isSuccess && errors.Any()) || (!isSuccess && (errors is null || !errors.Any())))
        {
            throw new ArgumentException("Invalid arguments", nameof(errors));
        }

        IsSuccess = isSuccess;
        Errors = errors;
        ErrorType = errorType;
        SuccessType = successType;
    }

    public static Result Success(ResultSuccessType successType = ResultSuccessType.Retrieved)
    {
        return new Result(true, new List<Error>(), ErrorType.None, successType);
    }

    public static Result Failure(Error error, ErrorType errorType)
    {
        return new Result(false, new List<Error> { error }, errorType);
    }

    public static Result Failure(IEnumerable<Error> errors, ErrorType errorType)
    {
        return new Result(false, errors, errorType);
    }
}

public class Result<T> : Result
{
    public T Value { get; }

    private Result(bool isSuccess, IEnumerable<Error> error, ErrorType errorType, T result = default)
        : base(isSuccess, error, errorType)
    {
        Value = result;
    }

    public static Result<T> Success(T result)
    {
        return new Result<T>(true, new List<Error>(), ErrorType.None, result);
    }

    public static new Result<T> Failure(Error error, ErrorType errorType)
    {
        return new Result<T>(false, new List<Error> { error }, errorType);
    }

    public static new Result<T> Failure(IEnumerable<Error> errors, ErrorType errorType)
    {
        return new Result<T>(false, errors, errorType);
    }
}