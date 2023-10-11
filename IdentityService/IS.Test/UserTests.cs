using IS.Domain.AggregatesModel.UserAggregate;
using Microsoft.AspNetCore.Identity;
using Moq;
using System;
using System.Collections.Generic;
using Xunit;

namespace IS.Test
{
    public class UserTests
    {
        [Fact]
        public void AddRole_Should_AddRoleToUser()
        {
            var user = new User();
            var role = new Role("UserRole");

            user.AddRole(role);

            Assert.Contains(role, user.Roles);
        }

        [Fact]
        public void AddToken_Should_AddTokenToUser()
        {
            var user = new User();
            var token = new Token("your_token_value", DateTime.UtcNow);

            user.AddToken(token);

            Assert.Contains(token, user.Tokens);
        }

        [Fact]
        public void AddService_Should_AddServiceToUser()
        {
            var user = new User();
            var service = new Service("ServiceName");

            user.AddService(service);

            Assert.Contains(service, user.Services);
        }
    }


}
