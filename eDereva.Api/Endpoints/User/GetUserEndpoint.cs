using eDereva.Api.Exceptions;
using eDereva.Core.Contracts.Responses;
using eDereva.Core.Entities;
using eDereva.Core.Interfaces;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace eDereva.Api.Endpoints.User
{
    public class GetUserEndpoint(IUserRepository userRepository) : EndpointWithoutRequest<Results<Ok<UserData>, BadRequest>>
    {
        public override void Configure()
        {
            Get("/api/users/{nin}");
            Version(1);
            Policies("RequireViewUsers");
            Description(options =>
            {
                options.WithTags("User")
                    .WithSummary("Get user by Id")
                    .WithDescription("Retrieve a specific user by ID");
            });
            Throttle(
                hitLimit: 20,
                durationSeconds: 60,
                headerName: "X-Client-Id" // this is optional
            );
        }

        public override async Task<Results<Ok<UserData>, BadRequest>> ExecuteAsync(CancellationToken ct)
        {
            var nin = Route<string>("nin")!;

            if (string.IsNullOrEmpty(nin))
            {
                throw new ProblemException("Invalid user ID", "The user ID provided is invalid");
            }

            var user = await userRepository.GetByIdAsync(nin, ct);

            return user == null
                ? throw new ProblemException("User not found", "The user with the specified ID was not found")
                : (Results<Ok<UserData>, BadRequest>)TypedResults.Ok(user.MaptoUserData());
        }
    }
}