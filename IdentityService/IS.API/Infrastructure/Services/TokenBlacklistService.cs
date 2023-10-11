using IS.Domain.AggregatesModel.TokenBlacklistAggregate;

namespace IS.API.Infrastructure.Services
{
    public class TokenBlacklistService:ITokenBlacklistService
    {
        private readonly ITokenBlacklistRepository _tokenBlacklistRepository;

        public TokenBlacklistService(ITokenBlacklistRepository tokenBlacklistRepository)
        {
            _tokenBlacklistRepository = tokenBlacklistRepository;
        }

        public async Task<bool> RevokeTokenAsync(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new ArgumentNullException(nameof(token), "Token cannot be null or empty.");
            }

            if (!await _tokenBlacklistRepository.IsTokenBlacklistedAsync(token))
            {
                await _tokenBlacklistRepository.AddTokenToBlacklistAsync(token);
            }
            return true;
        }

        public async Task<bool> IsTokenRevokedAsync(string token)
        {
            if (string.IsNullOrEmpty(token))
            {
                throw new ArgumentNullException(nameof(token), "Token cannot be null or empty.");
            }

            return await _tokenBlacklistRepository.IsTokenBlacklistedAsync(token);
        }
    }

}
