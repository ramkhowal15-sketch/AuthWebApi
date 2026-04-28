
using AuthWebApi.Domains;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuthWebApi.AuthServices.AuthServe
{
    public class OtpServices : IOtpServices
    {
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;

        public OtpServices(IConfiguration configuration, ApplicationDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        public async Task Forget(string username)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == username);

            if (user == null)
            {
                throw new Exception("User not found");
            }

            var Otp = new Random().Next(1000, 9999);
            var otp = new Otp
            {
                OtpCode = Otp,
                UserId = user.Id,
                Time = DateTime.UtcNow,
                IsUsed = false
            };
            _context.Otps.Add(otp);
            await _context.SaveChangesAsync();

            // otp send to mail
        }

        public async Task VerifyOtp(int otpCode,string username)
        {
            var otp = await _context.Otps.FirstOrDefaultAsync(x=> x.OtpCode == otpCode && x.User.Username == username);

            if (otp == null)
            {
                throw new Exception("Invalid OTP");
            }

            if(otp.IsUsed)
            {
                throw new Exception("OTP already used");
            }

            if(otp.Time < DateTime.Now.AddMinutes(-10))
            {
                throw new Exception("OTP expired");
            }

            otp.IsUsed = true;
            _context.Otps.Update(otp);
            await _context.SaveChangesAsync();

            throw new Exception("OTP verified successfully");
        }



    }
}
