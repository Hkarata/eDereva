using eDereva.Core.Contracts.Responses;
using eDereva.Core.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace eDereva.Api.Endpoints.Question;

public class GetQuestionEndpoint (IQuestionBankRepository questionBankRepository, ILogger<GetQuestionEndpoint> logger) 
    : EndpointWithoutRequest<Results<Ok<QuestionDto>, BadRequest<string>>>
{
    public override void Configure()
    {
        logger.LogInformation("Configuring GetQuestionEndpoint");
        Get("/questions/{questionId}");
        Version(1);
        Policies("RequireViewQuestionBanks");
        Description(options =>
        {
            options.WithTags("Question")
                .WithSummary("Retrieves a specific question")
                .WithDescription(
                    "Retrieves a specific question by its unique identifier. Returns the question details including name, description, and associated permissions.");
        });
    }

    public override async Task<Results<Ok<QuestionDto>, BadRequest<string>>> ExecuteAsync(CancellationToken ct)
    {
        var questionId = Route<Guid>("questionId");
        logger.LogInformation("Received request to get question with ID: {QuestionId}", questionId);
        
        var question = await questionBankRepository.GetByIdAsync(questionId, ct);
        
        if (question is null)
        {
            logger.LogWarning("Question with ID: {QuestionId} not found", questionId);
            return TypedResults.BadRequest("Question not found");
        }

        logger.LogInformation("Successfully retrieved question with ID: {QuestionId}", questionId);
        return TypedResults.Ok(question);
    }
}