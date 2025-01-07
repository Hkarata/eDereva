using eDereva.Core.Contracts.Responses;
using eDereva.Core.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Caching.Hybrid;

namespace eDereva.Api.Endpoints.User;

public class GetDashboardData (IUserRepository userRepository, HybridCache hybridCache)
    : EndpointWithoutRequest<Results<Ok<DashboardDto>, NoContent>>
{
    public override void Configure()
    {
        Get("/user/{nin}/dashboard-data");
        Version(1);
        Policies("RequireViewUser");
        Description(options => 
            options.WithTags("Dashboard")
                .WithSummary("Dashboard Data")
        );
        Throttle(
            10,
            60,
            "X-Client-Id" // this is optional
        );
    }

    public override async Task<Results<Ok<DashboardDto>, NoContent>> ExecuteAsync(CancellationToken ct)
    {
        var nin = Route<string>("nin");
        
        var cacheKey = $"{nin}-dashboard-data";

        var data = await hybridCache.GetOrCreateAsync(cacheKey,
            async (entry) =>
            {
                if (nin != null) return await userRepository.GetDashboardDataAsync(nin, ct);
                return null;
            }, cancellationToken: ct);

        if (data is null)
        {
            return TypedResults.NoContent();
        }

        return TypedResults.Ok(data);
    }
}