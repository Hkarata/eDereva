using eDereva.Core.Contracts.Requests;
using eDereva.Core.ValueObjects;

namespace eDereva.Core.Services;

public interface IVenueExemptionService
{
    Task Exempt(ExemptVenueDto venue, CancellationToken cancellationToken);
    Task ApproveExemption(Guid venueId, CancellationToken cancellationToken);

    Task<List<Guid>> GetExemptedVenuesInDateRange(DateTime startDate, DateTime endDate,
        CancellationToken cancellationToken);

    Task<PaginatedResult<Contracts.Responses.ExemptVenueDto>> GetNewExemptionRequests(PaginationParams paginationParams,
        CancellationToken cancellationToken);

    Task<bool> IsVenueExemptedForDate(Guid pairVenueId, DateTime pairDate, CancellationToken cancellationToken);
}