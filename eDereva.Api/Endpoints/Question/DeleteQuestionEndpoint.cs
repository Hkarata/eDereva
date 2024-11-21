using eDereva.Core.Repositories;
using FastEndpoints;

namespace eDereva.Api.Endpoints.Question;

public class DeleteQuestionEndpoint (IQuestionBankRepository questionBankRepository, ILogger<DeleteQuestionEndpoint> logger) 
    : EndpointWithoutRequest
{
    public override void Configure()
    {
        Delete("/questions/{questionId}");
        Version(1);
        Policies("RequireDeleteQuestionBanks");
        Description(options =>
        {
            options.WithTags("Question")
                .WithSummary("Delete question by Id")
                .WithDescription("Delete question by Id");
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var questionId = Route<Guid>("questionId");
        
        logger.LogInformation("Deleting question with Id: {QuestionId}", questionId);

        await questionBankRepository.DeleteAsync(questionId, ct);

        logger.LogInformation("Question with Id: {QuestionId} deleted successfully", questionId);

        await SendOkAsync(ct);
    }
}