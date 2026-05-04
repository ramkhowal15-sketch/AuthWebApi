using AuthWebApi.AuthServices.AuthServe;
using AuthWebApi.AuthServices.AuthServee;
using AuthWebApi.Domains;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AuthWebApi.Controllers.Auth
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuthSevice _authSevice;
        private readonly IOtpServices _otpServices;

        public AuthController(ApplicationDbContext context, IAuthSevice authSevice, IOtpServices otpServices)
        {
            _context = context;
            _authSevice = authSevice;
            _otpServices = otpServices;
        }


        [HttpPost("login")]
        //[Authorize]
        public async Task<IActionResult> Login(string username, string password)
        {
            return Ok("Login Successful");
        }
        [HttpPost("register")]
        public async Task<IActionResult> Register(string name,string username, string password)
          {
          var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == username);
            if (user != null)
            {
                return BadRequest("Username already exists");
            }
            var newUser = new User
            {
                Name= name,
                Email = username,
                Password = password
            };
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();
            return Ok("Registration Successful");                                                   

        }
        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword(string email)
        {
            try
            {
                await _authSevice.ForgetPassword(email); // Replace 123456 with the actual OTP generation logic
                return Ok(new { message = "OTP sent to your registered email." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpPost("Verify-Otp")]
        public async Task<IActionResult>VerifyOtp(int Otpcode,string email, int UserId)
        {
            var otprecord = await _context.Otps.Where(x=>x.UserId==UserId).OrderByDescending(x=>x.Time).FirstOrDefaultAsync();
            if(otprecord == null)
            return BadRequest("No Otp");

            if (otprecord.IsUsed)
            return BadRequest("Otp Already Used");

            if (otprecord.Time.AddMinutes(10) < DateTime.UtcNow)
                return BadRequest("Otp Exipers");
            

            var TodayOtpCount = await _context.Otps.Where(x => x.UserId == UserId && x.Time.Date == DateTime.Now.Date).CountAsync();
            if (TodayOtpCount >= 10)
                return BadRequest("You Have Reached Today Otp Limit");

            if (otprecord.OtpCode != Otpcode)
            {
                return BadRequest("Invailid OTP");
            }
            
            otprecord.IsUsed = true;

            return Ok(new
            {
                message = "OTP verified succecfully"
            });
            

        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword(string email, int otp, string newPassword)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
                if (user == null)
                {
                    return NotFound(new { message = "User not found." });
                }

                var isValidOtp = await _otpServices.VerifyOtp(user.Id, otp);
                if (!isValidOtp)
                {
                    return BadRequest(new { message = "Invalid OTP." });
                }

                user.Password = newPassword;
                await _context.SaveChangesAsync();
                return Ok(new { message = "Password reset successful." });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }
        [HttpPut("email/id")]
        public async Task<IActionResult>Update(int UserId,string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            return BadRequest("Email is Reqiured");
            

            var user = await _context.Users.FindAsync(UserId);

            if(user == null)
            return BadRequest("User Not Found");
            
            var emailexits =await _context.Users.AnyAsync(x=> x.Email == email&& x.Id!=UserId);

            if (emailexits == true)
            return BadRequest("Email Already Exits");

                user.Email = email;
            
            

            _context.Users.Update(user);
            await _context.SaveChangesAsync();

            return Ok(new
            {
             message ="Email Updated Succesfully",
             UserId = user.Id,
             email = user.Email,
            });

        }

        }
}
