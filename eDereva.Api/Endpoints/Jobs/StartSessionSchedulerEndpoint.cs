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
                DateTime.Now.Date.AddDays(-((int)DateTime.Now.DayOfWeek) + 1), // Current week's Monday
                DateTime.Now.Date.AddDays(-((int)DateTime.Now.DayOfWeek) + 6), // Current week's Saturday
                ct),
            "0 3 * * 0"
        );

        await SendOkAsync(ct);
    }
}