using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IS.Domain.AggregatesModel.UserAggregate
{
    public class Role: IdentityRole<Guid>
    {
        public User User { get; set; }
        public Guid UserId { get; set; }

        public Role(string name)
        {
            Name = name;
        }
    }

}
