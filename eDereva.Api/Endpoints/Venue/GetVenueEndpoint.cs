using eDereva.Core.Contracts.Responses;
using eDereva.Core.Entities;
using eDereva.Core.Interfaces;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace eDereva.Api.Endpoints.Venue;
public class GetVenueEndpoint(IVenueRepository venueRepository)
    : EndpointWithoutRequest<Results<Ok<VenueDto>, BadRequest>>
{
    public override void Configure()
    {
        Get("/venues/{venueId}");
        Version(1);
        Policies("RequireViewVenues");
        Description(options =>
        {
            options.WithTags("Venue")
                .WithSummary("Retrieves a specific venue")
                .WithDescription("Fetches detailed information about a specific venue identified by its unique ID.");
        });
    }

    public override async Task<Results<Ok<VenueDto>, BadRequest>> ExecuteAsync(CancellationToken ct)
    {
        var venueId = Route<Guid>("venueId");

        var venue = await venueRepository.GetVenueById(venueId, ct);

        if (venue is null)
        {
            return TypedResults.BadRequest();
        }
        
        return TypedResults.Ok(venue.MapToVenueDto());
    }
}
