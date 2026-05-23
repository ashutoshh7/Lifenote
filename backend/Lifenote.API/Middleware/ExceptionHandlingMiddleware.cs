using Lifenote.API.Models.Responses;
using Lifenote.Domain.Exceptions;
using System.Net;
using System.Text.Json;

namespace Lifenote.API.Middleware;

/// <summary>
/// Global exception handler — replaces inline try/catch blocks in every controller.
///
/// All error responses are serialised as ApiResponse&lt;object&gt;.Fail(...) so clients
/// always receive the same envelope shape:
///   { "succeeded": false, "errors": ["..."] }
///
/// Mapping table (evaluated top-to-bottom; first match wins):
///   NotFoundException           → 404 Not Found
///   ForbiddenException          → 403 Forbidden
///   ConflictException           → 409 Conflict
///   DomainException (base)      → 400 Bad Request
///   KeyNotFoundException        → 404 Not Found  (legacy — prefer NotFoundException)
///   ArgumentException           → 400 Bad Request
///   InvalidOperationException   → 400 Bad Request
///   UnauthorizedAccessException → 401 Unauthorized
///   Exception (catch-all)       → 500 Internal Server Error (internal message hidden)
/// </summary>
public class ExceptionHandlingMiddleware
{
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next   = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (NotFoundException ex)
        {
            await WriteError(context, HttpStatusCode.NotFound, ex.Message);
        }
        catch (ForbiddenException ex)
        {
            await WriteError(context, HttpStatusCode.Forbidden, ex.Message);
        }
        catch (ConflictException ex)
        {
            await WriteError(context, HttpStatusCode.Conflict, ex.Message);
        }
        catch (DomainException ex)
        {
            // Base domain exception → generic 400
            await WriteError(context, HttpStatusCode.BadRequest, ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            // Legacy: services that haven't migrated to NotFoundException yet
            await WriteError(context, HttpStatusCode.NotFound, ex.Message);
        }
        catch (ArgumentException ex)
        {
            await WriteError(context, HttpStatusCode.BadRequest, ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            await WriteError(context, HttpStatusCode.BadRequest, ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            await WriteError(context, HttpStatusCode.Unauthorized, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception on {Method} {Path}",
                context.Request.Method, context.Request.Path);
            await WriteError(context, HttpStatusCode.InternalServerError,
                "An unexpected error occurred.");
        }
    }

    /// <summary>
    /// Writes a camelCase-serialised ApiResponse&lt;object&gt;.Fail envelope.
    /// Using ApiResponse here keeps the error shape identical to what controllers
    /// return on the happy path — one contract, one shape, always.
    /// </summary>
    private static async Task WriteError(
        HttpContext context, HttpStatusCode statusCode, string message)
    {
        context.Response.StatusCode  = (int)statusCode;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(
            JsonSerializer.Serialize(
                ApiResponse<object>.Fail(message),
                _jsonOptions));
    }
}
