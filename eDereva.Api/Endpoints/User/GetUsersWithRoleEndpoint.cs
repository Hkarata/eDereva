using eDereva.Api.Exceptions;
using eDereva.Core.Contracts.Responses;
using eDereva.Core.Interfaces;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace eDereva.Api.Endpoints.User
{
    public class GetUsersWithRoleEndpoint(IUserRepository userRepository) : EndpointWithoutRequest<Results<Ok<List<UserData>>, BadRequest>>
    {
        public override void Configure()
        {
            Get("/api/users/role/{roleId}");
            Version(1);
            AllowAnonymous();
            Description(options =>
            {
                options.WithTags("User")
                        .WithSummary("Get all users with a specific role")
                        .WithDescription("Retrieve all users with a specific role");
            });
            Throttle(
                hitLimit: 20,
                durationSeconds: 60,
                headerName: "X-Client-Id" // this is optional
            );
        }

        public override async Task<Results<Ok<List<UserData>>, BadRequest>> ExecuteAsync(CancellationToken ct)
        {
            var roleId = Route<Guid>("roleId");

            if (roleId == Guid.Empty)
            {
                throw new ProblemException("Invalid role ID", "The role ID is invalid");
            }

            var users = await userRepository.GetUsersByRoleAsync(roleId);

            return users == null
                ? throw new ProblemException("No users found", "No users were found")
                : (Results<Ok<List<UserData>>, BadRequest>)TypedResults.Ok(users.Select(user => new UserData
                {
                    Id = user.Id,
                    NIN = user.NIN,
                    FirstName = user.FirstName,
                    MiddleName = user.MiddleName,
                    LastName = user.LastName,
                }).ToList());
        }
    }
}