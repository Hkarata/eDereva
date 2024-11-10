using eDereva.Api.Exceptions;
using eDereva.Core.Contracts.Requests;
using eDereva.Core.Services;
using FastEndpoints;
using Microsoft.AspNetCore.Http.HttpResults;

namespace eDereva.Api.Endpoints.Identity
{
    public class RequestOtpEndpoint(IOtpService otpService, ISmsService smsService) : EndpointWithoutRequest<Results<Ok, BadRequest>>
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

            var Otp = await otpService.GenerateOtpAsync(phoneNumber);

            if (Otp is null)
                throw new ProblemException("Failed to generate OTP", "Failed to generate OTP.");

            var phone = 255 + phoneNumber.TrimStart('0');

            var sms = new Sms
            {
                source_addr = "RSAllies",
                schedule_time = string.Empty,
                encoding = "0",
                message = $"Hello, your OTP is {Otp}. Use it before it expires.\nHabari, OTP yako ni {Otp}. Tumia kabla muda wake hujaisha.",
                recipients =
                [
                    new() { recipient_id = "1", dest_addr = phone}
                ]
            };

            await smsService.SendMessageAsync(sms);

            return (Results<Ok, BadRequest>)TypedResults.Ok();
        }
    }
}
