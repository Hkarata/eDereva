using System.Text;
using eDereva.Core.Entities;
using eDereva.Core.Services;
using eDereva.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace eDereva.Infrastructure.Services;

public class OtpService(ApplicationDbContext context, ILogger<OtpService> logger) : IOtpService
{
    private static readonly Random _random = new(); // Static random instance

    public async Task<bool> ConfirmOtp(string phoneNumber, string otp)
    {
        var otpEntity = await context.Otps
            .FirstOrDefaultAsync(o => o.PhoneNumber == phoneNumber && o.Code == otp);

        if (otpEntity == null || otpEntity.ExpiresAt < DateTime.UtcNow)
        {
            logger.LogWarning("Invalid or expired OTP for phone number: {PhoneNumber}", phoneNumber);
            return false;
        }

        context.Otps.Remove(otpEntity);
        await context.SaveChangesAsync();

        logger.LogInformation("OTP successfully confirmed for phone number: {PhoneNumber}", phoneNumber);
        return true;
    }

    public async Task<string> GenerateOtpAsync(string phoneNumber)
    {
        string otp;
        bool isUnique;

        // Ensure uniqueness by checking the OTP asynchronously
        do
        {
            otp = GenerateOtp();
            isUnique = await IsOtpUniqueAsync(otp);
        } while (!isUnique);

        var otpEntity = new Otp
        {
            Code = otp,
            PhoneNumber = phoneNumber,
            ExpiresAt = DateTime.UtcNow.AddMinutes(5)
        };

        await context.Otps.AddAsync(otpEntity);
        await context.SaveChangesAsync();

        logger.LogInformation("Generated OTP for phone number: {PhoneNumber}", phoneNumber);
        return otp;
    }

    private static string GenerateOtp()
    {
        const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
        var otp = new StringBuilder(6);

        for (var i = 0; i < 6; i++) otp.Append(chars[_random.Next(chars.Length)]);

        return otp.ToString();
    }

    private async Task<bool> IsOtpUniqueAsync(string otp)
    {
        return await context.Otps.CountAsync(o => o.Code == otp) == 0;
    }
}