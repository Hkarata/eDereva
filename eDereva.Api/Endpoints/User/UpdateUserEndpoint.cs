using eDereva.Api.Exceptions;
using eDereva.Core.Contracts.Requests;
using eDereva.Core.Interfaces;
using eDereva.Core.Services;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace eDereva.Api.Endpoints.User
{
    public class UpdateUserEndpoint(IUserRepository userRepository, IPasswordService passwordService) : Endpoint<UpdateUserDto, Results<Ok, BadRequest>>
    {
        public override void Configure()
        {
            Put("/users/{userId}");
            Version(1);
            AllowAnonymous();
            Description(options =>
            {
                options.WithTags("User")
                        .WithSummary("Update a user")
                        .WithDescription("Update a user");
            });
            Throttle(
                hitLimit: 10,
                durationSeconds: 60,
                headerName: "X-Client-Id" // this is optional
            );
        }

        public override async Task HandleAsync(UpdateUserDto req, CancellationToken ct)
        {
            var userId = Route<Guid>("userId");

            if (userId == Guid.Empty)
            {
                throw new ProblemException("user Id is required", "please provide the user Id");
            }

            var user = await userRepository.GetByIdAsync(userId) ?? throw new ProblemException("User not found", "User with the provided Id does not exist");

            user.Email = req.Email;
            user.PhoneNumber = req.PhoneNumber;
            user.Password = passwordService.HashPassword(req.Password);


            await userRepository.UpdateAsync(user);

            return;
        }
    }
}
