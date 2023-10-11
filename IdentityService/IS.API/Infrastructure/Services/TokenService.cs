using IS.Domain.AggregatesModel.UserAggregate;

namespace IS.API.Infrastructure.Services
{
    using IS.Infrastructure.Repositories;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.EntityFrameworkCore.Metadata.Internal;
    using Microsoft.Extensions.Primitives;
    using Microsoft.IdentityModel.Tokens;
    using System;
    using System.IdentityModel.Tokens.Jwt;
    using System.Security.Claims;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading.Tasks;

    public class TokenService : ITokenService
    {
        byte[] privateKeyBytes = Convert.FromBase64String("MIICXAIBAAKBgQCJHYcuRHdJ5S8GYcz/IgF5hJY+vvchVaeKyD+GGSoiD58pRJ3kx5b7YbbP/EyzwhUxWncvbsiWZFdqca/DHsFKxNnRgvidoyq2dgA+erP91aFHpccH2ykNxC6LTzMXX4XWp5mXKm6XfMkBFzsVC4/a7A6UHnsL7MU2b4lec+WkSwIDAQABAoGAMV1zLOIzfGRKAOc3MefhVgm5Og/w04yODHY6AKKQu8CaEfaFTjfZkNnGQq1YRCOtE565aFdfWl335vfVSs+I0UTKYtUdU0DkeZ93nB+eaUIQ/7UC99UlcdSrlRfXGwBxdcwM+Ek93VeITWERydh+xyXN3VxzaYtApA1fB/YGnzkCQQDcb8X6xFigw0qSpHXX+qpBQcTSbIQ5u47vK0VRIkDI3vN3tGBeIpU78kK3E+cHG8e74Nobn1/nJ09TKS4jzoudAkEAnzyDQPD269xzwTvICVMDvezEN4sy3+XJdJjtOdtL4RZKRqOUiaJmKhR1QUtlrG71LqudUiwZ7DnzloKyvLYvBwJAXCyw0GcB2FdQ+3ihfipmvtrNfl+5+poe7otddMup41S24bsfAL3dQS/QDdXYqPRI1Jr1GM/PvkyFsvRpQre/UQJBAIavIDVlmvSUWjQu5Fs+pAOYp75zNmy6Z1L/pmcxXVTdDaYB5jkj61XcR/EaXL0kfK0k6sP+GU79FVNQ6O1FCzECQAD8SLqaJOGbiPYrf+gRx337xQTatlaaXIaRrtNKj4E3/WtyKQQXEILCMDS1Xa88sK12nQgu6DVxOtIs+7cEvds=");

        private readonly IConfiguration _configuration;
        private readonly IUserRepository _userRepository;
        public TokenService(IConfiguration configuration, IUserRepository userRepository)
        {
            _configuration = configuration;
            _userRepository = userRepository;
        }

        public string GenerateJwtToken(User user)
        {
            var rsa = RSA.Create(2048);
            rsa.ImportRSAPrivateKey(privateKeyBytes, out _);
            var rsaKey = new RsaSecurityKey(rsa);

            var claims = new List<Claim> {
            new Claim(ClaimTypes.Name, user.UserName)
            };

            foreach (Role role in user.Roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role.Name.ToUpper()));
            }

            foreach (var service in user.Services)
            {
                claims.Add(new Claim("services", service.Name.ToUpper()));
            }
            var expireDate = DateTime.UtcNow.AddHours(Convert.ToDouble(_configuration["Jwt:ExpirationHours"]));

            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = expireDate,
                SigningCredentials = new SigningCredentials(rsaKey, SecurityAlgorithms.RsaSha256Signature),
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"]
            };



            var token = tokenHandler.CreateToken(tokenDescriptor);
            var stringToken = tokenHandler.WriteToken(token);

            _userRepository.AddToken(user.Id, new Token(stringToken, expireDate));

            return stringToken;
        }



        public bool ValidateJwtToken(string token)
        {
            var rsa = RSA.Create(2048);
            rsa.ImportRSAPrivateKey(privateKeyBytes, out _);
            var rsaKey = new RsaSecurityKey(rsa);

            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    RequireSignedTokens = true,
                    ValidateLifetime = true, 
                    ClockSkew = TimeSpan.Zero, 
                    IssuerSigningKey = rsaKey,
                }, out SecurityToken validatedToken);

                return true;
            }
            catch (Exception ex)
            {
                return false;
            }

        }


    }
}
