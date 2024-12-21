using eDereva.Core.Contracts.Responses;
using eDereva.Core.Repositories;
using FastEndpoints;
using Microsoft.Extensions.Caching.Hybrid;

namespace eDereva.Api.Endpoints.Option;

public class GetQuestionOptionsEndpoint(IOptionRepository optionRepository, HybridCache hybridCache)
    : EndpointWithoutRequest<List<OptionDto>>
{
    public override void Configure()
    {
        Get("/options/{questionId}");
        Version(1);
        AllowAnonymous();
        Description(options =>
        {
            options.WithTags("Option")
                .WithSummary("Get Question Options")
                .WithDescription(
                    "This endpoint retrieves a specific question by its unique identifier. Returns the question details including name, description, and associated options.");
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var questionId = Route<Guid>("questionId");

        var cacheKey = $"question-{questionId}-options";

        var options = await hybridCache.GetOrCreateAsync(cacheKey, async token
            => await optionRepository.GetByQuestionIdAsync(questionId, token), cancellationToken: ct);

        var optionsDto = options.Select(o => new OptionDto
        {
            Id = o.Id,
            Text = o.Text,
            IsCorrect = o.IsCorrect,
            QuestionId = o.QuestionId
        }).ToList();

        if (optionsDto.Count == 0) await SendNoContentAsync(ct);

        await SendOkAsync(optionsDto, ct);
    }
}