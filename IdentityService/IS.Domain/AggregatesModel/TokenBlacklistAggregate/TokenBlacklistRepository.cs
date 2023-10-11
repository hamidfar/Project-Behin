using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IS.Domain.AggregatesModel.TokenBlacklistAggregate
{
    public interface ITokenBlacklistRepository
    {
        Task AddTokenToBlacklistAsync(string token);
        Task<bool> IsTokenBlacklistedAsync(string token);
    }

}
