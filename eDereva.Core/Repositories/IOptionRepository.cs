using eDereva.Core.Entities;

namespace eDereva.Core.Repositories;

public interface IOptionRepository
{
    Task<Option?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IEnumerable<Option>> GetByQuestionIdAsync(Guid questionId, CancellationToken cancellationToken);
    Task AddAsync(Option option, CancellationToken cancellationToken);
    Task UpdateAsync(Option option, CancellationToken cancellationToken);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}