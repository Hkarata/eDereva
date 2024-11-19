using eDereva.Core.Jobs;
using FastEndpoints;
using Hangfire;

namespace eDereva.Api.Endpoints;

public class TestSchedulerEndpoint(IBackgroundJobClientV2 jobClientV2,
    ISessionCreationJob sessionCreationJob) : EndpointWithoutRequest
{
    public override void Configure()
    {
        Get("/test-scheduler");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        jobClientV2.Enqueue(() =>
            sessionCreationJob.RunJobAsync(DateTime.Now, DateTime.Now.AddDays(5), ct)
        );
        
        await SendOkAsync(ct);
    }
}