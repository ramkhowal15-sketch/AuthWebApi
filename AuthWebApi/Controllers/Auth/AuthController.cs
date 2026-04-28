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
        public async Task<IActionResult> Register(string username, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == username);
            if (user != null)
            {
                return BadRequest("Username already exists");
            }
            var newUser = new User
            {
                Username = username,
                Password = password
            };
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();
            return Ok("Registration Successful");

        }
        [HttpPost("forget-password")]
        public async Task<IActionResult> ForgetPassword(string username)
            {
                var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == username);
                if (user == null)
                {
                    return BadRequest("Username does not exist");
                }

                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                return Ok("Password reset successful");
        }
    }
}
