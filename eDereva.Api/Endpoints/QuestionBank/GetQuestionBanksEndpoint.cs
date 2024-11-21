using eDereva.Core.Contracts.Responses;
using eDereva.Core.Repositories;
using eDereva.Core.ValueObjects;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace eDereva.Api.Endpoints.QuestionBank;

public class GetQuestionBanksEndpoint(IQuestionBankRepository questionBankRepository)
    : Endpoint<PaginationParams, Results<Ok<PaginatedResult<QuestionBankDto>>, BadRequest<string>>>
{
    public override void Configure()
    {
        Get("/question-banks");
        Version(1);
        Policies("RequireViewQuestionBanks");
        Description(options =>
        {
            options.WithTags("QuestionBank")
                .WithSummary("Retrieves a list of question banks")
                .WithDescription(
                    "This endpoint allows the retrieval of a list of question banks with pagination support.");
        });
    }

    public override async Task<Results<Ok<PaginatedResult<QuestionBankDto>>, BadRequest<string>>> ExecuteAsync(PaginationParams req, CancellationToken ct)
    {
        // Consider adding validation for req parameters if not already handled
        var questionBanks = await questionBankRepository.GetQuestionBanks(req, ct);
        
        // Return a more appropriate 404 (Not Found) instead of BadRequest for empty results
        if (questionBanks.TotalCount == 0)
            return TypedResults.BadRequest("No question banks found");
        
        return TypedResults.Ok(questionBanks);
    }
}