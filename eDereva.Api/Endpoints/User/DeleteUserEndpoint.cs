using eDereva.Api.Exceptions;
using eDereva.Core.Interfaces;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace eDereva.Api.Endpoints.User
{
    public class DeleteUserEndpoint(IUserRepository userRepository) : EndpointWithoutRequest<Results<Ok, BadRequest>>
    {
        public override void Configure()
        {
            Delete("/users/{userId}");
            Version(1);
            AllowAnonymous();
            Description(options =>
            {
                options.WithTags("User")
                        .WithSummary("Delete user")
                        .WithDescription("Delete the specified user");
            });
            Throttle(
                hitLimit: 15,
                durationSeconds: 60,
                headerName: "X-Client-Id" // this is optional
            );
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var userId = Route<Guid>("userId");

            if (userId == Guid.Empty)
            {
                throw new ProblemException("user Id is required", "please provide the user Id");
            }

            await userRepository.DeleteAsync(userId);

            await SendOkAsync(ct);
            return;
        }
    }
}
