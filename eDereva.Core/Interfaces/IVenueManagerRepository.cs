using eDereva.Core.Entities;
using eDereva.Core.Enums;

namespace eDereva.Core.Interfaces
{
    public interface IVenueManagerRepository
    {
        Task<bool> InitiateSession(Guid sessionId);
        Task<bool> ReportContingency(Guid sessionId, ContingencyType type, DateTime ContingencyTime, string description);
        Task<IEnumerable<Session>> GetTodaysSessions(Guid venueManagerId, Guid venueId);
    }
}
