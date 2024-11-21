using eDereva.Core.Contracts.Responses;
using eDereva.Core.Repositories;
using eDereva.Core.ValueObjects;
using FastEndpoints;

namespace eDereva.Api.Endpoints.Venue;

public class GetVenuesEndpoint(IVenueRepository venueRepository)
    : Endpoint<PaginationParams, PaginatedResult<VenueDto>>
{
    public override void Configure()
    {
        Get("/venues");
        Version(1);
        Policies("RequireViewVenues");
        Description(options =>
        {
            options.WithTags("Venue")
                .WithSummary("Retrieves a list of venues")
                .WithDescription(
                    "This endpoint allows users to retrieve a paginated list of venues from the system. It accepts pagination parameters, such as `page` and `pageSize`, to control the number of venues returned in each request. The response includes a list of venue data, with details such as venue name, location, and available capacity. This endpoint is ideal for users who wish to browse or search for venues, especially in cases where there are many venues in the system. It is designed to support efficient pagination for large datasets.");
        });
    }

    public override async Task HandleAsync(PaginationParams req, CancellationToken ct)
    {
        var venues = await venueRepository.GetVenuesPaginated(req, ct);

        if (venues.TotalCount == 0) await SendNoContentAsync(ct);

        await SendOkAsync(venues, ct);
    }
}