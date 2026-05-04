using Microsoft.EntityFrameworkCore;

namespace AuthWebApi.Domains
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }
        public DbSet<User> Users { get; set; }
        
        public DbSet<Otp>Otps { get; set; }
    }
}
