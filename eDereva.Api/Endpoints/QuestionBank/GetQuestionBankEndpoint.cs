using eDereva.Core.Contracts.Responses;
using eDereva.Core.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace eDereva.Api.Endpoints.QuestionBank;

public class GetQuestionBankEndpoint(IQuestionBankRepository questionBankRepository)
    : EndpointWithoutRequest<Results<Ok<QuestionBankDto>, BadRequest<string>>>
{
    public override void Configure()
    {
        Get("/question-banks/{questionBankId}");
        Version(1);
        Policies("RequireViewQuestionBanks");
        Description(options =>
        {
            options.WithTags("QuestionBank")
                .WithSummary("Retrieves a specific question bank")
                .WithDescription(
                    "This endpoint retrieves a specific question bank by its unique identifier. Returns the question bank details including name, description, and associated questions.");
        });
    }

    public override async Task<Results<Ok<QuestionBankDto>, BadRequest<string>>> ExecuteAsync(CancellationToken ct)
    {
        var questionBankId = Route<Guid>("questionBankId");
        
        var questionBank = await questionBankRepository.GetQuestionBankByIdAsync(questionBankId, ct);

        if (questionBank is null)
        {
            return TypedResults.BadRequest("Question bank not found");
        }

        return TypedResults.Ok(questionBank);
    }
}