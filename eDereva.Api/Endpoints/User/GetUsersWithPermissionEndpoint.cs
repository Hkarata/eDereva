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
                headerName: "X-Client-Id"
            );
        }

        public override async Task<Results<Ok<List<UserData>>, BadRequest>> ExecuteAsync(CancellationToken ct)
        {
            var permissionId = Route<int>("permissionId");
            var users = await userRepository.GetUsersWithPermissionsAsync(permissionId);

            if (users == null || !users.Any())
            {
                throw new ProblemException("No users found", "No users were found with the specified permission");
            }

            var userData = users.Select(user => new UserData
            {
                Id = user.Id,
                NIN = user.NIN,
                FirstName = user.FirstName,
                MiddleName = user.MiddleName,
                LastName = user.LastName,
            }).ToList();

            return TypedResults.Ok(userData);
        }
    }
}