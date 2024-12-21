using eDereva.Core.Entities;
using eDereva.Core.Repositories;
using eDereva.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace eDereva.Infrastructure.Repositories;

public class TestRepository (ApplicationDbContext context) : ITestRepository
{
    public async Task<Test?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await context.Tests
            .Where(t => t.Id == id)
            .Include(t => t.TestSections)
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<Test>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await context.Tests
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(Test test, CancellationToken cancellationToken)
    {
        context.Tests.Add(test);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Test test, CancellationToken cancellationToken)
    {
        context.Tests.Update(test);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        context.Tests.Remove(new Test {Id = id});
        await context.SaveChangesAsync(cancellationToken);
    }
}