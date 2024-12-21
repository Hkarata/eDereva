using eDereva.Core.Entities;
using eDereva.Core.Repositories;
using eDereva.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace eDereva.Infrastructure.Repositories;

public class TestSectionRepository(ApplicationDbContext context) : ITestSectionRepository
{
    public async Task<TestSection?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await context.TestSections
            .Where(ts => ts.Id == id)
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<TestSection>> GetByTestIdAsync(Guid testId, CancellationToken cancellationToken)
    {
        return await context.TestSections
            .AsNoTracking()
            .Where(ts => ts.TestId == testId)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(TestSection testSection, CancellationToken cancellationToken)
    {
        context.TestSections.Add(testSection);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(TestSection testSection, CancellationToken cancellationToken)
    {
        context.TestSections.Update(testSection);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        context.TestSections.Remove(new TestSection { Id = id });
        await context.SaveChangesAsync(cancellationToken);
    }
}