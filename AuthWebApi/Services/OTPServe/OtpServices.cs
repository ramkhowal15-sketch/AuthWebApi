using AuthWebApi.Domains;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;

namespace AuthWebApi.AuthServices.AuthServe
{
    public class OtpServices : IOtpServices
    {
        private readonly ConcurrentDictionary<string, Otp> _otpStore = new ConcurrentDictionary<string, Otp>();
        private readonly IConfiguration _configuration;
        private readonly ApplicationDbContext _context;

        public OtpServices(IConfiguration configuration, ApplicationDbContext context)
        {
            _configuration = configuration;
            _context = context;
        }

        public string GenerateOtp(int user)
        {
            var random = new Random();
            int otpCode = random.Next(1000, 9999);

            var otp = new Otp
            {
                OtpCode = otpCode,
                Time = DateTime.UtcNow,
                UserId = user,
                IsUsed = false,

            };

            _context.Otps.Add(otp);
            _context.SaveChanges();



            return otpCode.ToString();
        }

        public async Task<bool> VerifyOtp(int userId, int OtpCode)
        {
            var otpp =await _context.Otps.OrderByDescending(x => x.Time).FirstOrDefaultAsync(y=>y.UserId==userId);
         
           if(otpp.IsUsed)
            {
                return false;

            }
           else return true;

           
            


        }
    }
    }
