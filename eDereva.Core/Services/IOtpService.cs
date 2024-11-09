namespace eDereva.Core.Services
{
    public interface IOtpService
    {
        Task<string> GenerateOtpAsync(string phoneNumber);
        Task<bool> ConfirmOtp(string phoneNumber, string otp);
    }
}
