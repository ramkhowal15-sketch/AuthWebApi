using AuthWebApi.AuthServices.AuthServe;
using AuthWebApi.Domains;
using Microsoft.EntityFrameworkCore;

namespace AuthWebApi.AuthServices.AuthServee
{
    public class AuthService : IAuthSevice
    {
        private readonly ApplicationDbContext _context;
        private readonly IOtpServices _otpServices;

        public AuthService(ApplicationDbContext context, IOtpServices otpServices)
        {
            _context = context;
            _otpServices = otpServices;
        }

        public async Task Login(string username, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == username && x.Password == password);
            if (user == null)
            {
                throw new Exception("Invalid Username or Password");
            }

            _context.SaveChanges();
            throw new Exception("Login Successful");
        }

        public async Task Register(string username, string password)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == username);
            if (user != null)
            {
                throw new Exception("Username already exists");
            }
            var newUser = new User
            {
                Username = username,
                Password = password
            };
            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();
            throw new Exception("Registration Successful");
        }
        public async Task ForgetPassword(string username)
        {
            var user = await _context.Users.FirstOrDefaultAsync(x => x.Username == username);
            if (user == null)
            {
                throw new Exception("Username does not exist");
            }
           
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
           
        }
       
    }
}
