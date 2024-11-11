using eDereva.Core.Contracts.Responses;
using eDereva.Core.Entities;
using eDereva.Core.ValueObjects;

namespace eDereva.Core.Interfaces
{
    public interface IVenueRepository : IGenericRepository<Venue>
    {
        Task<PaginatedResult<VenueDto>> GetVenuesByDistrictIdAsync(Guid districtId, PaginationParams pagination);
        Task<PaginatedResult<VenueDto>> GetVenuesWithSessionsAsync(PaginationParams pagination);
        Task<PaginatedResult<VenueDto>> GetAvailableVenuesAsync(DateTime startTime, DateTime endTime, PaginationParams pagination);
        Task<PaginatedResult<VenueDto>> GetAllVenuesAsync(PaginationParams pagination);
    }
}
