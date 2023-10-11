using System;
using System.Threading.Tasks;
using IS.Infrastructure.Data;
using IS.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Xunit;


namespace IS.Test
{

    public class TokenBlacklistRepositoryTests
    {
        [Fact]
        public async Task AddTokenToBlacklistAsync_Should_AddTokenToBlacklist()
        {
            var options = new DbContextOptionsBuilder<IdentityDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new IdentityDbContext(options))
            {
                var repository = new TokenBlacklistRepository(context);
                var tokenToAdd = "test-token";

                await repository.AddTokenToBlacklistAsync(tokenToAdd);

                var isTokenBlacklisted = await repository.IsTokenBlacklistedAsync(tokenToAdd);
                Assert.True(isTokenBlacklisted, "Token should be blacklisted");
            }
        }

        [Fact]
        public async Task IsTokenBlacklistedAsync_Should_ReturnFalseForNonBlacklistedToken()
        {
            var options = new DbContextOptionsBuilder<IdentityDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new IdentityDbContext(options))
            {
                var repository = new TokenBlacklistRepository(context);
                var tokenToAdd = "test-token";
                var tokenToCheck = "another-token";

                await repository.AddTokenToBlacklistAsync(tokenToAdd);
                var isTokenBlacklisted = await repository.IsTokenBlacklistedAsync(tokenToCheck);

                Assert.False(isTokenBlacklisted, "Token should not be blacklisted");
            }
        }
    }

}
