using eDereva.Core.Contracts.Responses;
using eDereva.Core.Interfaces;
using eDereva.Core.ValueObjects;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Caching.Hybrid;

namespace eDereva.Api.Endpoints.Venue
{
    public class GetVenuesByDistrictEndpoint(IVenueRepository venueRepository, HybridCache hybridCache)
            : Endpoint<PaginationParams, Results<Ok<PaginatedResult<VenueDto>>, BadRequest>>
    {
        public override void Configure()
        {
            Get("venues/district/{districtId}");
            AllowAnonymous();
            Version(1);
            Description(options =>
            {
                options.WithTags("Venue")
                        .WithSummary("Get venues by district")
                        .WithDescription("Get venues by district");
            });
            Throttle(
                hitLimit: 5,
                durationSeconds: 60,
                headerName: "X-Client-Id" // this is optional
            );
        }

        public override async Task<Results<Ok<PaginatedResult<VenueDto>>, BadRequest>> ExecuteAsync(PaginationParams req, CancellationToken ct)
        {
            var districtId = Route<Guid>("districtId");

            var cacheKey = $"venues_by_district_{req.PageNumber}_{req.PageSize}";

            var venues = await hybridCache.GetOrCreateAsync(cacheKey, async (cancellationToken) =>
            {
                return await venueRepository.GetVenuesByDistrictIdAsync(districtId, req);
            }, cancellationToken: ct);

            if (venues != null)
            {
                return TypedResults.Ok(venues);
            }
            else
            {
                return TypedResults.BadRequest();
            }
        }
    }
}
