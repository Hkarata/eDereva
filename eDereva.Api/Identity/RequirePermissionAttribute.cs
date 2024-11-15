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
                // User permissions
                options.AddPolicy("RequireViewUsers", policy =>
                    policy.RequireClaim("Permissions", PermissionFlag.ViewUsers.ToString()));
                options.AddPolicy("RequireEditUsers", policy =>
                    policy.RequireClaim("Permissions", PermissionFlag.EditUsers.ToString()));
                options.AddPolicy("RequireDeleteUsers", policy =>
                    policy.RequireClaim("Permissions", PermissionFlag.DeleteUsers.ToString()));
                options.AddPolicy("RequireManageUsers", policy =>
                    policy.RequireClaim("Permissions", PermissionFlag.ManageUsers.ToString()));

                // Venue permissions
                options.AddPolicy("RequireViewVenues", policy =>
                    policy.RequireClaim("Permissions", PermissionFlag.ViewVenues.ToString()));
                options.AddPolicy("RequireEditVenues", policy =>
                    policy.RequireClaim("Permissions", PermissionFlag.EditVenues.ToString()));
                options.AddPolicy("RequireDeleteVenues", policy =>
                    policy.RequireClaim("Permissions", PermissionFlag.DeleteVenues.ToString()));
                options.AddPolicy("RequireManageVenues", policy =>
                    policy.RequireClaim("Permissions", PermissionFlag.ManageVenues.ToString()));

                // Question Bank permissions
                options.AddPolicy("RequireViewQuestionBanks", policy =>
                    policy.RequireClaim("Permissions", PermissionFlag.ViewQuestionBanks.ToString()));
                options.AddPolicy("RequireEditQuestionBanks", policy =>
                    policy.RequireClaim("Permissions", PermissionFlag.EditQuestionBanks.ToString()));
                options.AddPolicy("RequireDeleteQuestionBanks", policy =>
                    policy.RequireClaim("Permissions", PermissionFlag.DeleteQuestionBanks.ToString()));
                options.AddPolicy("RequireManageQuestionBanks", policy =>
                    policy.RequireClaim("Permissions", PermissionFlag.ManageQuestionBanks.ToString()));

                // Test permissions
                options.AddPolicy("RequireViewTests", policy =>
                    policy.RequireClaim("Permissions", PermissionFlag.ViewTests.ToString()));
                options.AddPolicy("RequireEditTests", policy =>
                    policy.RequireClaim("Permissions", PermissionFlag.EditTests.ToString()));
                options.AddPolicy("RequireDeleteTests", policy =>
                    policy.RequireClaim("Permissions", PermissionFlag.DeleteTests.ToString()));
                options.AddPolicy("RequireManageTests", policy =>
                    policy.RequireClaim("Permissions", PermissionFlag.ManageTests.ToString()));

                // Booking permissions
                options.AddPolicy("RequireViewBookings", policy =>
                    policy.RequireClaim("Permissions", PermissionFlag.ViewBookings.ToString()));
                options.AddPolicy("RequireCreateBookings", policy =>
                    policy.RequireClaim("Permissions", PermissionFlag.CreateBookings.ToString()));
                options.AddPolicy("RequireEditBookings", policy =>
                    policy.RequireClaim("Permissions", PermissionFlag.EditBookings.ToString()));
                options.AddPolicy("RequireDeleteBookings", policy =>
                    policy.RequireClaim("Permissions", PermissionFlag.DeleteBookings.ToString()));
                options.AddPolicy("RequireManageBookings", policy =>
                    policy.RequireClaim("Permissions", PermissionFlag.ManageBookings.ToString()));

                // Special permissions
                options.AddPolicy("RequireViewSoftDeletedData", policy =>
                    policy.RequireClaim("Permissions", PermissionFlag.ViewSoftDeletedData.ToString()));

                // Composite permissions
                options.AddPolicy("RequireAdministrator", policy =>
                    policy.RequireClaim("Permissions", PermissionFlag.Administrator.ToString()));
            });
            return services;
        }
    }
}