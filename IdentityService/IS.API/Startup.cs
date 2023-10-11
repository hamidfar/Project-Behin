
using IdentityServer4.Models;
using IS.API.Application.DTO;
using IS.API.Infrastructure.Services;
using IS.API.Middlewares;
using IS.Domain.AggregatesModel.TokenBlacklistAggregate;
using IS.Domain.AggregatesModel.UserAggregate;
using IS.Infrastructure.Data;
using IS.Infrastructure.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;


namespace IS.API
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<IdentityDbContext>(options =>
            options.UseSqlServer(_configuration["ConnectionString"]));

            //builder.WithOrigins("http://localhost:3000") 

            services.AddCors(options =>
            {
                options.AddPolicy("AllowGateway",
                    builder =>
                    {
                        builder.AllowAnyOrigin()
                       .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
            });
            services.AddIdentityServer()
             .AddInMemoryClients(new List<Client>
             {
                new Client
                {
                    ClientId = "pd-api",
                    ClientName = "Product API",
                    AllowedGrantTypes = { "client_credentials", "password" },
                    ClientSecrets = { new Secret("pd-secret".Sha256()) },
                    AllowedScopes = { "pd-api" }
                },
                new Client
                {
                    ClientId = "pd-swagger-client",
                    ClientName = "PD Swagger Client",
                    AllowedGrantTypes = { "implicit" },
                    AllowAccessTokensViaBrowser = true,
                    RedirectUris = { "http://localhost:6001/swagger/oauth2-redirect.html" },
                    PostLogoutRedirectUris = { "http://localhost:6001/swagger/oauth2-redirect.html" },
                    AllowedCorsOrigins = { "http://localhost:6001" },
                    AllowedScopes = { "pd-api" }
                }
             })
             .AddInMemoryApiResources(new List<ApiResource>
             {
                new ApiResource("pd-api", "Product API")
             })
             .AddInMemoryApiScopes(new List<ApiScope>
             {
                new ApiScope("pd-api", "Product API")
             })
             .AddInMemoryPersistedGrants();


            services
                .AddDefaultIdentity<User>(options =>
                {
                    options.SignIn.RequireConfirmedAccount = false;
                    options.SignIn.RequireConfirmedEmail = false;
                    options.SignIn.RequireConfirmedPhoneNumber = false;
                })
                .AddEntityFrameworkStores<IdentityDbContext>()
                .AddSignInManager<SignInManager<User>>()
                .AddDefaultTokenProviders();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Identity API", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme",
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    new string[] {}
                }
            });
            });


            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<ITokenBlacklistRepository, TokenBlacklistRepository>();

            services.AddScoped<IAuthenticateService, AuthenticateService>();
            services.AddScoped<ITokenBlacklistService, TokenBlacklistService>();
            services.AddScoped<ITokenService, TokenService>();


            services.AddControllers();

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            app.UseMiddleware<ExceptionMiddleware>();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Identity Service API V1");
                c.RoutePrefix = string.Empty;

                c.DocumentTitle = "Identity Service API";
                c.OAuthClientId("swagger-ui");
                c.OAuthAppName("Identity Service API - Swagger UI");
            });

            app.UseRouting();
            app.UseCors("AllowGateway");

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
