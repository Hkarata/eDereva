using eDereva.Core.Contracts.Requests;
using eDereva.Core.Entities;
using eDereva.Core.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using VenueDto = eDereva.Core.Contracts.Responses.VenueDto;

namespace eDereva.Api.Endpoints.Venue;

public class UpdateVenueEndpoint(IVenueRepository venueRepository)
    : Endpoint<Core.Contracts.Requests.VenueDto, Results<Ok<VenueDto>, BadRequest>>
{
    public override void Configure()
    {
        Put("/venues/{venueId}");
        Version(1);
        Policies("RequireEditVenues");
        Description(options =>
        {
            options.WithTags("Venue")
                .WithSummary("Updates a specific venue")
                .WithDescription("Updates the details of an existing venue identified by its unique ID.");
        });
    }

    public override async Task<Results<Ok<VenueDto>, BadRequest>> ExecuteAsync(Core.Contracts.Requests.VenueDto req,
        CancellationToken ct)
    {
        var venueId = Route<Guid>("venueId");

        var updatedVenue = await venueRepository.UpdateVenue(venueId, req.MapToVenue(), ct);

        return TypedResults.Ok(updatedVenue.MapToVenueDto());
    }
}