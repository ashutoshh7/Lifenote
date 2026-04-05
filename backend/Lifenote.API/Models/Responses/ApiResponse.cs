namespace Lifenote.API.Models.Responses;

/// <summary>
/// Standard API response envelope for all endpoints.
/// Controllers return ApiResponse<T> — never raw Application DTOs.
/// 
/// Usage:
///   return Ok(ApiResponse<NoteDto>.Success(dto));
///   return BadRequest(ApiResponse<object>.Fail("Title is required."));
/// </summary>
public class ApiResponse<T>
{
    public bool Succeeded { get; init; }
    public string? Message { get; init; }
    public T? Data { get; init; }
    public IReadOnlyList<string>? Errors { get; init; }

    private ApiResponse() { }

    public static ApiResponse<T> Success(T data, string? message = null) =>
        new() { Succeeded = true, Data = data, Message = message };

    public static ApiResponse<T> Fail(string error) =>
        new() { Succeeded = false, Errors = new[] { error } };

    public static ApiResponse<T> Fail(IEnumerable<string> errors) =>
        new() { Succeeded = false, Errors = errors.ToList().AsReadOnly() };
}
