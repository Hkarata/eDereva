using eDereva.Core.Contracts.Requests;
using eDereva.Core.Entities;
using eDereva.Core.Repositories;
using FastEndpoints;

namespace eDereva.Api.Endpoints.TestSection;

public class CreateTestSectionEndpoint(
    ITestSectionRepository testSectionRepository,
    ISectionTemplateRepository sectionTemplateRepository
) : Endpoint<TestSectionDto>
{
    public override void Configure()
    {
        Post("/test-sections");
        Version(1);
        Policies("RequireAdministrator");
        Description(options =>
        {
            options.WithTags("Test Section")
                .WithSummary("Create a new test section")
                .WithDescription(
                    "This endpoint allows the creation of a new test section. The request should include test section details such as name, description, and any questions to be assigned to the test section.");
        });
    }

    public override async Task HandleAsync(TestSectionDto req, CancellationToken ct)
    {
        var testSection = new Core.Entities.TestSection
        {
            Id = Guid.CreateVersion7(),
            TestId = req.TestId,
            SectionTemplateId = Guid.CreateVersion7(),
            ContributionCount = req.ContributionCount
        };

        var sectionTemplate = new SectionTemplate
        {
            Id = testSection.SectionTemplateId,
            Name = req.Name,
            Description = req.Description
        };

        await sectionTemplateRepository.AddAsync(sectionTemplate, ct);

        await testSectionRepository.AddAsync(testSection, ct);

        await SendOkAsync(ct);
    }
}