using eDereva.Core.Entities;
using eDereva.Core.Repositories;
using eDereva.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace eDereva.Infrastructure.Repositories;

public class QuestionRepository (ApplicationDbContext context) : IQuestionRepository
{
    public async Task<Question?> GetByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await context.Questions
            .Where(q => q.Id == id)
            .Include(q => q.Options)
            .SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<IEnumerable<Question>> GetBySectionTemplateIdAsync(Guid sectionTemplateId, CancellationToken cancellationToken)
    {
        var questions = await context.Questions
            .Where(q => q.SectionTemplateId == sectionTemplateId)
            .Include(q => q.Options)
            .ToListAsync(cancellationToken);
        
        return questions;
    }

    public async Task AddAsync(Question question, CancellationToken cancellationToken)
    {
        context.Questions.Add(question);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Question question, CancellationToken cancellationToken)
    {
        context.Questions.Update(question);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken)
    {
        context.Questions.Remove(new Question {Id = id});
        await context.SaveChangesAsync(cancellationToken);
    }
}