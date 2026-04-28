using AuthWebApi.Domains;
using System.Threading.Tasks;

namespace AuthWebApi.AuthServices.AuthServe
{
    public interface IOtpServices
    {
        Task Forget(string username);
       
        Task VerifyOtp(int otpCode, string username);
        
    }
}
