using IS.Domain.SeedWork;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IS.Domain.AggregatesModel.UserAggregate
{

    public class User: IdentityUser<Guid>, IAggregateRoot
    {
        public List<Role> Roles { get; set; }
        public List<Token> Tokens { get; set; }
        public List<Service> Services { get; set; }

        public User()
        {
            Roles = new List<Role>();
            Tokens = new List<Token>();
            Services = new List<Service>();
        }

        public void AddRole(Role role)
        {
            Roles.Add(role);
        }

        public void AddToken(Token token)
        {
            Tokens.Add(token);
        }

        public void AddService(Service service)
        {
            Services.Add(service);
        }
    }



}
