using IS.API;
using Microsoft.AspNetCore.Mvc.Testing;
using Newtonsoft.Json;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;


namespace BFF.Test
{

    public class IdentityServiceTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly WebApplicationFactory<Startup> _factory;

        public IdentityServiceTests(WebApplicationFactory<Startup> factory)
        {
            _factory = factory;
        }

        [Fact]
        public async Task Authenticate_ReturnsValidToken()
        {
            // Arrange
            var client = _factory.CreateClient();
            var requestBody = new
            {
                Username = "example_user",
                Password = "your_password"
            };

            var content = new StringContent(JsonConvert.SerializeObject(requestBody), Encoding.UTF8, "application/json");

            // Act
            var response = await client.PostAsync("/api/authentication/token", content);
            response.EnsureSuccessStatusCode();
            var token = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.False(string.IsNullOrWhiteSpace(token));
            // Add more assertions as needed
        }

        [Fact]
        public async Task RevokeToken_ReturnsSuccess()
        {
            // Arrange
            var client = _factory.CreateClient();
            var requestContent = new StringContent("YourRequestBodyContent");

            // Act
            var response = await client.PostAsync("/api/authentication/revoke", requestContent);
            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadAsStringAsync();

            // Assert
            Assert.Equal("Success", result);
            // Add more assertions as needed
        }
    }

}