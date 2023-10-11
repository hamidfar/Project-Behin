using IS.API.Infrastructure.Services;
using IS.Domain.AggregatesModel.UserAggregate;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Threading.Tasks;
using Xunit;

namespace IS.Test
{
    public class AuthenticateServiceTests
    {
        private readonly AuthenticateService _authenticateService;
        private readonly Mock<IUserRepository> _userRepositoryMock = new Mock<IUserRepository>();
        private readonly Mock<ITokenService> _tokenServiceMock = new Mock<ITokenService>();
        private readonly Mock<ITokenBlacklistService> _tokenBlacklistServiceMock = new Mock<ITokenBlacklistService>();
        private readonly Mock<IConfiguration> _configurationMock = new Mock<IConfiguration>();
        private readonly Mock<SignInManager<User>> _signInManagerMock = MockSignInManager.Create();

        public AuthenticateServiceTests()
        {
            _authenticateService = new AuthenticateService(
                _userRepositoryMock.Object,
                _tokenServiceMock.Object,
                _tokenBlacklistServiceMock.Object,
                _signInManagerMock.Object,
                _configurationMock.Object);
        }

        [Fact]
        public async Task AuthenticateAsync_ValidCredentials_ReturnsToken()
        {
            
            var username = "testuser";
            var password = "testpassword";
            var user = new User { UserName = "userTest" , PasswordHash = "1234" };
            _userRepositoryMock.Setup(repo => repo.GetUserByUsername(username)).ReturnsAsync(user);

            
            _signInManagerMock
                .Setup(signInManager => signInManager.CheckPasswordSignInAsync(user, password, false))
                .ReturnsAsync(SignInResult.Success);

            
            var expectedToken = "valid-token";
            _tokenServiceMock.Setup(tokenService => tokenService.GenerateJwtToken(user)).Returns(expectedToken);

            
            var token = await _authenticateService.AuthenticateAsync(username, password);

            
            Assert.Equal(expectedToken, token);
        }

        [Fact]
        public async Task AuthenticateAsync_InvalidUsername_ReturnsNull()
        {
            
            var username = "nonexistentuser";
            var password = "testpassword";
            _userRepositoryMock.Setup(repo => repo.GetUserByUsername(username)).ReturnsAsync((User)null);

            
            var token = await _authenticateService.AuthenticateAsync(username, password);

            
            Assert.Null(token);
        }

        [Fact]
        public async Task AuthenticateAsync_InvalidPassword_ReturnsNull()
        {
            
            var username = "testuser";
            var password = "wrongpassword";
            var user = new User { UserName = "userTest", PasswordHash = "1234" };
            _userRepositoryMock.Setup(repo => repo.GetUserByUsername(username)).ReturnsAsync(user);

            _signInManagerMock
                .Setup(signInManager => signInManager.CheckPasswordSignInAsync(user, password, false))
                .ReturnsAsync(SignInResult.Failed);

            
            var token = await _authenticateService.AuthenticateAsync(username, password);

            
            Assert.Null(token);
        }

        [Fact]
        public async Task RevokeToken_ValidToken_RevokesToken()
        {
            
            var token = "valid-token";

            _tokenServiceMock.Setup(tokenService => tokenService.ValidateJwtToken(token)).Returns(true);

            _tokenBlacklistServiceMock.Setup(blacklistService => blacklistService.RevokeTokenAsync(token)).ReturnsAsync(true);

            
            var result = await _authenticateService.RevokeToken(token);

            
            Assert.True(result);
        }

        [Fact]
        public async Task RevokeToken_InvalidToken_DoesNotRevokeToken()
        {
            
            var token = "invalid-token";

            _tokenServiceMock.Setup(tokenService => tokenService.ValidateJwtToken(token)).Returns(false);

            
            var result = await _authenticateService.RevokeToken(token);

            
            Assert.False(result);
        }
    }

    public class MockSignInManager : SignInManager<User>
    {
        private MockSignInManager(UserManager<User> userManager, IHttpContextAccessor contextAccessor, IUserClaimsPrincipalFactory<User> claimsFactory, IOptions<IdentityOptions> optionsAccessor, ILogger<SignInManager<User>> logger, IAuthenticationSchemeProvider schemes, IUserConfirmation<User> confirmation)
            : base(userManager, contextAccessor, claimsFactory, optionsAccessor, logger, schemes, confirmation)
        {
        }

        public static Mock<SignInManager<User>> Create()
        {
            var userManager = MockUserManager.Create();

            var contextAccessor = new Mock<IHttpContextAccessor>();
            var claimsFactory = new Mock<IUserClaimsPrincipalFactory<User>>();
            var optionsAccessor = new Mock<IOptions<IdentityOptions>>();
            var logger = new Mock<ILogger<SignInManager<User>>>();
            var schemes = new Mock<IAuthenticationSchemeProvider>();
            var confirmation = new Mock<IUserConfirmation<User>>();

            return new Mock<SignInManager<User>>(
                userManager.Object,
                contextAccessor.Object,
                claimsFactory.Object,
                optionsAccessor.Object,
                logger.Object,
                schemes.Object,
                confirmation.Object);
        }

    }
    public class MockUserManager : UserManager<User>
    {
        public MockUserManager(IUserStore<User> store, IOptions<IdentityOptions> optionsAccessor, IPasswordHasher<User> passwordHasher, IEnumerable<IUserValidator<User>> userValidators, IEnumerable<IPasswordValidator<User>> passwordValidators, ILookupNormalizer keyNormalizer, IdentityErrorDescriber errors, IServiceProvider services, ILogger<UserManager<User>> logger)
            : base(store, optionsAccessor, passwordHasher, userValidators, passwordValidators, keyNormalizer, errors, services, logger)
        {
        }

        public static Mock<UserManager<User>> Create()
        {
            var store = new Mock<IUserStore<User>>();
            var optionsAccessor = new Mock<IOptions<IdentityOptions>>();
            var passwordHasher = new Mock<IPasswordHasher<User>>();
            var userValidators = new List<IUserValidator<User>>();
            var passwordValidators = new List<IPasswordValidator<User>>();
            var keyNormalizer = new Mock<ILookupNormalizer>();
            var errors = new Mock<IdentityErrorDescriber>();
            var services = new Mock<IServiceProvider>();
            var logger = new Mock<ILogger<UserManager<User>>>();

            return new Mock<UserManager<User>>(
                store.Object,
                optionsAccessor.Object,
                passwordHasher.Object,
                userValidators,
                passwordValidators,
                keyNormalizer.Object,
                errors.Object,
                services.Object,
                logger.Object);
        }

    }
}