using eDereva.Core.Interfaces;
using FastEndpoints;

namespace eDereva.Api.Endpoints.VenueManager
{
    public class InitiateSessionEndpoint(IVenueManagerRepository venueManagerRepository) : EndpointWithoutRequest
    {
        public override void Configure()
        {
            Get("/venue-manager/initiate-session/{sessionId}");
            AllowAnonymous();
            Version(1);
            Description(options =>
            {
                options.WithTags("Venue Manager");
                options.WithSummary("Initiate a session");
                options.WithDescription("Initiate the start of a session");
            });
            Throttle(
                hitLimit: 10,
                durationSeconds: 60,
                headerName: "X-Client-Id"
            );
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var sessionId = Route<Guid>("sessionId");

            if (sessionId == Guid.Empty)
            {
                await SendNotFoundAsync(ct);
                return;
            }

            var success = await venueManagerRepository.InitiateSession(sessionId);

            if (success)
            {
                await SendOkAsync(ct);
            }
            else
            {
                await SendErrorsAsync(StatusCodes.Status400BadRequest, ct);
            }
        }
    }
}