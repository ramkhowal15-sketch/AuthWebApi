using AuthWebApi.Domains;

namespace AuthWebApi.Services.EmailServices
{
    public interface IMailServices
    {
         Task<string> SendMail(Mail mail);

    }
}
