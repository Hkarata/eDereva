using eDereva.Core.Contracts.Responses;
using eDereva.Core.Entities;
using eDereva.Core.Interfaces;
using FastEndpoints;

namespace eDereva.Api.Endpoints.VenueManager
{
    public class GetTodaysSessionsEndpoint(IVenueManagerRepository repository) : EndpointWithoutRequest<List<SessionDto>>
    {
        public override void Configure()
        {
            Get("/venue-manager/todays-session/{venueManagerId}/{venueId}");
            AllowAnonymous();
            Version(1);
            Description(options =>
            {
                options.WithTags("Venue Manager");
                options.WithSummary("Get Today's Sessions for a Venue Manager");
                options.WithDescription("Fetches today's sessions for a specific venue managed by the venue manager.");
            });
            Throttle(
                hitLimit: 10,
                durationSeconds: 60,
                headerName: "X-Client-Id"
            );
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var venueManagerId = Route<Guid>("venueManagerId");
            var venueId = Route<Guid>("venueId");

            if (venueManagerId == Guid.Empty || venueId == Guid.Empty)
            {
                await SendNotFoundAsync(ct);
                return;
            }

            // Fetch today's sessions for the given venueManagerId and venueId
            var result = await repository.GetTodaysSessions(venueManagerId, venueId);

            // Map the result to SessionDto using the extension method
            var sessions = result.Select(s => s.MapToSessionDto()).ToList();

            // Send the response
            await SendAsync(sessions, cancellation: ct);
        }
    }
}
