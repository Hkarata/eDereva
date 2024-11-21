using eDereva.Core.Contracts.Requests;
using eDereva.Core.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace eDereva.Api.Endpoints.Venue;

public class CreateVenueEndpoint(IVenueRepository venueRepository, ILogger<CreateVenueEndpoint> logger)
    : Endpoint<VenueDto, Results<Created, BadRequest>>
{
    public override void Configure()
    {
        Post("/venues");
        Version(1);
        Policies("RequireManageVenues", "RequireAdministrator");
        Description(options =>
        {
            options.WithTags("Venue")
                .WithSummary("Creates a new venue")
                .WithDescription(
                    "This endpoint allows users to create a new venue by providing its name and other optional details such as the venue's location, capacity, and facilities. The venue name is required, while other details are optional and can be provided based on the user's needs. Upon successful creation, the new venue will be stored in the system, and the user will receive a confirmation response with the venue's unique ID and the details that were submitted. This endpoint is ideal for administrators or venue managers who wish to register a new venue in the system.");
        });
    }

    public override async Task<Results<Created, BadRequest>> ExecuteAsync(VenueDto req, CancellationToken ct)
    {
        var venue = req.MapToVenue();
        var result = await venueRepository.AddVenue(venue, ct);

        if (!result) return TypedResults.BadRequest();
        logger.LogInformation("Created a new venue with id: {venueId}", venue.Id.ToString());
        return TypedResults.Created();
    }
}