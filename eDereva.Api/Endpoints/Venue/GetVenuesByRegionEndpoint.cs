using eDereva.Core.Contracts.Responses;
using eDereva.Core.Repositories;
using eDereva.Core.ValueObjects;
using FastEndpoints;
using Microsoft.Extensions.Caching.Hybrid;

namespace eDereva.Api.Endpoints.Venue;

public class GetVenuesByRegionEndpoint(IVenueRepository venueRepository, HybridCache hybridCache)
    : Endpoint<PaginationParams, PaginatedResult<VenueDto>>
{
    public override void Configure()
    {
        Post("/region/{regionId}/venues");
        Version(1);
        Policies("RequireViewVenues");
        Description(options =>
        {
            options.WithTags("Venue")
                .WithSummary("Retrieves a list of venues for the specified region")
                .WithDescription(
                    "This endpoint allows users to retrieve a paginated list of venues from the system. It accepts pagination parameters, such as `page` and `pageSize`, to control the number of venues returned in each request. The response includes a list of venue data, with details such as venue name, location, and available capacity. This endpoint is ideal for users who wish to browse or search for venues, especially in cases where there are many venues in the system. It is designed to support efficient pagination for large datasets.");
        });
    }

    public override async Task HandleAsync(PaginationParams req, CancellationToken ct)
    {
        var regionId = Route<Guid>("regionId");

        var cacheKey = $"venues-{regionId}-{req.PageNumber}-{req.PageSize}";

        var venues = await hybridCache.GetOrCreateAsync(cacheKey, async _
            => await venueRepository.GetVenuesByRegionPaginated(regionId, req, ct), cancellationToken: ct);

        if (venues.TotalCount == 0)
            await SendNoContentAsync(ct);

        await SendOkAsync(venues, ct);
    }
}