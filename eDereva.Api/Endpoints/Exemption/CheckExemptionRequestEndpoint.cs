using eDereva.Core.Services;
using FastEndpoints;

namespace eDereva.Api.Endpoints.Exemption;

public class CheckExemptionRequestEndpoint(
    IVenueExemptionService venueExemptionService,
    ILogger<CheckExemptionRequestEndpoint> logger) : EndpointWithoutRequest
{
    public override void Configure()
    {
        Get("/venue/{venueId}/exemption-date/{date}");
        Version(1);
        Policies("RequireEditVenues");
        Description(options =>
        {
            options.WithTags("Exemption")
                .WithSummary("Check if a venue is exempt for a date")
                .WithDescription("Check if a venue is exempt for a date");
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var venueId = Route<Guid>("venueId");
        var date = Route<DateTime>("date");

        logger.LogInformation("Checking exemption for VenueId: {VenueId} on Date: {Date}", venueId, date);

        var result = await venueExemptionService.IsVenueExemptedForDate(venueId, date, ct);

        logger.LogInformation("Exemption check result for VenueId: {VenueId} on Date: {Date} is: {Result}", venueId,
            date, result);

        await SendOkAsync(result, ct);
    }
}