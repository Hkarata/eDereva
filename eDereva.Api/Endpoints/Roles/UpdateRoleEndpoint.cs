using eDereva.Core.Contracts.Requests;
using eDereva.Core.Entities;
using eDereva.Core.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace eDereva.Api.Endpoints.Roles;

public class UpdateRoleEndpoint(IRoleRepository roleRepository) : Endpoint<RoleDto, Results<Ok<Role>, BadRequest>>
{
    public override void Configure()
    {
        Put("/roles/{roleId}");
        Version(1);
        AllowAnonymous();
        Description(options =>
        {
            options.WithTags("Role")
                .WithSummary("Update an existing role")
                .WithDescription(
                    "This endpoint updates an existing role with the provided details. The role ID must be specified in the route, and the updated role details should be provided in the request body.");
        });
    }

    public override async Task<Results<Ok<Role>, BadRequest>> ExecuteAsync(RoleDto req, CancellationToken ct)
    {
        var roleId = Route<Guid>("roleId");

        var role = new Role
        {
            Id = roleId,
            Name = req.Name,
            Description = req.Description,
            Permission = new Permission
            {
                Flags = req.Permissions
            }
        };

        var result = await roleRepository.UpdateAsync(role, ct);

        if (result != null) return TypedResults.Ok(result);

        return TypedResults.BadRequest();
    }
}