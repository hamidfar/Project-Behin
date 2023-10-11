using IS.Domain.AggregatesModel.UserAggregate;
using IS.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;

namespace IS.Infrastructure.Repositories
{

    public class UserRepository:IUserRepository
    {
        private readonly IdentityDbContext _context;

        public UserRepository(IdentityDbContext context)
        {
            _context = context;
        }

        public async Task<User> GetById(Guid userId)
        {
            try
            {
                return await _context.Users
                .Include(u => u.Roles)
                .Include(u => u.Tokens)
                .Include(u => u.Services)
                .SingleOrDefaultAsync(u => u.Id == userId);
            }
            catch (Exception ex)
            {
                throw;
            }
           
        }

        public async Task<User> GetUserByUsername(string userName)
        {
            try
            {
                return await _context.Users
                .Include(u => u.Roles)
                .Include(u => u.Tokens)
                .Include(u => u.Services)
                .SingleOrDefaultAsync(u => u.UserName == userName);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public void AddToken(Guid userId, Token token)
        {
            var user = _context.Users.SingleOrDefault(u => u.Id == userId);
            if (user != null)
            {
                user.Tokens.Add(token);
                _context.SaveChanges();
            }
        }

        public void AddRole(Guid userId, Role role)
        {
            var user = _context.Users.SingleOrDefault(u => u.Id == userId);
            if (user != null)
            {
                user.Roles.Add(role);
                _context.SaveChanges();
            }
        }

        public void AddService(Guid userId, Service service)
        {
            var user = _context.Users.SingleOrDefault(u => u.Id == userId);
            if (user != null)
            {
                user.Services.Add(service);
                _context.SaveChanges();
            }
        }
    }

}
