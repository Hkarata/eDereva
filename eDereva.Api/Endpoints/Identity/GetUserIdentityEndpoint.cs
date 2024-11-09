using eDereva.Api.Exceptions;
using eDereva.Core.Contracts.Responses;
using eDereva.Core.Services;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace eDereva.Api.Endpoints.Identity
{
    public class GetUserIdentityEndpoint(INIDAService nidaService) : EndpointWithoutRequest<Results<Ok<UserDto>, BadRequest>>
    {
        public override void Configure()
        {
            Get("/identity/{NIN}");
            Version(1);
            AllowAnonymous();
            Description(options =>
            {
                options.WithTags("Identity")
                    .WithSummary("Get User Identity")
                    .WithDescription("Get user identity information from NIDA");
            });
            Throttle(
                hitLimit: 5,
                durationSeconds: 60,
                headerName: "X-Client-Id" // this is optional
            );
        }

        public override async Task<Results<Ok<UserDto>, BadRequest>> ExecuteAsync(CancellationToken ct)
        {
            var NIN = Route<string>("NIN");

            if (string.IsNullOrEmpty(NIN))
            {
                throw new ProblemException("NIN is required", "please provide the National identification number");
            }

            var userdata = await nidaService.LoadUserDataAsync(NIN);

            return userdata is null
                ? throw new ProblemException("User not found", "User with the provided NIN not found")
                : (Results<Ok<UserDto>, BadRequest>)TypedResults.Ok(userdata);
        }

    }
}
