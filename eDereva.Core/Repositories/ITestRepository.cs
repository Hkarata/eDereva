using eDereva.Core.Entities;

namespace eDereva.Core.Repositories;

public interface ITestRepository
{
    Task<Test?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IEnumerable<Test>> GetAllAsync(CancellationToken cancellationToken);
    Task AddAsync(Test test, CancellationToken cancellationToken);
    Task UpdateAsync(Test test, CancellationToken cancellationToken);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}