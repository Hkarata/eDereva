using eDereva.Core.Entities;
using eDereva.Core.Enums;
using eDereva.Core.Jobs;
using eDereva.Core.Repositories;
using eDereva.Core.Services;
using eDereva.Infrastructure.Data;
using eDereva.Infrastructure.Services;
using Microsoft.Extensions.Logging;

namespace eDereva.Infrastructure.Jobs;

public class SessionCreationJob(
    IVenueExemptionService venueExemptionService,
    IVenueRepository venueRepository,
    IPublicHolidayService publicHolidayService,
    ILogger<SessionCreationJob> logger,
    ApplicationDbContext context // Injecting DbContext
) : ISessionCreationJob
{
    public async Task RunJobAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken)
    {
        logger.LogInformation("Starting RunJobAsync from {StartDate} to {EndDate}", startDate, endDate);

        var venueExemptions = await venueExemptionService
            .GetExemptedVenuesInDateRange(startDate, endDate, cancellationToken);
        logger.LogInformation("Fetched venue exemptions: {VenueExemptions}", venueExemptions);

        var venueIds = await venueRepository
            .GetAllVenuesIds(cancellationToken);
        logger.LogInformation("Fetched venue IDs: {VenueIds}", venueIds);

        // Remove the exempted venue IDs from the venueIds list
        var availableVenueIds = venueIds.Except(venueExemptions).ToList();
        logger.LogInformation("Available venue IDs after exemptions: {AvailableVenueIds}", availableVenueIds);

        // Get public holidays between the specified start and end dates
        var publicHolidays = publicHolidayService.GetPublicHolidays(startDate, endDate);
        logger.LogInformation("Public holidays between {StartDate} and {EndDate}: {PublicHolidays}", startDate,
            endDate, publicHolidays);

        // Get all dates within the specified range
        var validDates = Enumerable.Range(0, (endDate - startDate).Days + 1)
            .Select(offset => startDate.AddDays(offset))
            .Where(date =>
                date.DayOfWeek != DayOfWeek.Sunday &&
                publicHolidays.All(publicHoliday => publicHoliday.Date != date))
            .Select(date => date)
            .ToList();
        logger.LogInformation("Valid dates for sessions: {ValidDates}", validDates);

        await ProcessVenuesInParallelAsync(
            availableVenueIds,
            validDates,
            cancellationToken
        );
    }

    private async Task ProcessVenuesInParallelAsync(
        List<Guid> venueIds,
        List<DateTime> validDates,
        CancellationToken cancellationToken)
    {
        var tasks = new List<Task<(Guid, DateTime, List<Session>)>>();

        foreach (var venueId in venueIds)
        foreach (var date in validDates)
        {
            if (cancellationToken.IsCancellationRequested) return;

            tasks.Add(ProcessVenueDatePairAsync((venueId, date)));
        }

        // Wait for all tasks to complete and collect results
        var results = await Task.WhenAll(tasks);

        // Insert all sessions and save changes to the database
        var sessions = results.SelectMany(result => result.Item3).ToList();

        await context.BulkInsertAsync(sessions, cancellationToken);
    }

    private Task<(Guid VenueId, DateTime Date, List<Session> sessionList)> ProcessVenueDatePairAsync(
        (Guid VenueId, DateTime Date) pair)
    {
        var sessionList = new List<Session>();

        var sessionSlots = GenerateSessionSlots();

        foreach (var slot in sessionSlots)
        {
            var session = new Session
            {
                Id = Guid.NewGuid(),
                Date = pair.Date,
                VenueId = pair.VenueId,
                StartTime = slot.StartTime,
                EndTime = slot.EndTime,
                Status = SessionStatus.Scheduled
            };

            sessionList.Add(session);

            logger.LogInformation("Prepared session {SessionId} for VenueId {VenueId} on Date {Date}", session.Id,
                pair.VenueId, pair.Date);
        }

        return Task.FromResult((pair.VenueId, pair.Date, sessionList));
    }

    private IEnumerable<(TimeOnly StartTime, TimeOnly EndTime)> GenerateSessionSlots()
    {
        var startTime = new TimeOnly(8, 0);
        var endTime = new TimeOnly(20, 0);
        var sessionDuration = TimeSpan.FromHours(2); // Example: 2-hour session
        var restDuration = TimeSpan.FromMinutes(30);

        var slots = new List<(TimeOnly StartTime, TimeOnly EndTime)>();

        while (startTime.Add(sessionDuration) <= endTime)
        {
            var sessionEndTime = startTime.Add(sessionDuration);
            slots.Add((startTime, sessionEndTime));
            startTime = sessionEndTime.Add(restDuration);
        }

        return slots;
    }
}