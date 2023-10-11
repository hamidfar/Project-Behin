using IS.Domain.AggregatesModel.UserAggregate;
using IS.Infrastructure.Data;
using IS.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace IS.Test
{

    public class UserRepositoryTests
    {
        [Fact]
        public async Task GetById_ReturnsUser_WhenUserExists()
        {
            var userId = Guid.NewGuid();
            var user = new User { Id = userId, UserName = "testuser" };

            var options = new DbContextOptionsBuilder<IdentityDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new IdentityDbContext(options))
            {
                context.Users.Add(user);
                context.SaveChanges();
            }

            var dbContext = new IdentityDbContext(options);
            var userRepository = new UserRepository(dbContext);

            var result = await userRepository.GetById(userId);

            Assert.NotNull(result);
            Assert.Equal(userId, result.Id);
        }

        [Fact]
        public async Task GetUserByUsernameAsync_ValidUsername_ReturnsUser()
        {
            var userName = "testuser";
            var user = new User { Id = Guid.NewGuid(), UserName = userName };

            var options = new DbContextOptionsBuilder<IdentityDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new IdentityDbContext(options))
            {
                context.Users.Add(user);
                context.SaveChanges();
            }

            var dbContext = new IdentityDbContext(options);
            var userRepository = new UserRepository(dbContext);

            var result = await userRepository.GetUserByUsername(userName);

            Assert.NotNull(result);
            Assert.Equal(userName, result.UserName);
        }



        [Fact]
        public void AddToken_Should_Add_Token_To_User()
        {
            var options = new DbContextOptionsBuilder<IdentityDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new IdentityDbContext(options))
            {
                var userRepository = new UserRepository(context);

                var user = new User { Id = Guid.NewGuid() };
                context.Users.Add(user);
                context.SaveChanges();
                var token = new Token(Guid.NewGuid().ToString(), DateTime.UtcNow);

                userRepository.AddToken(user.Id, token);

                var updatedUser = context.Users
                    .Include(u => u.Tokens)
                    .SingleOrDefault(u => u.Id == user.Id);

                Assert.NotNull(updatedUser);
                Assert.Contains(token, updatedUser.Tokens);
            }
        }

        [Fact]
        public void AddRole_Should_Add_Role_To_User()
        {
            var options = new DbContextOptionsBuilder<IdentityDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new IdentityDbContext(options))
            {
                var userRepository = new UserRepository(context);

                var user = new User { Id = Guid.NewGuid() };
                context.Users.Add(user);
                context.SaveChanges();
                var role = new Role("admin");


                userRepository.AddRole(user.Id, role);

                var updatedUser = context.Users
                    .Include(u => u.Roles)
                    .SingleOrDefault(u => u.Id == user.Id);

                Assert.NotNull(updatedUser);
                Assert.Contains(role, updatedUser.Roles);
            }
        }

        [Fact]
        public void AddService_Should_Add_Service_To_User()
        {
            var options = new DbContextOptionsBuilder<IdentityDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            using (var context = new IdentityDbContext(options))
            {
                var userRepository = new UserRepository(context);

                var user = new User { Id = Guid.NewGuid() };
                context.Users.Add(user);
                context.SaveChanges();
                var service = new Service("PD");

                userRepository.AddService(user.Id, service);

                var updatedUser = context.Users
                    .Include(u => u.Services)
                    .SingleOrDefault(u => u.Id == user.Id);

                Assert.NotNull(updatedUser);
                Assert.Contains(service, updatedUser.Services);
            }

        }
        private static DbSet<T> MockDbSet<T>(IEnumerable<T> data) where T : class
        {
            var queryable = data.AsQueryable();
            var mockDbSet = new Mock<DbSet<T>>();
            mockDbSet.As<IQueryable<T>>().Setup(m => m.Provider).Returns(queryable.Provider);
            mockDbSet.As<IQueryable<T>>().Setup(m => m.Expression).Returns(queryable.Expression);
            mockDbSet.As<IQueryable<T>>().Setup(m => m.ElementType).Returns(queryable.ElementType);
            mockDbSet.As<IQueryable<T>>().Setup(m => m.GetEnumerator()).Returns(() => queryable.GetEnumerator());
            return mockDbSet.Object;
        }
    }
}
