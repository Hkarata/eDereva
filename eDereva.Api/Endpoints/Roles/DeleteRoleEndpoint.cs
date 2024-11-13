using eDereva.Core.Interfaces;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace eDereva.Api.Endpoints.Roles
{
    public class DeleteRoleEndpoint(IRoleRepository roleRepository) : EndpointWithoutRequest<Results<Ok, BadRequest>>
    {
        public override void Configure()
        {
            Delete("/roles/{roleId}");
            Version(1);
            AllowAnonymous();
            Description(options =>
            {
                options.WithTags("Role")
                .WithSummary("Delete a role")
                .WithDescription("This endpoint allows the deletion of an existing role. The role name or ID should be provided in the request to identify the role to be deleted.");
            });
        }

        public override async Task<Results<Ok, BadRequest>> ExecuteAsync(CancellationToken ct)
        {
            var roleId = Route<Guid>("roleId");

            var result = await roleRepository.DeleteAsync(roleId, ct);

            return result ? TypedResults.Ok() : TypedResults.BadRequest();
        }
    }
}
