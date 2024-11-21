using eDereva.Core.Contracts.Requests;
using eDereva.Core.Repositories;
using eDereva.Core.Services;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace eDereva.Api.Endpoints.User;

public class CreateUserEndpoint(
    IUserRepository userRepository,
    IRoleRepository roleRepository,
    IPasswordService passwordService)
    : Endpoint<CreateUserDto, Results<Ok, BadRequest>>
{
    public override void Configure()
    {
        Post("users");
        Version(1);
        AllowAnonymous();
        Description(options =>
        {
            options.WithTags("User")
                .WithSummary("Create a new user")
                .WithDescription("Create a new user");
        });
        Throttle(
            10,
            60,
            "X-Client-Id" // this is optional
        );
    }

    public override async Task HandleAsync(CreateUserDto req, CancellationToken ct)
    {
        var user = new Core.Entities.User
        {
            FirstName = req.FirstName,
            LastName = req.LastName,
            Email = req.Email,
            PhoneNumber = req.PhoneNumber,
            DateOfBirth = req.DateOfBirth,
            Nin = req.Nin,
            MiddleName = req.MiddleName,
            Password = passwordService.HashPassword(req.Password)
        };

        var role = await roleRepository.GetByNameAsync("Basic User", ct);

        if (role != null) user.Roles?.Add(role);

        await userRepository.AddAsync(user, ct);
    }
}