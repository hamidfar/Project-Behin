namespace IS.API.Infrastructure.Services
{
    public interface ITokenBlacklistService
    {
        Task<bool> RevokeTokenAsync(string token);
        Task<bool> IsTokenRevokedAsync(string token);
    }
}
