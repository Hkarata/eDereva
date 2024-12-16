using eDereva.Core.Contracts.Responses;

namespace eDereva.Core.Repositories;

public interface ILocaleRepository
{
    ValueTask<List<RegionDto>?> GetAllRegions(CancellationToken cancellationToken);
    ValueTask<List<DistrictDto>> GetAllDistricts(Guid? regionId, CancellationToken cancellationToken);
}