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
    public async ValueTask<QuestionDto?> GetByIdAsync(Guid questionId, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting question by ID: {QuestionId}", questionId);
        
        var question = await context.Questions
            .AsSplitQuery()
            .Include(q => q.Choices)
            .Where(q => !q.IsDeleted && q.Id == questionId)
            .Select(q => new QuestionDto
            {
                Id = q.Id,
                Scenario = q.Scenario,
                ImageUrls = q.ImageUrls,
                QuestionText = q.QuestionText,
                Choices = q.Choices!.Select(c => new ChoiceDto
                {
                    Id = c.Id,
                    Text = c.Text
                }).ToList()
            })
            .FirstOrDefaultAsync(cancellationToken: cancellationToken);
        
        if (question == null)
        {
            logger.LogWarning("Question with ID {QuestionId} not found", questionId);
        }
        
        return question;
    }

    public async ValueTask<PaginatedResult<QuestionDto>> GetAllAsync(Guid questionBankId, PaginationParams paginationParams, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting all questions with pagination: {PaginationParams}", paginationParams);
        
        var query = context.Questions
            .AsNoTracking()
            .AsSplitQuery()
            .Include(q => q.Choices)
            .Where(q => !q.IsDeleted && q.QuestionBankId == questionBankId)
            .Select(q => new QuestionDto
            {
                Id = q.Id,
                Scenario = q.Scenario,
                ImageUrls = q.ImageUrls,
                QuestionText = q.QuestionText,
                Choices = q.Choices!.Select(c => new ChoiceDto
                {
                    Id = c.Id,
                    Text = c.Text
                }).ToList()
            });

        var totalItems = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize)
            .Take(paginationParams.PageSize)
            .ToListAsync(cancellationToken);

        logger.LogInformation("Retrieved {ItemCount} questions out of {TotalItems}", items.Count, totalItems);
        return new PaginatedResult<QuestionDto>(items, totalItems, paginationParams);
    }

    public async Task AddAsync(Question question, CancellationToken cancellationToken)
    {
        logger.LogInformation("Adding new question: {Question}", question);
        context.Questions.Add(question);
        await context.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Question added successfully: {Question}", question);
    }

    public async Task UpdateAsync(Question question, CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating question: {Question}", question);
        context.Questions.Update(question);
        await context.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Question updated successfully: {Question}", question);
    }

    public async Task DeleteAsync(Guid questionId, CancellationToken cancellationToken)
    {
        logger.LogInformation("Deleting question with ID: {QuestionId}", questionId);
        var question = context.Questions.FirstOrDefault(q => q.Id == questionId);
        if (question != null)
        {
            question.IsDeleted = true;
            context.Questions.Update(question);
            logger.LogInformation("Deleted (soft) question with ID: {QuestionId}", questionId);
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
        
        logger.LogInformation("Adding new answer: {Answer}", answer);
        context.Answers.Add(answer);
        await context.SaveChangesAsync(cancellationToken); 
        logger.LogInformation("Answer added successfully: {Answer}", answer);
    }

    public async Task UpdateAnswerAsync(Answer answer, CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating answer: {Answer}", answer);
        context.Answers.Update(answer);
        await context.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Answer updated successfully: {Answer}", answer);
    }

    public async Task AddQuestionBankAsync(QuestionBank questionBank, CancellationToken cancellationToken)
    {
        logger.LogInformation("Adding new question bank: {QuestionBank}", questionBank);
        context.QuestionBanks.Add(questionBank);
        await context.SaveChangesAsync(cancellationToken); 
        logger.LogInformation("Question bank added successfully: {QuestionBank}", questionBank);
    }

    public async Task UpdateQuestionBankAsync(QuestionBank questionBank, CancellationToken cancellationToken)
    {
        logger.LogInformation("Updating question bank: {QuestionBank}", questionBank);
        context.QuestionBanks.Update(questionBank);
        await context.SaveChangesAsync(cancellationToken); 
        logger.LogInformation("Question bank updated successfully: {QuestionBank}", questionBank);
    }

    public async Task<QuestionBankDto?> GetQuestionBankByIdAsync(Guid questionBankId, CancellationToken cancellationToken)
    {
        logger.LogInformation("Getting question bank by ID: {QuestionBankId}", questionBankId);
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
        logger.LogInformation("Getting all question banks with pagination: {PaginationParams}", paginationParams);
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

        logger.LogInformation("Retrieved {ItemCount} question banks out of {TotalItems}", items.Count, totalItems);
        return new PaginatedResult<QuestionBankDto>(items, totalItems, paginationParams);
    }
}