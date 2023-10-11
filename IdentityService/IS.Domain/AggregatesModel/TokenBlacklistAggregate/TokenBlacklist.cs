using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IS.Domain.AggregatesModel.TokenBlacklistAggregate
{
    public class TokenBlacklist
    {
        public Guid Id { get; set; }
        public string Token { get; set; }
        public DateTime RevocationTime { get; set; }
    }

}
