using eDereva.Core.Contracts.Requests;
using eDereva.Core.Enums;
using eDereva.Core.Repositories;
using eDereva.Core.Services;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace eDereva.Api.Endpoints.Identity;

public class AuthenticateEndpoint(
    IUserRepository userRepository,
    IRoleRepository roleRepository,
    ITokenService tokenService)
    : Endpoint<AuthenticateDto, Results<Ok<string>, BadRequest>>
{
    public override void Configure()
    {
        Post("/identity/authenticate");
        Version(1);
        AllowAnonymous();
        Description(options =>
        {
            options.WithTags("Authentication")
                .WithSummary("Authenticate User")
                .WithDescription(
                    "Authenticates a user by their credentials and returns a JWT token for accessing secured endpoints.");
        });
    }

    public override async Task<Results<Ok<string>, BadRequest>> ExecuteAsync(AuthenticateDto req, CancellationToken ct)
    {
        var authenticationResults = await userRepository
            .AuthenticateAsync(req.PhoneNumber, req.Password, ct);

        if (authenticationResults.IsAuthenticated == false) 
            return TypedResults.BadRequest();

        PermissionFlag permissionFlag;

        if (req.IsDriver)
            permissionFlag = await roleRepository.GetBasicRolePermissionFlag();
        else
            permissionFlag = await userRepository.GetAggregatePermissionFlag(req.PhoneNumber, ct);

        var userdata = await userRepository.GetUserDataAsync(req.PhoneNumber, ct);

        var token = tokenService.GenerateToken(userdata.nin, userdata.givenName,
            userdata.surname, userdata.email, userdata.phoneNumber, permissionFlag);


        return TypedResults.Ok(token);
    }
}