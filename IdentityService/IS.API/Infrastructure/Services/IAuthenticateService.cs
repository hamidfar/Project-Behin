namespace IS.API.Infrastructure.Services
{

    public interface IAuthenticateService
    {
        Task<string> AuthenticateAsync(string username, string password);
        Task<bool> RevokeToken(string token);
    }

}
