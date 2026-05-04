using AuthWebApi.Domains;
using System.Threading.Tasks;

namespace AuthWebApi.AuthServices.AuthServe
{
    public interface IOtpServices
    {
        public string GenerateOtp(int username);
        Task<bool> VerifyOtp(int userId, int OtpCode);
    }
}
