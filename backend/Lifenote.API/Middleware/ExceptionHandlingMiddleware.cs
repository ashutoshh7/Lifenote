using System.Net;
using System.Text.Json;

namespace Lifenote.API.Middleware;

/// <summary>
/// Global exception handler — replaces inline try/catch blocks in every controller.
/// Catches domain exceptions and returns consistent JSON error responses.
/// Controllers are now free of exception-handling boilerplate.
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
        catch (KeyNotFoundException ex)
        {
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
            _logger.LogError(ex, "Unhandled exception");
            await WriteResponse(context, HttpStatusCode.InternalServerError, "An unexpected error occurred.");
        }
    }

    private static async Task WriteResponse(HttpContext context, HttpStatusCode statusCode, string message)
    {
        context.Response.StatusCode = (int)statusCode;
        context.Response.ContentType = "application/json";
        await context.Response.WriteAsync(JsonSerializer.Serialize(new { error = message }));
    }
}
