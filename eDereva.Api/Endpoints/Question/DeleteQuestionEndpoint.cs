using eDereva.Core.Repositories;
using FastEndpoints;

namespace eDereva.Api.Endpoints.Question;

public class DeleteQuestionEndpoint(IQuestionRepository questionRepository) : EndpointWithoutRequest
{
    public override void Configure()
    {
        Delete("/questions/{questionId}");
        Version(1);
        AllowAnonymous();
        Description(options =>
        {
            options.WithTags("Question")
                .WithSummary("Update an existing question")
                .WithDescription(
                    "Update an existing question");
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var questionId = Route<Guid>("questionId");

        await questionRepository.DeleteAsync(questionId, ct);

        await SendOkAsync(ct);
    }
}