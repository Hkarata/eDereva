using eDereva.Core.Entities;
using eDereva.Core.ValueObjects;

namespace eDereva.Core.Interfaces
{
    public interface IVenueRepository : IGenericRepository<Venue>
    {
        Task<PaginatedResult<Venue>> GetVenuesByDistrictIdAsync(int districtId, PaginationParams pagination);
        Task<PaginatedResult<Venue>> GetVenuesWithSessionsAsync(PaginationParams pagination);
        Task<PaginatedResult<Venue>> GetAvailableVenuesAsync(DateTime startTime, DateTime endTime, PaginationParams pagination);
        Task<IEnumerable<Venue>> GetAllVenuesLightAsync(); // For dropdowns/lists without related data
    }
}
