using eDereva.Api.Exceptions;
using eDereva.Core.Contracts.Responses;
using eDereva.Core.Interfaces;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace eDereva.Api.Endpoints.User
{
    public class GetUsersWithPermissionEndpoint(IUserRepository userRepository) : EndpointWithoutRequest<Results<Ok<List<UserData>>, BadRequest>>
    {
        public override void Configure()
        {
            Get("/users/permissions/{permissionId}");
            Version(1);
            AllowAnonymous();
            Description(options =>
            {
                options.WithTags("User")
                        .WithSummary("Get users with a specific permission")
                        .WithDescription("Get all users with a specific permission");
            });
            Throttle(
                hitLimit: 15,
                durationSeconds: 60,
                headerName: "X-Client-Id" // this is optional
            );
        }

        public override Task<Results<Ok<List<UserData>>, BadRequest>> ExecuteAsync(CancellationToken ct)
        {
            var permissionId = Route<int>("permissionId");

            var users = userRepository.GetUsersWithPermissionsAsync(permissionId);

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
