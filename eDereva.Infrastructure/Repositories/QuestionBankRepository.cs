using eDereva.Core.Contracts.Responses;
using eDereva.Core.Entities;
using eDereva.Core.Repositories;
using eDereva.Core.ValueObjects;
using eDereva.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace eDereva.Infrastructure.Repositories;

public class QuestionBankRepository(ApplicationDbContext context, ILogger<QuestionBankRepository> logger) 
    : IQuestionBankRepository
{
    public async ValueTask<Question?> GetByIdAsync(Guid questionId, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting question by ID: {QuestionId}", questionId);
        var question = await context.Questions
            .Where(q => !q.IsDeleted && q.Id == questionId)
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);
        
        if (question == null)
        {
            logger.LogWarning("Question with ID {QuestionId} not found", questionId);
        }
        
        return question;
    }

    public async ValueTask<PaginatedResult<Question>> GetAllAsync(Guid questionBankId, PaginationParams paginationParams, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting all questions with pagination: {PaginationParams}", paginationParams);
        
        var query = context.Questions
            .AsNoTracking()
            .AsSplitQuery()
            .Where(q => !q.IsDeleted && q.QuestionBankId == questionBankId);

        if (!string.IsNullOrEmpty(paginationParams.SortBy))
        {
            query = paginationParams.IsDescending
                ? query.OrderByDescending(e => EF.Property<object>(e, paginationParams.SortBy))
                : query.OrderBy(e => EF.Property<object>(e, paginationParams.SortBy));
        }

        var totalItems = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize)
            .Take(paginationParams.PageSize)
            .ToListAsync(cancellationToken);

        logger.LogInformation("Retrieved {ItemCount} questions out of {TotalItems}", items.Count, totalItems);
        return new PaginatedResult<Question>(items, totalItems, paginationParams);
    }

    public async Task AddAsync(Question question, CancellationToken cancellationToken)
    {
        context.Questions.Add(question);
        logger.LogInformation("Adding new question: {Question}", question);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Question question, CancellationToken cancellationToken)
    {
        context.Questions.Update(question);
        logger.LogInformation("Updating question: {Question}", question);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(Guid questionId, CancellationToken cancellationToken)
    {
        var question = context.Questions.FirstOrDefault(q => q.Id == questionId);
        if (question != null)
        {
            question.IsDeleted = true;
            context.Questions.Update(question);
            logger.LogInformation("Deleting (soft) question with ID: {QuestionId}", questionId);
        }
        else
        {
            logger.LogWarning("Attempted to delete non-existent question with ID: {QuestionId}", questionId);
        }
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task AddAnswerAsync(Guid questionId, Guid choiceId, CancellationToken cancellationToken)
    {
        var answer = new Answer
        {
            QuestionId = questionId,
            ChoiceId = choiceId
        };
        
        context.Answers.Add(answer);
        await context.SaveChangesAsync(cancellationToken); 
    }

    public async Task UpdateAnswerAsync(Answer answer, CancellationToken cancellationToken)
    {
        context.Answers.Update(answer);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task AddQuestionBankAsync(QuestionBank questionBank, CancellationToken cancellationToken)
    {
        context.QuestionBanks.Add(questionBank);
        await context.SaveChangesAsync(cancellationToken); 
    }

    public async Task UpdateQuestionBankAsync(QuestionBank questionBank, CancellationToken cancellationToken)
    {
        context.QuestionBanks.Update(questionBank);
        await context.SaveChangesAsync(cancellationToken); 
    }

    public async Task<QuestionBankDto?> GetQuestionBankByIdAsync(Guid questionBankId, CancellationToken cancellationToken)
    {
        var questionBank = await context.QuestionBanks
            .AsNoTracking()
            .AsSplitQuery()
            .Include(qb => qb.Questions)
            .Where(qb => qb.Id == questionBankId)
            .Select(qb => new QuestionBankDto
            {
                Id = qb.Id,
                Name = qb.Name,
                Description = qb.Description,
                QuestionCount = qb.Questions!.Count(q => !q.IsDeleted)
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (questionBank != null)
            return questionBank;
        
        logger.LogWarning("QuestionBank with ID {QuestionBankId} not found", questionBankId);
        return null;

    }

    public async Task<PaginatedResult<QuestionBankDto>> GetQuestionBanks(PaginationParams paginationParams, CancellationToken cancellationToken)
    {
        var query = context.QuestionBanks
            .AsNoTracking()
            .AsSplitQuery()
            .Include(qb => qb.Questions);
    
        var totalItems = await query.CountAsync(cancellationToken);
        
        var items = await query
            .Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize)
            .Take(paginationParams.PageSize)
            .Select(qb => new QuestionBankDto
            {
                Id = qb.Id,
                Name = qb.Name,
                Description = qb.Description,
                QuestionCount = qb.Questions!.Count(q => !q.IsDeleted)
            })
            .ToListAsync(cancellationToken);
        
        return new PaginatedResult<QuestionBankDto>(items, totalItems, paginationParams);
    }
}