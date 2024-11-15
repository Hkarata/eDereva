using eDereva.Core.Interfaces;
using FastEndpoints;

namespace eDereva.Api.Endpoints.Roles
{
    public class GetAllRolesEndpoint(IRoleRepository roleRepository) : EndpointWithoutRequest<List<Core.Entities.Role>>
    {
        public override void Configure()
        {
            Get("/roles");
            Version(1);
            Policies("RequireAdministrator");
            Description(options =>
            {
                options.WithTags("Role")
                    .WithSummary("Get all roles")
                    .WithDescription("This endpoint retrieves all the roles in the system.");
            });
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var roles = await roleRepository.GetAllAsync(ct);

            if (roles.Count == 0)
            {
                await SendNotFoundAsync(ct);
                return;

            }

            await SendOkAsync(roles, ct);
            return;
        }
    }
}