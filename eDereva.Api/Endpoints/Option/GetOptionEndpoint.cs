using eDereva.Core.Contracts.Responses;
using eDereva.Core.Repositories;
using FastEndpoints;
using Microsoft.Extensions.Caching.Hybrid;

namespace eDereva.Api.Endpoints.Option;

public class GetOptionEndpoint(IOptionRepository optionRepository, HybridCache hybridCache)
    : EndpointWithoutRequest<OptionDto>
{
    public override void Configure()
    {
        Get("/options/{optionId}");
        Version(1);
        AllowAnonymous();
        Description(options =>
        {
            options.WithTags("Option")
                .WithSummary("Get Option")
                .WithDescription(
                    "This endpoint retrieves a specific option by its unique identifier. Returns the option details including name, description, and associated permissions.");
        });
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        var optionId = Route<Guid>("optionId");

        var cacheKey = $"option-{optionId}";

        var option = await hybridCache.GetOrCreateAsync<Core.Entities.Option>(cacheKey, async token =>
            await optionRepository.GetByIdAsync(optionId, token) ?? new Core.Entities.Option(), cancellationToken: ct);

        var optionDto = new OptionDto
        {
            Id = option.Id,
            Text = option.Text,
            IsCorrect = option.IsCorrect,
            QuestionId = option.QuestionId
        };

        await SendOkAsync(optionDto, ct);
    }
}