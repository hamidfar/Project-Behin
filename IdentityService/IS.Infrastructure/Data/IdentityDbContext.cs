using IS.Domain.AggregatesModel.TokenBlacklistAggregate;
using IS.Domain.AggregatesModel.UserAggregate;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace IS.Infrastructure.Data
{
    public class IdentityDbContext : DbContext
    {
        private static readonly IPasswordHasher<User> _passwordHasher = new PasswordHasher<User>();

        public IdentityDbContext(DbContextOptions<IdentityDbContext> options)
            : base(options)
        {
        }


        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Token> Tokens { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<Service> Services { get; set; }
        public virtual DbSet<TokenBlacklist> TokenBlacklist { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>()
                .HasMany(u => u.Tokens)
                .WithOne(t => t.User)
                .HasForeignKey(t => t.UserId);

            modelBuilder.Entity<User>()
              .HasMany(u => u.Roles)
              .WithOne(t => t.User)
              .HasForeignKey(t => t.UserId);

            modelBuilder.Entity<User>()
            .HasMany(u => u.Services)
            .WithOne(t => t.User)
            .HasForeignKey(t => t.UserId);

        }

        public void SeedData()
        {

            var user = new User
            {
                Id = Guid.NewGuid(),
                UserName = "user",
                NormalizedUserName = "USER",
                Email = "user@example.com",
                NormalizedEmail = "USER@EXAMPLE.COM",
                EmailConfirmed = false,
            };
            user.PasswordHash = _passwordHasher.HashPassword(user, "1234");

            Role role = new Role("Admin");

            Service service = new Service("PD");

            user.AddRole(role);
            user.AddService(service);

            Users.Add(user);

            SaveChanges();
        }
    }
}
