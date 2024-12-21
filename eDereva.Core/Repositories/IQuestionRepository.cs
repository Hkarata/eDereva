using eDereva.Core.Entities;

namespace eDereva.Core.Repositories;

public interface IQuestionRepository
{
    Task<Question?> GetByIdAsync(Guid id, CancellationToken cancellationToken);
    Task<IEnumerable<Question>> GetBySectionTemplateIdAsync(Guid sectionTemplateId, CancellationToken cancellationToken);
    Task AddAsync(Question question, CancellationToken cancellationToken);
    Task UpdateAsync(Question question, CancellationToken cancellationToken);
    Task DeleteAsync(Guid id, CancellationToken cancellationToken);
}