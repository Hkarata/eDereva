using eDereva.Core.Contracts.Requests;
using eDereva.Core.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace eDereva.Api.Endpoints.QuestionBank;

public class CreateQuestionBankEndpoint (IQuestionBankRepository questionBankRepository)
    : Endpoint<QuestionBankDto, Results<Ok, BadRequest<string>>>
{
    public override void Configure()
    {
        Post("/question-banks");
        Version(1);
        Policies("RequireManageQuestionBanks");
        Description(options => 
            options.WithTags("QuestionBank")
            .WithSummary("Create a new question bank")
            .WithDescription(
                "This endpoint allows the creation of a new question bank. The request should include question bank details such as name, description, and any questions to be assigned to the question bank."
            ));
    }

    public override async Task HandleAsync(QuestionBankDto req, CancellationToken ct)
    {
        var questionBank = req.MapToQuestionBank();
        
        await questionBankRepository.AddQuestionBankAsync(questionBank, ct);
        
        await SendOkAsync(ct);
    }
}