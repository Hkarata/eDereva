using eDereva.Core.ValueObjects;
using Microsoft.Extensions.Options;

namespace eDereva.Api.Extensions
{
    public class ApiKeyAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _validApiKey;

        public ApiKeyAuthenticationMiddleware(RequestDelegate next, IOptions<ApiSettings> apiSettings)
        {
            _next = next; // This will be automatically injected by the ASP.NET Core pipeline
            _validApiKey = apiSettings.Value.ApiKey;
        }

        public async Task InvokeAsync(HttpContext httpContext)
        {
            // Skip API key validation for the exempted path (e.g., /scalar/v1)
            if (httpContext.Request.Path.StartsWithSegments("/scalar/v1"))
            {
                await _next(httpContext); // Proceed to the next middleware if path is exempted
                return;
            }

            // Exempt routes like /openapi/{documentName}.json
            if (httpContext.Request.Path.StartsWithSegments("/openapi"))
            {
                await _next(httpContext);
                return;
            }

            if (!httpContext.Request.Headers.TryGetValue("X-Api-Key", out var apiKey) || apiKey.FirstOrDefault() != _validApiKey)
            {
                httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await httpContext.Response.WriteAsync("Invalid or missing API key.");
                return;
            }

            await _next(httpContext);
        }
    }

}
