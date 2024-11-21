using eDereva.Core.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace eDereva.Api.Identity;

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

        var permissionsClaim = user.Claims.FirstOrDefault(c => c.Type == "Permission");
        if (permissionsClaim == null)
        {
            context.Result = new ForbidResult();
            return;
        }

        var permissionsList = permissionsClaim.Value.Split(',').Select(p => p.Trim()).ToList();
        if (permissionsList.Contains(permission.ToString()))
        {
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
        services.AddAuthorizationBuilder()
            .AddPolicy("RequireViewUsers", policy =>
                policy.RequireAssertion(context =>
                    context.User.HasClaim(c =>
                        c.Type == "Permission" && c.Value.Split(',').Contains(PermissionFlag.ViewUsers.ToString()))))
            .AddPolicy("RequireEditUsers", policy =>
                policy.RequireAssertion(context =>
                    context.User.HasClaim(c =>
                        c.Type == "Permission" && c.Value.Split(',').Contains(PermissionFlag.EditUsers.ToString()))))
            .AddPolicy("RequireDeleteUsers", policy =>
                policy.RequireAssertion(context =>
                    context.User.HasClaim(c =>
                        c.Type == "Permission" && c.Value.Split(',').Contains(PermissionFlag.DeleteUsers.ToString()))))
            .AddPolicy("RequireManageUsers", policy =>
                policy.RequireAssertion(context =>
                    context.User.HasClaim(c =>
                        c.Type == "Permission" && c.Value.Split(',').Contains(PermissionFlag.ManageUsers.ToString()))))
            .AddPolicy("RequireViewVenues", policy =>
                policy.RequireAssertion(context =>
                    context.User.HasClaim(c =>
                        c.Type == "Permission" && c.Value.Split(',').Contains(PermissionFlag.ViewVenues.ToString()))))
            .AddPolicy("RequireEditVenues", policy =>
                policy.RequireAssertion(context =>
                    context.User.HasClaim(c =>
                        c.Type == "Permission" && c.Value.Split(',').Contains(PermissionFlag.EditVenues.ToString()))))
            .AddPolicy("RequireDeleteVenues", policy =>
                policy.RequireAssertion(context =>
                    context.User.HasClaim(c =>
                        c.Type == "Permission" && c.Value.Split(',').Contains(PermissionFlag.DeleteVenues.ToString()))))
            .AddPolicy("RequireManageVenues", policy =>
                policy.RequireAssertion(context =>
                    context.User.HasClaim(c =>
                        c.Type == "Permission" && c.Value.Split(',').Contains(PermissionFlag.ManageVenues.ToString()))))
            .AddPolicy("RequireViewSessions", policy =>
                policy.RequireAssertion(context =>
                    context.User.HasClaim(c =>
                        c.Type == "Permission" && c.Value.Split(',').Contains(PermissionFlag.ViewSessions.ToString()))))
            .AddPolicy("RequireCreateSessions", policy =>
                policy.RequireAssertion(context =>
                    context.User.HasClaim(c =>
                        c.Type == "Permission" &&
                        c.Value.Split(',').Contains(PermissionFlag.CreateSessions.ToString()))))
            .AddPolicy("RequireEditSessions", policy =>
                policy.RequireAssertion(context =>
                    context.User.HasClaim(c =>
                        c.Type == "Permission" && c.Value.Split(',').Contains(PermissionFlag.EditSessions.ToString()))))
            .AddPolicy("RequireDeleteSessions", policy =>
                policy.RequireAssertion(context =>
                    context.User.HasClaim(c =>
                        c.Type == "Permission" &&
                        c.Value.Split(',').Contains(PermissionFlag.DeleteSessions.ToString()))))
            .AddPolicy("RequireManageSessions", policy =>
                policy.RequireAssertion(context =>
                    context.User.HasClaim(c =>
                        c.Type == "Permission" &&
                        c.Value.Split(',').Contains(PermissionFlag.ManageSessions.ToString()))))
            .AddPolicy("RequireViewQuestionBanks", policy =>
                policy.RequireAssertion(context =>
                    context.User.HasClaim(c =>
                        c.Type == "Permission" &&
                        c.Value.Split(',').Contains(PermissionFlag.ViewQuestionBanks.ToString()))))
            .AddPolicy("RequireEditQuestionBanks", policy =>
                policy.RequireAssertion(context =>
                    context.User.HasClaim(c =>
                        c.Type == "Permission" &&
                        c.Value.Split(',').Contains(PermissionFlag.EditQuestionBanks.ToString()))))
            .AddPolicy("RequireDeleteQuestionBanks", policy =>
                policy.RequireAssertion(context =>
                    context.User.HasClaim(c =>
                        c.Type == "Permission" &&
                        c.Value.Split(',').Contains(PermissionFlag.DeleteQuestionBanks.ToString()))))
            .AddPolicy("RequireManageQuestionBanks", policy =>
                policy.RequireAssertion(context =>
                    context.User.HasClaim(c =>
                        c.Type == "Permission" &&
                        c.Value.Split(',').Contains(PermissionFlag.ManageQuestionBanks.ToString()))))
            .AddPolicy("RequireViewTests", policy =>
                policy.RequireAssertion(context =>
                    context.User.HasClaim(c =>
                        c.Type == "Permission" && c.Value.Split(',').Contains(PermissionFlag.ViewTests.ToString()))))
            .AddPolicy("RequireEditTests", policy =>
                policy.RequireAssertion(context =>
                    context.User.HasClaim(c =>
                        c.Type == "Permission" && c.Value.Split(',').Contains(PermissionFlag.EditTests.ToString()))))
            .AddPolicy("RequireDeleteTests", policy =>
                policy.RequireAssertion(context =>
                    context.User.HasClaim(c =>
                        c.Type == "Permission" && c.Value.Split(',').Contains(PermissionFlag.DeleteTests.ToString()))))
            .AddPolicy("RequireManageTests", policy =>
                policy.RequireAssertion(context =>
                    context.User.HasClaim(c =>
                        c.Type == "Permission" && c.Value.Split(',').Contains(PermissionFlag.ManageTests.ToString()))))
            .AddPolicy("RequireViewBookings", policy =>
                policy.RequireAssertion(context =>
                    context.User.HasClaim(c =>
                        c.Type == "Permission" && c.Value.Split(',').Contains(PermissionFlag.ViewBookings.ToString()))))
            .AddPolicy("RequireCreateBookings", policy =>
                policy.RequireAssertion(context =>
                    context.User.HasClaim(c =>
                        c.Type == "Permission" &&
                        c.Value.Split(',').Contains(PermissionFlag.CreateBookings.ToString()))))
            .AddPolicy("RequireEditBookings", policy =>
                policy.RequireAssertion(context =>
                    context.User.HasClaim(c =>
                        c.Type == "Permission" && c.Value.Split(',').Contains(PermissionFlag.EditBookings.ToString()))))
            .AddPolicy("RequireDeleteBookings", policy =>
                policy.RequireAssertion(context =>
                    context.User.HasClaim(c =>
                        c.Type == "Permission" &&
                        c.Value.Split(',').Contains(PermissionFlag.DeleteBookings.ToString()))))
            .AddPolicy("RequireManageBookings", policy =>
                policy.RequireAssertion(context =>
                    context.User.HasClaim(c =>
                        c.Type == "Permission" &&
                        c.Value.Split(',').Contains(PermissionFlag.ManageBookings.ToString()))))
            .AddPolicy("RequireViewSoftDeletedData", policy =>
                policy.RequireAssertion(context =>
                    context.User.HasClaim(c =>
                        c.Type == "Permission" &&
                        c.Value.Split(',').Contains(PermissionFlag.ViewSoftDeletedData.ToString()))))
            .AddPolicy("RequireAdministrator", policy =>
                policy.RequireAssertion(context =>
                    context.User.HasClaim(c =>
                        c.Type == "Permission" &&
                        c.Value.Split(',').Contains(PermissionFlag.Administrator.ToString()))));
        return services;
    }
}