using eDereva.Core.Contracts.Requests;
using eDereva.Core.Interfaces;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace eDereva.Api.Endpoints.User
{
    public class CreateUserEndpoint(IUserRepository userRepository) : Endpoint<CreateUserDto, Results<Ok, BadRequest>>
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
                hitLimit: 10,
                durationSeconds: 60,
                headerName: "X-Client-Id" // this is optional
            );
        }

        public override async Task HandleAsync(CreateUserDto req, CancellationToken ct)
        {
            var user = new Core.Entities.User
            {
                Id = Guid.CreateVersion7(),
                FirstName = req.FirstName,
                LastName = req.LastName,
                Email = req.Email,
                PhoneNumber = req.PhoneNumber,
                DateOfBirth = req.DateOfBirth,
                NIN = req.NIN,
                MiddleName = req.MiddleName
            };

            await userRepository.AddAsync(user);

            return;
        }
    }
}
