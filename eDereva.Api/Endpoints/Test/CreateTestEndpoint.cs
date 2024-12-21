using eDereva.Core.Contracts.Requests;
using eDereva.Core.Repositories;
using FastEndpoints;

namespace eDereva.Api.Endpoints.Test;

public class CreateTestEndpoint(ITestRepository testRepository)
    : Endpoint<TestDto, Core.Entities.Test>
{
    public override void Configure()
    {
        Post("/tests");
        Version(1);
        AllowAnonymous();
        Description(options =>
        {
            options.WithTags("Test")
                .WithSummary("Create a test")
                .WithDescription(
                    "Create a test");
        });
    }

    public override async Task HandleAsync(TestDto req, CancellationToken ct)
    {
        var test = new Core.Entities.Test
        {
            Id = Guid.CreateVersion7(),
            Name = req.Name,
            Description = req.Description,
            Duration = req.Duration,
            PassScore = req.PassScore,
            TestVersion = req.TestVersion
        };

        await testRepository.AddAsync(test, ct);

        await SendOkAsync(ct);
    }
}