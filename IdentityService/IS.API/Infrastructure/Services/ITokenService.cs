using IS.Domain.AggregatesModel.UserAggregate;

namespace IS.API.Infrastructure.Services
{
    public interface ITokenService
    {
        string GenerateJwtToken(User user);
        bool ValidateJwtToken(string token);
    }
}

