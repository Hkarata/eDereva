using eDereva.Core.Repositories;
using FastEndpoints;

namespace eDereva.Api.Endpoints.Session;

public class DeleteSessionEndpoint(ISessionRepository sessionRepository)
    : EndpointWithoutRequest
{
    public override void Configure()
    {
        Delete("/sessions/{sessionId}");
        Version(1);
        Policies("RequireDeleteSessions");
        Description(options =>
        {
            options.WithTags("Session")
                .WithSummary("Delete session by Id")
                .WithDescription("Delete session by Id");
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var sessionId = Route<Guid>("sessionId");

        await sessionRepository.SoftDeleteAsync(sessionId, ct);

        await SendOkAsync(ct);
    }
}