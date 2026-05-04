namespace AuthWebApi.AuthServices.AuthServee
{
    public interface IAuthSevice
    {
        Task Login(string username, string password);
        Task Register(string username, string password);
        Task<string> ForgetPassword(string email);




    }
}
