using eDereva.Core.Contracts.Responses;
using eDereva.Core.Entities;
using eDereva.Core.ValueObjects;

namespace eDereva.Core.Repositories;

public interface IQuestionBankRepository
{
    ValueTask<QuestionDto?> GetByIdAsync(Guid questionId, CancellationToken cancellationToken);
    ValueTask<PaginatedResult<QuestionDto>> GetAllAsync(Guid questionBankId, PaginationParams paginationParams, CancellationToken cancellationToken);
    Task AddAsync(Question question, CancellationToken cancellationToken);
    Task UpdateAsync(Question question, CancellationToken cancellationToken);
    Task DeleteAsync(Guid questionId, CancellationToken cancellationToken);
    Task AddAnswerAsync(Guid questionId, Guid choiceId, CancellationToken cancellationToken);
    Task UpdateAnswerAsync(Answer answer, CancellationToken cancellationToken);
    Task AddQuestionBankAsync(QuestionBank questionBank, CancellationToken cancellationToken);
    Task UpdateQuestionBankAsync(QuestionBank questionBank, CancellationToken cancellationToken);
    Task<QuestionBankDto?> GetQuestionBankByIdAsync(Guid questionBankId, CancellationToken cancellationToken);
    Task<PaginatedResult<QuestionBankDto>> GetQuestionBanks(PaginationParams paginationParams, CancellationToken cancellationToken);
}