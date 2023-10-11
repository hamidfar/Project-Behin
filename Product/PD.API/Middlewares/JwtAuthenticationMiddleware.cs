using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace PD.API.Middlewares
{

    public class JwtAuthenticationMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<JwtAuthenticationMiddleware> _logger;
        byte[] publicKeyBytes = Convert.FromBase64String("MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQCJHYcuRHdJ5S8GYcz/IgF5hJY+vvchVaeKyD+GGSoiD58pRJ3kx5b7YbbP/EyzwhUxWncvbsiWZFdqca/DHsFKxNnRgvidoyq2dgA+erP91aFHpccH2ykNxC6LTzMXX4XWp5mXKm6XfMkBFzsVC4/a7A6UHnsL7MU2b4lec+WkSwIDAQAB");
        public JwtAuthenticationMiddleware(
            RequestDelegate next,
            ILogger<JwtAuthenticationMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                if (context.Request.Headers.ContainsKey("Authorization"))
                {
                    var token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

                    var rsa = RSA.Create(2048);
                    rsa.ImportSubjectPublicKeyInfo(publicKeyBytes, out _);
                    var rsaKey = new RsaSecurityKey(rsa);

                    var tokenHandler = new JwtSecurityTokenHandler();
                    var validationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        RequireSignedTokens = true,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero,
                        IssuerSigningKey = rsaKey,
                    };

                    try
                    {
                        var claimsPrincipal = tokenHandler.ValidateToken(token, validationParameters, out var validatedToken);
                        context.User = claimsPrincipal;

                        if (context.User.HasClaim(c => c.Type == ClaimTypes.Role && c.Value == "ADMIN") &&
                            context.User.HasClaim(c => c.Type == "services" && c.Value == "PD"))
                        {
                            await _next(context);
                            return;
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Token validation failed.");
                        throw;
                    }
                }

                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                await context.Response.WriteAsync("Unauthorized");
                await _next(context);
                return;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in JwtAuthenticationMiddleware");
                context.Response.StatusCode = StatusCodes.Status500InternalServerError;
                throw;
            }
        }
    }

}
