using eDereva.Core.Contracts.Requests;
using eDereva.Core.Repositories;
using FastEndpoints;

namespace eDereva.Api.Endpoints.Test;

public class PatchTestEndpoint(ITestRepository testRepository) : Endpoint<TestDto>
{
    public override void Configure()
    {
        Patch("/tests/{testId}");
        Version(1);
        Description(options =>
        {
            options.WithTags("Test")
                .WithSummary("Patch Test")
                .WithDescription(
                    "This endpoint is used to patch a test");
        });
    }

    public override async Task HandleAsync(TestDto req, CancellationToken ct)
    {
        var testId = Route<Guid>("testId");
        var test = await testRepository.GetByIdAsync(testId, ct);

        if (test is null)
        {
            await SendNotFoundAsync(ct);
            return;
        }
        
        test.Name = req.Name;
        test.Description = req.Description;
        test.TestVersion = req.TestVersion;
        test.Duration = req.Duration;
        test.PassScore = req.PassScore;
        
        await testRepository.UpdateAsync(test, ct);

        await SendOkAsync(ct);
    }
}