using eDereva.Core.Contracts.Requests;
using eDereva.Core.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace eDereva.Api.Endpoints.Question;

public class CreateQuestionEndpoint (IQuestionBankRepository questionBankRepository) 
    : Endpoint<QuestionDto, Results<Ok, BadRequest<string>>>
{
    public override void Configure()
    {
        Post("/questions/{questionBankId}");
        Version(1);
        Description(options =>
        {
            options.WithTags("Question")
                .WithSummary("Create a new question")
                .WithDescription(
                    "This endpoint allows the creation of a new question. The request should include question details such as title, description, and any answers to be assigned to the question.");
        });
    }

    public override async Task HandleAsync(QuestionDto req, CancellationToken ct)
    {
        var questionBankId = Route<Guid>("questionBankId");
        
        var results = req.MapToQuestion();
        
        results.question.QuestionBankId = questionBankId;

        await questionBankRepository.AddAsync(results.question, ct);

        await questionBankRepository.AddAnswerAsync(results.questionId, results.choiceId, ct);

        await SendOkAsync(ct);
    }
}