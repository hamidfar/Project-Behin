using IS.Domain.AggregatesModel.TokenBlacklistAggregate;
using IS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IS.Infrastructure.Repositories
{

    public class TokenBlacklistRepository : ITokenBlacklistRepository
    {
        private readonly IdentityDbContext _context;

        public TokenBlacklistRepository(IdentityDbContext context)
        {
            _context = context;
        }

        public async Task AddTokenToBlacklistAsync(string token)
        {
            var blacklistEntry = new TokenBlacklist
            {
                Token = token,
                RevocationTime = DateTime.UtcNow
            };

            _context.TokenBlacklist.Add(blacklistEntry);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsTokenBlacklistedAsync(string token)
        {
            return await _context.TokenBlacklist
                .AnyAsync(blacklist => blacklist.Token == token);
        }
    }

}
