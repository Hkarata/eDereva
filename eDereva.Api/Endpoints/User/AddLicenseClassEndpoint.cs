using eDereva.Core.Contracts.Requests;
using eDereva.Core.Repositories;
using FastEndpoints;

namespace eDereva.Api.Endpoints.User;

public class AddLicenseClassEndpoint (IUserRepository userRepository) : Endpoint<LicenseClassRequest>
{
    public override void Configure()
    {
        Post("/user/{nin}/licenses");
        Version(1);
        Policies("RequireEditUsers");
        Description(options =>
        {
            options.WithTags("User");
            options.WithSummary("Add User License Class");
        });
    }

    public override async Task HandleAsync(LicenseClassRequest req, CancellationToken ct)
    {
        var userNin = Route<string>("nin");

        await userRepository.AddLicenseClasses(userNin!, req, ct);
        
        await SendOkAsync(ct);
    }
}