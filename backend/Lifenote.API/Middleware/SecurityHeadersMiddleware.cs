using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace Lifenote.API.Middleware
{
    public class SecurityHeadersMiddleware
    {
        private readonly RequestDelegate _next;

        public SecurityHeadersMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            context.Response.OnStarting(() =>
            {
                var headers = context.Response.Headers;
                headers.Remove("Server");
                headers.Remove("X-Powered-By");

                headers.Append("X-Content-Type-Options", "nosniff");
                headers.Append("X-Frame-Options", "DENY");
                headers.Append("X-XSS-Protection", "1; mode=block");
                headers.Append("Content-Security-Policy", "default-src 'self';");

                return Task.CompletedTask;
            });

            await _next(context);
        }
    }
}
