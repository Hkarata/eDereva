using eDereva.Core.Contracts.Requests;
using eDereva.Core.Entities;
using eDereva.Core.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace eDereva.Api.Endpoints.Question;

public class UpdateQuestionEndpoint(IQuestionBankRepository questionBankRepository)
    : Endpoint<QuestionDto, Results<Ok, BadRequest<string>>>
{
    public override void Configure()
    {
        Put("/questions/{questionId}");
        Version(1);
        Policies("RequireEditQuestionBanks");
        Description(options =>
        {
            options.WithTags("Question")
                .WithSummary("Update an existing question")
                .WithDescription(
                    "This endpoint allows the update of an existing question. The request should include question details such as title, description, and any answers to be assigned to the question.");
        });
    }

    public override async Task HandleAsync(QuestionDto req, CancellationToken ct)
    {
        var questionId = Route<Guid>("questionId");

        var results = req.MapToQuestion(questionId);

        await questionBankRepository.UpdateAsync(results.question, ct);

        var answer = new Answer
        {
            QuestionId = questionId,
            ChoiceId = results.choiceId
        };

        await questionBankRepository.UpdateAnswerAsync(answer, ct);

        await SendOkAsync(ct);
    }
}