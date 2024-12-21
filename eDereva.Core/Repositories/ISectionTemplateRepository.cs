using eDereva.Core.Entities;

namespace eDereva.Core.Repositories;

public interface ISectionTemplateRepository
{
    Task<SectionTemplate?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IEnumerable<SectionTemplate>> GetAllAsync(CancellationToken cancellationToken);
    Task AddAsync(SectionTemplate sectionTemplate, CancellationToken cancellationToken);
    Task UpdateAsync(SectionTemplate sectionTemplate, CancellationToken cancellationToken);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}