using eDereva.Core.Contracts.Responses;
using eDereva.Core.Repositories;
using eDereva.Core.ValueObjects;
using FastEndpoints;

namespace eDereva.Api.Endpoints.User;

public class GetUsersEndpoint(IUserRepository userRepository)
    : Endpoint<PaginationParams, PaginatedResult<UserDto>>
{
    public override void Configure()
    {
        Get("/users");
        Version(1);
        Policies("RequireViewUsers");
        Description(options =>
        {
            options.WithTags("User")
                .WithSummary("Retrieves a paginated list of users")
                .WithDescription(
                    "This endpoint retrieves a paginated list of users based on the provided pagination parameters. It supports filtering and sorting where applicable.");
        });
    }

    public override async Task HandleAsync(PaginationParams req, CancellationToken ct)
    {
        var users = await userRepository.GetAllAsync(req, ct);

        await SendOkAsync(users, ct);
    }
}