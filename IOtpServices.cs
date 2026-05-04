namespace AuthWebApi.Services.OtpServices
{
    public interface IOtpServices
    {
        Task<string> GenerateOtp(string username);
        Task<bool> ValidateOtp(string username, int otpCode);
    }
}