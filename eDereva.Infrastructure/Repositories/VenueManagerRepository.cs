using eDereva.Core.Entities;
using eDereva.Core.Enums;
using eDereva.Core.Interfaces;
using eDereva.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace eDereva.Infrastructure.Repositories
{
    public class VenueManagerRepository(ApplicationDbContext context) : IVenueManagerRepository
    {
        public async Task<IEnumerable<Session>> GetTodaysSessions(Guid venueManagerId, Guid venueId)
        {
            var today = DateTime.UtcNow.Date;

            var venueManager = await context.VenueManagers
                .Include(vm => vm.Venues)
                .FirstOrDefaultAsync(vm => vm.Id == venueManagerId);

            if (venueManager == null || !venueManager.Venues!.Any(v => v.Id == venueId))
            {
                throw new InvalidOperationException("Venue manager does not manage this venue.");
            }

            return await context.Sessions
                .Where(s => s.StartTime.Date == today
                            && s.EndTime.Date == today
                            && !s.IsDeleted
                            && s.VenueId == venueId)
                .ToListAsync();
        }


        public async Task<bool> InitiateSession(Guid sessionId)
        {
            var session = await context.Sessions.FindAsync(sessionId);

            if (session == null) return false;

            if (session.Status != SessionStatus.Active || session.InitiationTime == null)
            {
                session.Status = SessionStatus.Active;
                session.InitiationTime = DateTime.UtcNow;
                session.Contingency = ContingencyType.None;

                await context.SaveChangesAsync();
            }

            return true;
        }

        public async Task<bool> ReportContingency(Guid sessionId, ContingencyType type, DateTime contingencyTime, string description)
        {
            var session = await context.Sessions.FindAsync(sessionId);

            if (session == null || session.IsDeleted)
            {
                return false; // Session not found or has been deleted
            }

            // Update contingency information
            session.Contingency = type;
            session.ContingencyTime = contingencyTime;

            // Set the contingency explanation if the type is "Other"
            if (type == ContingencyType.Other)
            {
                if (string.IsNullOrWhiteSpace(description))
                {
                    throw new ArgumentException("Description must be provided for 'Other' contingency type.", nameof(description));
                }
                session.ContingencyExplanation = description;
            }
            else
            {
                session.ContingencyExplanation = null; // Clear any previous explanation if not needed
            }

            await context.SaveChangesAsync();
            return true;
        }
    }
}
