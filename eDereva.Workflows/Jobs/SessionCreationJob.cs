using eDereva.Core.Entities;
using eDereva.Core.Enums;
using eDereva.Core.Interfaces;
using eDereva.Core.Jobs;
using eDereva.Core.Services;
using eDereva.Workflows.Services;
using Microsoft.Extensions.Logging;

namespace eDereva.Workflows.Jobs
{
    public class SessionCreationJob(
        IVenueExemptionService venueExemptionService,
        IVenueRepository venueRepository,
        IPublicHolidayService publicHolidayService,
        ISessionRepository sessionRepository,
        ILogger<SessionCreationJob> logger
    ) : ISessionCreationJob
    {
        private readonly ILogger<SessionCreationJob> _logger = logger;

        public async Task RunJobAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting RunJobAsync from {StartDate} to {EndDate}", startDate, endDate);

            var venueExemptions = await venueExemptionService
                .GetExemptedVenuesInDateRange(startDate, endDate, cancellationToken);
            _logger.LogInformation("Fetched venue exemptions: {VenueExemptions}", venueExemptions);

            var venueIds = await venueRepository
                .GetAllVenuesIds(cancellationToken);
            _logger.LogInformation("Fetched venue IDs: {VenueIds}", venueIds);

            // Remove the exempted venue IDs from the venueIds list
            var availableVenueIds = venueIds.Except(venueExemptions).ToList();
            _logger.LogInformation("Available venue IDs after exemptions: {AvailableVenueIds}", availableVenueIds);

            // Get public holidays between the specified start and end dates
            var publicHolidays = publicHolidayService.GetPublicHolidays(startDate, endDate);
            _logger.LogInformation("Public holidays between {StartDate} and {EndDate}: {PublicHolidays}", startDate, endDate, publicHolidays);

            // Get all dates within the specified range
            var validDates = Enumerable.Range(0, (endDate - startDate).Days + 1)
                .Select(offset => startDate.AddDays(offset))
                .Where(date =>
                    date.DayOfWeek != DayOfWeek.Sunday &&
                    publicHolidays.All(publicHoliday => publicHoliday.Date != date))
                .Select(date => date)
                .ToList();
            _logger.LogInformation("Valid dates for sessions: {ValidDates}", validDates);

            await ProcessVenuesSequentiallyAsync(
                availableVenueIds,
                validDates,
                cancellationToken
                );
        }

        private async Task ProcessVenuesSequentiallyAsync(
            List<Guid> venueIds,
            List<DateTime> validDates,
            CancellationToken cancellationToken)
        {
            foreach (var venueId in venueIds)
            {
                foreach (var date in validDates)
                {
                    if (cancellationToken.IsCancellationRequested) return;

                    _logger.LogInformation("Processing VenueId {VenueId} for Date {Date}", venueId, date);
                    await ProcessVenueDatePairAsync((venueId, date), cancellationToken);
                }
            }
        }

        private async Task ProcessVenueDatePairAsync((Guid VenueId, DateTime Date) pair,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Checking sessions for VenueId {VenueId} on Date {Date}", pair.VenueId, pair.Date);
                var existingSessions = await sessionRepository
                    .CheckSessionsByVenueAndDate(pair.VenueId, pair.Date, cancellationToken);

                if (existingSessions)
                {
                    _logger.LogInformation("Sessions already exist for VenueId {VenueId} on Date {Date}", pair.VenueId, pair.Date);
                    // Sessions already exist for this venue and date
                    return;
                }

                // Check if the venue is exempted for the specific date
                var isExempted = await venueExemptionService.IsVenueExemptedForDate(pair.VenueId, pair.Date, cancellationToken);
                if (isExempted)
                {
                    _logger.LogInformation("VenueId {VenueId} is exempted on Date {Date}", pair.VenueId, pair.Date);
                    return;
                }

                // Generate session slots and create sessions
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
                        Status = SessionStatus.Scheduled,
                    };

                    // Create sessions for the specific date and venue
                    await sessionRepository.CreateAsync(session, cancellationToken);
                    _logger.LogInformation("Created session {SessionId} for VenueId {VenueId} on Date {Date}", session.Id, pair.VenueId, pair.Date);
                }
            }
            catch (Exception ex) when (ex is not OperationCanceledException)
            {
                // Log error and continue
                _logger.LogError(ex, "Error processing VenueId {VenueId} for {Date}", pair.VenueId, pair.Date);
            }
        }

        private static IEnumerable<(TimeOnly StartTime, TimeOnly EndTime)> GenerateSessionSlots()
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
}
