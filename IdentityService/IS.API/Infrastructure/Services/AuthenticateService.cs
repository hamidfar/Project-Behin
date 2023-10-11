using Azure.Core;
using IS.Domain.AggregatesModel.UserAggregate;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;


namespace IS.API.Infrastructure.Services
{

    public class AuthenticateService: IAuthenticateService
    {
        private readonly IUserRepository _userRepository;
        private readonly ITokenService _tokenService;
        private readonly ITokenBlacklistService _tokenBlacklistService;
        private readonly SignInManager<User> _signInManager;
        private readonly IConfiguration _configuration;

        public AuthenticateService(
            IUserRepository userRepository,
            ITokenService tokenService,
            ITokenBlacklistService tokenBlacklistService,
            SignInManager<User> signInManager,
            IConfiguration configuration)
        {
            _userRepository = userRepository;
            _tokenService = tokenService;
            _tokenBlacklistService = tokenBlacklistService;
            _signInManager = signInManager;
            _configuration = configuration;
        }

        public async Task<string> AuthenticateAsync(string username, string password)
        {
            var user = await _userRepository.GetUserByUsername(username);
            if (user == null)
            {
                return null; 
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, password, false);
            
            if (!result.Succeeded)
            {
                return null; 
            }

            var token = _tokenService.GenerateJwtToken(user);

   

            return token;
        }

        public async Task<bool> RevokeToken(string token)
        {
            if (_tokenService.ValidateJwtToken(token))
            {
                return await _tokenBlacklistService.RevokeTokenAsync(token);
            }
            return false;
        }

    }

}
