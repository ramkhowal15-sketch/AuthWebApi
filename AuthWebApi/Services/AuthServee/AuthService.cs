using AuthWebApi.AuthServices.AuthServe;
using AuthWebApi.Domains;
using AuthWebApi.Services.EmailServices;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;

namespace AuthWebApi.AuthServices.AuthServee
{
    public class AuthService : IAuthSevice
    {
        private readonly ApplicationDbContext _context;
        private readonly IOtpServices _otpServices;
        private readonly IMailServices _mailServices;

        public AuthService(ApplicationDbContext context, IOtpServices otpServices, IMailServices mailServices)
        {
            _context = context;
            _otpServices = otpServices;
            _mailServices = mailServices;
        }

        public async Task Login(string username, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == username && x.Password == password);
            if (user == null)
            {
                throw new Exception("Invalid Username or Password");
            }

            _context.SaveChanges();

            throw new Exception("Login Successful");
        }

        public async Task Register(string username, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == username);
            if (user != null)
            {
                throw new Exception("Username already exists");
            }
            var newUser = new User
            {
                Email = username,
                Password = password
            };
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();
            throw new Exception("Registration Successful");
        }

        // Fixed signature to match IAuthSevice: otp is an int
        public async Task<string> ForgetPassword(string email)
        {
            var user=await _context.Users.FirstOrDefaultAsync(x=>x.Email== email);
            if(user==null)
            {
                throw new Exception("User not Found");
            }
            var otp =  _otpServices.GenerateOtp(user.Id);

            var mail = new Mail
            {
                To = user.Email,
                Subject = "Password Reset OTP",
                Body = $"Your OTP for password reset is: {otp}"
            };

            await _mailServices.SendMail(mail);

            return "Success";
         }
        public async Task ResetPassword(string email, int  VerifyOtp, string newPassword)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
            if (user==null)
            {
                throw new Exception("User not Found");
            }

           
            var isValidOtp = await _otpServices.VerifyOtp(user.Id, VerifyOtp);
            if (!isValidOtp)
            {
                throw new Exception("Invalid or expired OTP");
            }

           
            user.Password = newPassword;
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }
    }
}
