using eDereva.Api.Exceptions;
using eDereva.Core.Services;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace eDereva.Api.Endpoints.Identity
{
    public class RequestOtpEndpoint(IOtpService otpService) : EndpointWithoutRequest<Results<Ok, BadRequest>>
    {
        public override void Configure()
        {
            Get("identity/request-otp/{phone-number}");
            Version(1);
            AllowAnonymous();
            Description(options =>
            {
                options.WithTags("Identity")
                    .WithSummary("Request OTP")
                    .WithDescription("Request an OTP for a phone number");
            });
            Throttle(
                hitLimit: 5,
                durationSeconds: 60,
                headerName: "X-Client-Id" // this is optional
            );
        }

        public override async Task<Results<Ok, BadRequest>> ExecuteAsync(CancellationToken ct)
        {
            var phoneNumber = Route<string>("phone-number");

            if (string.IsNullOrWhiteSpace(phoneNumber))
                throw new ProblemException("Phone number is required", "please provide your phone number");

            var otp = await otpService.GenerateOtpAsync(phoneNumber);

            return otp is null
                ? throw new ProblemException("Failed to generate OTP", "Failed to generate OTP")
                : (Results<Ok, BadRequest>)TypedResults.Ok();
        }
    }
}
