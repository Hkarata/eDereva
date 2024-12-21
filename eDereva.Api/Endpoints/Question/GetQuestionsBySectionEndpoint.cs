using eDereva.Core.Contracts.Responses;
using eDereva.Core.Repositories;
using FastEndpoints;

namespace eDereva.Api.Endpoints.Question;

public class GetQuestionsBySectionEndpoint(IQuestionRepository questionRepository)
    : EndpointWithoutRequest<List<QuestionDto>>
{
    public override void Configure()
    {
        Get("/sections/{sectionId}/questions");
        Version(1);
        AllowAnonymous();
        Description(options =>
        {
            options.WithTags("Question")
                .WithSummary("Get Questions By Section")
                .WithDescription(
                    "Get Questions By Section");
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var sectionId = Route<Guid>("sectionId");

        var questions = await questionRepository.GetBySectionTemplateIdAsync(sectionId, ct);

        var response = questions.Select(q => new QuestionDto
        {
            Id = q.Id,
            Scenario = q.Scenario,
            ImageUrls = q.ImageUrls,
            QuestionText = q.QuestionText,
            Choices = q.Options?.Select(o => new OptionDto
            {
                Id = o.Id,
                Text = o.Text,
                IsCorrect = o.IsCorrect,
                QuestionId = o.QuestionId
            }).ToList(),
            SectionTemplateId = q.SectionTemplateId
        }).ToList();

        await SendAsync(response, cancellation: ct);
    }
}