using eDereva.Core.Entities;
using eDereva.Core.Repositories;
using eDereva.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace eDereva.Infrastructure.Repositories;

public class SectionTemplateRepository(ApplicationDbContext context) : ISectionTemplateRepository
{
    public async Task<SectionTemplate?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await context.SectionTemplates
            .Where(st => st.Id == id)
            .Include(st => st.Questions)!
            .ThenInclude(q => q.Options)
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<SectionTemplate>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await context.SectionTemplates
            .Include(st => st.Questions)
            .ToListAsync(cancellationToken);
    }

    public async Task AddAsync(SectionTemplate sectionTemplate, CancellationToken cancellationToken)
    {
        context.SectionTemplates.Add(sectionTemplate);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(SectionTemplate sectionTemplate, CancellationToken cancellationToken)
    {
        context.SectionTemplates.Update(sectionTemplate);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        context.SectionTemplates.Remove(new SectionTemplate { Id = id });
        await context.SaveChangesAsync(cancellationToken);
    }
}