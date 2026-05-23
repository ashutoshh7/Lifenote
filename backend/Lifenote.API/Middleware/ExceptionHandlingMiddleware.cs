using Lifenote.Domain.Exceptions;
using System.Net;
using System.Text.Json;

namespace Lifenote.API.Middleware;

/// <summary>
/// Global exception handler — replaces inline try/catch blocks in every controller.
///
/// Mapping table (evaluated top-to-bottom; first match wins):
///   NotFoundException        → 404 Not Found
///   ForbiddenException       → 403 Forbidden
///   ConflictException        → 409 Conflict
///   DomainException (base)   → 400 Bad Request
///   ArgumentException        → 400 Bad Request
///   InvalidOperationException→ 400 Bad Request
///   UnauthorizedAccessException → 401 Unauthorized
///   Exception (catch-all)    → 500 Internal Server Error (message hidden from client)
/// </summary>
public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
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
            await WriteResponse(context, HttpStatusCode.NotFound, ex.Message);
        }
        catch (ForbiddenException ex)
        {
            await WriteResponse(context, HttpStatusCode.Forbidden, ex.Message);
        }
        catch (ConflictException ex)
        {
            await WriteResponse(context, HttpStatusCode.Conflict, ex.Message);
        }
        catch (DomainException ex)
        {
            // Base domain exception → generic 400
            await WriteResponse(context, HttpStatusCode.BadRequest, ex.Message);
        }
        catch (KeyNotFoundException ex)
        {
            // Legacy: services that haven't migrated to NotFoundException yet
            await WriteResponse(context, HttpStatusCode.NotFound, ex.Message);
        }
        catch (ArgumentException ex)
        {
            await WriteResponse(context, HttpStatusCode.BadRequest, ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            await WriteResponse(context, HttpStatusCode.BadRequest, ex.Message);
        }
        catch (UnauthorizedAccessException ex)
        {
            await WriteResponse(context, HttpStatusCode.Unauthorized, ex.Message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception on {Method} {Path}",
                context.Request.Method, context.Request.Path);
            await WriteResponse(context, HttpStatusCode.InternalServerError,
                "An unexpected error occurred.");
        }
    }

    private static async Task WriteResponse(HttpContext context, HttpStatusCode statusCode, string message)
    {
        context.Response.StatusCode  = (int)statusCode;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(
            JsonSerializer.Serialize(new { error = message }));
    }
}
