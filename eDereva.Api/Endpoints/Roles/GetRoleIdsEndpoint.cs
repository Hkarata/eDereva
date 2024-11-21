using eDereva.Core.Repositories;
using FastEndpoints;

namespace eDereva.Api.Endpoints.Roles;

public class GetRoleIdsEndpoint(IRoleRepository roleRepository) : EndpointWithoutRequest<List<Guid>>
{
    public override void Configure()
    {
        Get("/roles/ids");
        Version(1);
        AllowAnonymous();
        Description(options =>
        {
            options.WithTags("Role")
                .WithSummary("Get all role IDs")
                .WithDescription(
                    "This endpoint retrieves a list of all role IDs in the system. Returns a list of unique identifiers (GUIDs) for all available roles.");
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var roleIds = await roleRepository.GetRoleIdsAsync(ct);

        if (roleIds.Count == 0)
        {
            await SendNoContentAsync(ct);
            return;
        }

        await SendOkAsync(roleIds, ct);
    }
}