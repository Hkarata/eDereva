﻿using eDereva.Core.Contracts.Requests;
using eDereva.Core.Entities;
using eDereva.Core.Interfaces;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace eDereva.Api.Endpoints.Role
{
    public class CreateRoleEndpoint(IRoleRepository roleRepository) : Endpoint<RoleDto, Core.Entities.Role>
    {
        public override void Configure()
        {
            Post("/roles");
            Version(1);
            AllowAnonymous();
            Description(options =>
            {
                options.WithTags("Role")
                    .WithSummary("Create a new role")
                    .WithDescription("This endpoint allows the creation of a new role with associated permissions. The request should include role details such as name, description, and any permissions to be assigned to the role.");
            });
        }

        public override async Task HandleAsync(RoleDto req, CancellationToken ct)
        {
            var role = new Core.Entities.Role
            {
                Id = Guid.NewGuid(),
                Name = req.Name,
                Description = req.Description,
                Permission = new Permission
                {
                    Flags = req.Permissions
                }
            };

            var result = await roleRepository.AddAsync(role, ct);

            if (result)
            {
                await SendCreatedAtAsync<CreateRoleEndpoint>($"/roles/{role.Id}", role, cancellation: ct);
            }
            else
            {
                await SendNoContentAsync(cancellation: ct);
            }
        }
    }
}