using eDereva.Core.Contracts.Responses;
using eDereva.Core.Repositories;
using FastEndpoints;
using Microsoft.Extensions.Caching.Hybrid;

namespace eDereva.Api.Endpoints.Question;

public class GetQuestionEndpoint(IQuestionRepository questionRepository, HybridCache hybridCache)
    : EndpointWithoutRequest<QuestionDto>
{
    public override void Configure()
    {
        Get("/questions/{questionId}");
        Version(1);
        AllowAnonymous();
        Description(options =>
        {
            options.WithTags("Question")
                .WithSummary("Get Question")
                .WithDescription(
                    "Get Question");
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var questionId = Route<Guid>("questionId");

        var cacheKey = $"question-{questionId}";

        var questionEntity = await hybridCache.GetOrCreateAsync(cacheKey, async token
                => await questionRepository.GetByIdAsync(questionId, token) ?? new Core.Entities.Question(),
            cancellationToken: ct);

        if (questionEntity.Id == Guid.Empty)
        {
            await SendNotFoundAsync(ct);
            return;
        }

        var question = new QuestionDto
        {
            Id = questionEntity.Id,
            Scenario = questionEntity.Scenario,
            ImageUrls = questionEntity.ImageUrls ?? new List<string>(),
            QuestionText = questionEntity.QuestionText,
            Choices = questionEntity.Options?.Select(option => new OptionDto
            {
                Id = option.Id,
                Text = option.Text,
                IsCorrect = option.IsCorrect
            }).ToList(),
            SectionTemplateId = questionEntity.SectionTemplateId
        };

        await SendAsync(question, cancellation: ct);
    }
}