using eDereva.Core.Entities;

namespace eDereva.Core.Repositories;

public interface ITestSectionRepository
{
    Task<TestSection?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IEnumerable<TestSection>> GetByTestIdAsync(Guid testId, CancellationToken cancellationToken);
    Task AddAsync(TestSection testSection, CancellationToken cancellationToken);
    Task UpdateAsync(TestSection testSection, CancellationToken cancellationToken);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}