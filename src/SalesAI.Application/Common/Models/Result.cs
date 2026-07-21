namespace SalesAI.Application.Common.Models;

public class Result<T>
{
    public bool Succeeded { get; init; }
    public T? Data { get; init; }
    public string? Message { get; init; }
    public List<string> Errors { get; init; } = [];

    public static Result<T> Success(T data, string? message = null) =>
        new() { Succeeded = true, Data = data, Message = message };

    public static Result<T> Failure(string message, List<string>? errors = null) =>
        new() { Succeeded = false, Message = message, Errors = errors ?? [] };

    public static Result<T> NotFound(string entityName, object key) =>
        new() { Succeeded = false, Message = $"{entityName} with key '{key}' was not found." };

    public static Result<T> Forbidden(string message = "You do not have permission to perform this action.") =>
        new() { Succeeded = false, Message = message };

    public static Result<T> ValidationFailure(List<string> errors) =>
        new() { Succeeded = false, Message = "Validation failed.", Errors = errors };
}
