using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IS.Domain.AggregatesModel.UserAggregate
{
    public class Service
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public User User { get; set; }
        public Guid UserId { get; set; }

        public Service(string name)
        {
            Name = name;

        }
    }

}
