using eDereva.Api.Exceptions;
using eDereva.Core.Contracts.Responses;
using eDereva.Core.Interfaces;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace eDereva.Api.Endpoints.User
{
    public class GetUsersEndpoint(IUserRepository userRepository) : EndpointWithoutRequest<Results<Ok<List<UserData>>, BadRequest>>
    {
        public override void Configure()
        {
            Get("/api/users");
            Version(1);
            AllowAnonymous();
            Description(options =>
            {
                options.WithTags("User")
                        .WithSummary("Get all users")
                        .WithDescription("Retrieve all users");
            });
            Throttle(
                hitLimit: 20,
                durationSeconds: 60,
                headerName: "X-Client-Id" // this is optional
            );
        }
        public override async Task<Results<Ok<List<UserData>>, BadRequest>> ExecuteAsync(CancellationToken ct)
        {
            var users = await userRepository.GetAllAsync();
            return users == null
                ? throw new ProblemException("No users found", "No users were found")
                : (Results<Ok<List<UserData>>, BadRequest>)TypedResults.Ok(users.Select(user => new UserData
                {
                    Id = user.Id,
                    NIN = user.NIN,
                    FirstName = user.FirstName,
                    MiddleName = user.MiddleName,
                    LastName = user.LastName,
                    Sex = user.Sex.ToString(),
                    Age = DateTime.Now.Year - user.DateOfBirth.Year,
                    PhoneNumber = user.PhoneNumber,
                    Email = user.Email!
                }).ToList());
        }
    }
}
