using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuthWebApi.Domains
{
    public class Otp
    {
        
        public int Id { get; set; }
        public int OtpCode { get; set; }

        public DateTime Time { get; set; }
        public bool IsUsed { get; set; }

        [ForeignKey("User")]
        public int UserId { get; set; }
        public User User { get; set; }
    }
}
