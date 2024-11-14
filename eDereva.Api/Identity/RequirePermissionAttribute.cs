using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace eDereva.Api.Identity
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class RequirePermissionAttribute(PermissionFlag permission) : AuthorizeAttribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var user = context.HttpContext.User;
            if (!user.Identity?.IsAuthenticated ?? true)
            {
                context.Result = new UnauthorizedResult();
                return;
            }

            var permissionsClaim = user.Claims.FirstOrDefault(c => c.Type == "Permissions");
            if (permissionsClaim == null)
            {
                context.Result = new ForbidResult();
                return;
            }

            if (Enum.TryParse<PermissionFlag>(permissionsClaim.Value, out var userPermissions))
            {
                if (userPermissions.HasFlag(permission)) return;
                context.Result = new ForbidResult();
            }
            else
            {
                context.Result = new ForbidResult();
            }
        }
    }

    // Extension method to register the authorization services
    public static class AuthorizationServiceExtensions
    {
        public static IServiceCollection AddPermissionBasedAuthorization(this IServiceCollection services)
        {
            services.AddAuthorization(options =>
            {
                options.AddPolicy("RequireManageUsers", policy =>
                    policy.RequireClaim("Permissions", PermissionFlag.ManageUsers.ToString()));

                options.AddPolicy("RequireManageVenues", policy =>
                    policy.RequireClaim("Permissions", PermissionFlag.ManageVenues.ToString()));

                // Add other policies as needed
            });

            return services;
        }
    }
}