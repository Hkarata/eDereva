using eDereva.Core.ValueObjects;
using Microsoft.Extensions.Options;

namespace eDereva.Api.Extensions
{
    public class ApiKeyAuthenticationMiddleware(RequestDelegate next, IOptions<ApiSettings> apiSettings)
    {
        private readonly string _validApiKey = apiSettings.Value.ApiKey;

        public async Task InvokeAsync(HttpContext httpContext)
        {
            // Skip API key validation for the exempted path (e.g., /scalar/v1)
            if (httpContext.Request.Path.StartsWithSegments("/scalar"))
            {
                await next(httpContext); // Proceed to the next middleware if path is exempted
                return;
            }

            // Exempt routes like /openapi/{documentName}.json
            if (httpContext.Request.Path.StartsWithSegments("/openapi"))
            {
                await next(httpContext);
                return;
            }

            if (!httpContext.Request.Headers.TryGetValue("X-Api-Key", out var apiKey) || apiKey.FirstOrDefault() != _validApiKey)
            {
                httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await httpContext.Response.WriteAsync("Invalid or missing API key.");
                return;
            }

            await next(httpContext);
        }
    }

}
