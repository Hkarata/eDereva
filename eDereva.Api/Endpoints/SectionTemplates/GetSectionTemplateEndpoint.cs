using eDereva.Core.Contracts.Responses;
using eDereva.Core.Repositories;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.Extensions.Caching.Hybrid;

namespace eDereva.Api.Endpoints.SectionTemplates;

public class GetSectionTemplateEndpoint (ISectionTemplateRepository sectionTemplateRepository,
    HybridCache hybridCache) 
    : EndpointWithoutRequest<Results<Ok<SectionTemplateDto>, BadRequest<string>>>
{
    public override void Configure()
    {
        Get("/section-templates/{sectionTemplateId}");
        Version(1);
        AllowAnonymous();
        Description(options =>
        {
            options.WithTags("SectionTemplates")
                .WithSummary("Get Section Template")
                .WithDescription("Get Section Template");
        });
    }

    public override async Task<Results<Ok<SectionTemplateDto>, BadRequest<string>>> ExecuteAsync(CancellationToken ct)
    {
        var sectionTemplateId = Route<Guid>("sectionTemplateId");
        
        var cacheKey = $"sectionTemplate-{sectionTemplateId}";

        var sectionTemplate = await hybridCache.GetOrCreateAsync(cacheKey, async token 
            => await sectionTemplateRepository.GetByIdAsync(sectionTemplateId, ct), cancellationToken: ct);
        
        if (sectionTemplate is null) 
            return TypedResults.BadRequest("Section Template not found");

        var response = new SectionTemplateDto
        {
            Id = sectionTemplate.Id,
            Name = sectionTemplate.Name,
            Description = sectionTemplate.Description,
            Questions = sectionTemplate.Questions?
                .Select(q => new QuestionDto
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
                }).ToList()
        };
        
        return TypedResults.Ok(response);
    }
}