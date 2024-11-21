using eDereva.Core.Contracts.Responses;
using eDereva.Core.Repositories;
using eDereva.Core.ValueObjects;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace eDereva.Api.Endpoints.Question;

public class GetQuestionsEndpoint(IQuestionBankRepository questionBankRepository)
    : Endpoint<PaginationParams, Results<Ok<PaginatedResult<QuestionDto>>, BadRequest<string>>>
{
    public override void Configure()
    {
        Get("/questions/{questionBankId}");
        Version(1);
        Policies("RequireViewQuestionBanks");
        Description(options =>
        {
            options.WithTags("Question")
                .WithSummary("Retrieves a list of questions")
                .WithDescription(
                    "This endpoint allows the retrieval of a list of questions. The request should include pagination details such as page number and page size.");
        });
    }

    public override async Task<Results<Ok<PaginatedResult<QuestionDto>>, BadRequest<string>>> ExecuteAsync(
        PaginationParams req, CancellationToken ct)
    {
        var questionBankId = Route<Guid>("questionBankId");
        var questions = await questionBankRepository.GetAllAsync(questionBankId, req, ct);

        if (questions.TotalCount == 0) return TypedResults.BadRequest("No questions found");

        return TypedResults.Ok(questions);
    }
}