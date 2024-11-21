using eDereva.Core.Contracts.Requests;
using eDereva.Core.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace eDereva.Api.Endpoints.QuestionBank;

public class UpdateQuestionBankEndpoint (IQuestionBankRepository questionBankRepository)
    : Endpoint<QuestionBankDto, Results<Ok, BadRequest>>
{
    public override void Configure()
    {
        Put("/question-banks/{questionBankId}");
        Version(1);
        Policies("RequireManageQuestionBanks");
        Description(options =>
        {
            options.WithTags("QuestionBank")
                .WithSummary("Update an existing question bank")
                .WithDescription(
                    "This endpoint allows the update of an existing question bank. The request should include question bank details such as name, description, and any questions to be assigned to the question bank."
                );
        });
    }

    public override async Task HandleAsync(QuestionBankDto req, CancellationToken ct)
    {
        var questionBankId = Route<Guid>("questionBankId");

        var questionBank = new Core.Entities.QuestionBank
        {
            Id = questionBankId,
            Name = req.Name,
            Description = req.Description
        };
        
        await questionBankRepository.UpdateQuestionBankAsync(questionBank, ct);

        await SendOkAsync(ct);
    }
}