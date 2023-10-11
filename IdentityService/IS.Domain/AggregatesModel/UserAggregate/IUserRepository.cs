using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IS.Domain.AggregatesModel.UserAggregate
{
    public interface IUserRepository
    {
        Task<User> GetById(Guid userId);
        Task<User> GetUserByUsername(string userName);
        void AddToken(Guid userId, Token token);
        void AddRole(Guid userId, Role role);
        void AddService(Guid userId, Service service);
    }
}
