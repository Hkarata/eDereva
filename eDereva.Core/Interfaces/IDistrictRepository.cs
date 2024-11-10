using eDereva.Core.Entities;
using eDereva.Core.ValueObjects;

namespace eDereva.Core.Interfaces
{
    public interface IDistrictRepository : IGenericRepository<District>
    {
        Task<PaginatedResult<District>> GetDistrictsByRegionIdAsync(Guid regionId, PaginationParams pagination);
        Task<PaginatedResult<District>> GetDistrictsWithVenuesAsync(PaginationParams pagination);
        Task<IEnumerable<District>> GetAllDistrictsLightAsync(); // For dropdowns/lists without related data
    }
}
