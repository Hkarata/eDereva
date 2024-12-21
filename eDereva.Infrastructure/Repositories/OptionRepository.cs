using eDereva.Core.Entities;
using eDereva.Core.Repositories;
using eDereva.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace eDereva.Infrastructure.Repositories;

public class OptionRepository(ApplicationDbContext context) : IOptionRepository
{
    public async Task<Option?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await context.Options
            .Where(o => o.Id == id)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<Option>> GetByQuestionIdAsync(Guid questionId, CancellationToken cancellationToken)
    {
        var choices = await context.Options
            .Where(o => o.QuestionId == questionId)
            .ToListAsync(cancellationToken);

        return choices;
    }

    public async Task AddAsync(Option option, CancellationToken cancellationToken)
    {
        context.Options.Add(option);
        await context.SaveChangesAsync(cancellationToken);
    }

    public Task UpdateAsync(Option option, CancellationToken cancellationToken)
    {
        context.Options.Update(option);
        return context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        context.Options.Remove(new Option { Id = id });
        await context.SaveChangesAsync(cancellationToken);
    }
}