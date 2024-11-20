using eDereva.Infrastructure.Jobs;
using FastEndpoints;
using Hangfire;

namespace eDereva.Api.Endpoints.Jobs;

public class StartSessionSchedulerEndpoint : EndpointWithoutRequest
{
    public override void Configure()
    {
        Get("/jobs/start-session-scheduler");
        Policies("RequireAdministrator");
        Description(options =>
        {
            options.WithTags("Jobs")
                .WithSummary("Start Session Scheduler")
                .WithDescription("Start Session Scheduler");
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        RecurringJob.AddOrUpdate<SessionCreationJob>(
            "Session Scheduler",
            x => x.RunJobAsync(
                DateTime.Now.Date.AddDays((1 - (int)DateTime.Now.DayOfWeek) % 7 + 1), // Remove the +7
                DateTime.Now.Date.AddDays((1 - (int)DateTime.Now.DayOfWeek) % 7 + 6),
                ct),
            "0 3 * * 0"
        );
        
        await SendOkAsync(ct);
    }
}