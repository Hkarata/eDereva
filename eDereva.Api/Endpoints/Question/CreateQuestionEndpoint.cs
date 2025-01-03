using eDereva.Core.Contracts.Responses;
using eDereva.Core.Repositories;
using FastEndpoints;

namespace eDereva.Api.Endpoints.Question;

public class CreateQuestionEndpoint(IQuestionRepository questionRepository) : Endpoint<QuestionDto, QuestionDto>
{
    public override void Configure()
    {
        Post("/questions");
        Version(1);
        AllowAnonymous();
        Description(options =>
        {
            options.WithTags("Question")
                .WithSummary("Creates a new question")
                .WithDescription("Creates a new question");
        });
    }

    public override async Task HandleAsync(QuestionDto req, CancellationToken ct)
    {
        var question = new Core.Entities.Question
        {
            Id = Guid.NewGuid(),
            ImageUrls = req.ImageUrls,
            QuestionText = req.QuestionText,
            Scenario = req.Scenario,
            SectionTemplateId = req.SectionTemplateId,
            Options = req.Choices?.Select(choice => new Core.Entities.Option
            {
                Id = Guid.NewGuid(),
                Text = choice.Text,
                IsCorrect = choice.IsCorrect
            }).ToList()
        };

        await questionRepository.AddAsync(question, ct);

        // Return the response
        var response = new QuestionDto
        {
            Id = question.Id,
            ImageUrls = question.ImageUrls,
            QuestionText = question.QuestionText,
            Scenario = question.Scenario,
            SectionTemplateId = question.SectionTemplateId,
            Choices = question.Options?.Select(option => new OptionDto
            {
                Id = option.Id,
                Text = option.Text,
                IsCorrect = option.IsCorrect
            }).ToList()
        };

        await SendCreatedAtAsync<CreateQuestionEndpoint>(
            new { question.Id },
            response,
            cancellation: ct
        );
    }
}