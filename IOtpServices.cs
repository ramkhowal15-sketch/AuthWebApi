using System.Threading.Tasks;
using AuthWebApi.Domains;

namespace AuthWebApi.AuthServices.AuthServe
{
    public interface IOtpServices
    {
        Task Forget(string username);
        Task VerifyOtp(int otpCode, string username);
        Task<int> GenrateOtp(User user);
    }
}