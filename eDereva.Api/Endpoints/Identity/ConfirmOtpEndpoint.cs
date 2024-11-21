using eDereva.Api.Exceptions;
using eDereva.Core.Services;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace eDereva.Api.Endpoints.Identity;

public class ConfirmOtpEndpoint(IOtpService otpService) : EndpointWithoutRequest<Results<Ok, BadRequest>>
{
    public override void Configure()
    {
        Get("/identity/confirm-otp/{phone-number}/{otp}");
        Version(1);
        AllowAnonymous();
        Description(options =>
        {
            options.WithTags("Identity")
                .WithSummary("Confirm OTP")
                .WithDescription("Confirm OTP for a phone number");
        });
        Throttle(
            5,
            60,
            "X-Client-Id" // this is optional
        );
    }

    public override async Task<Results<Ok, BadRequest>> ExecuteAsync(CancellationToken ct)
    {
        var phoneNumber = Route<string>("phone-number");
        var otp = Route<string>("otp");

        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new ProblemException("Phone number is required", "please provide your phone number");

        if (string.IsNullOrWhiteSpace(otp))
            throw new ProblemException("OTP is required", "please provide the OTP");

        var isOtpValid = await otpService.ConfirmOtp(phoneNumber, otp);

        return isOtpValid
            ? (Results<Ok, BadRequest>)TypedResults.Ok()
            : throw new ProblemException("Invalid OTP", "The OTP provided is invalid");
    }
}