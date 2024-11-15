using eDereva.Core.Entities;
using eDereva.Core.Interfaces;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace eDereva.Api.Endpoints.Roles
{
    public class GetRoleEndpoint(IRoleRepository roleRepository) : EndpointWithoutRequest<Results<Ok<Role>, BadRequest>>
    {
        public override void Configure()
        {
            Get("/roles/{roleId}");
            Version(1);
            Policies("RequireAdministrator");
            Description(options =>
            {
                options.WithTags("Role")
                    .WithSummary("Get role by ID")
                    .WithDescription("This endpoint retrieves a specific role by its unique identifier. Returns the role details including name, description, and associated permissions.");
            });
        }

        public override async Task<Results<Ok<Role>, BadRequest>> ExecuteAsync(CancellationToken ct)
        {
            var roleId = Route<Guid>("roleId");

            var result = await roleRepository.GetByIdAsync(roleId, ct);

            return result == null ? TypedResults.BadRequest() : TypedResults.Ok(result);
        }
    }
}
