
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IS.Domain.AggregatesModel.UserAggregate
{
    public class Token
    {
        public Guid Id { get; set; }
        public string Value { get; set; }
        public DateTime Expiration { get; set; }
        public User User { get; set; }
        public Guid UserId { get; set; }

        public Token(string value, DateTime expiration)
        {
            Value = value;
            Expiration = expiration;
        }

        public bool IsExpired()
        {
            return DateTime.Now >= Expiration;
        }
    }

}
