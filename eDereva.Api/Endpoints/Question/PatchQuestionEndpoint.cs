using eDereva.Core.Contracts.Responses;
using eDereva.Core.Repositories;
using FastEndpoints;

namespace eDereva.Api.Endpoints.Question;

public class PatchQuestionEndpoint(IQuestionRepository questionRepository) : Endpoint<QuestionDto>
{
    public override void Configure()
    {
        Patch("/questions/{questionId}");
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

    public override async Task HandleAsync(QuestionDto req, CancellationToken ct)
    {
        var questionId = Route<Guid>("questionId");

        var question = await questionRepository.GetByIdAsync(questionId, ct);

        if (question == null)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        question.QuestionText = req.QuestionText;
        question.Scenario = req.Scenario;
        question.ImageUrls = req.ImageUrls ?? [];
        question.Options = req.Choices?.Select(choice => new Core.Entities.Option
        {
            Id = choice.Id,
            Text = choice.Text,
            IsCorrect = choice.IsCorrect,
            QuestionId = question.Id
        }).ToList();

        await questionRepository.UpdateAsync(question, ct);
        await SendOkAsync(req, ct);
    }
}