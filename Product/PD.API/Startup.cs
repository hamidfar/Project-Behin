using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using PD.API.Application.Commands;
using PD.API.Application.Queries;
using PD.API.Middlewares;
using PD.Domain.AggregatesModel.ProductAggregate;
using PD.Infrastructure.Data;
using PD.Infrastructure.Repositories;
using PD.Middlewares;
using System.Reflection;

namespace PD.API
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
            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

            services.AddTransient<IRequestHandler<CreateProductCommand, Product>, CreateProductCommandHandler>();
            services.AddTransient<IRequestHandler<GetProductsQuery, List<Product>>, GetProductsQueryHandler>();

            services.AddDbContext<ProductDbContext>(options =>
            options.UseSqlServer(_configuration["ConnectionString"]));


            services.AddCors(options =>
            {
                options.AddPolicy("AllowGateway",
                    builder =>
                    {
                        builder.WithOrigins("http://localhost:3000") 
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
            });



            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.Authority = "http:/localhost:5000";
                    options.Audience = "PD_API";
                    options.RequireHttpsMetadata = false;
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateAudience = false,
                        ValidateIssuer = false,
                        ValidateIssuerSigningKey = true,
                        RequireSignedTokens = true,
                        RequireExpirationTime = true,
                        ValidateLifetime = true
                    };
                    options.Configuration = new OpenIdConnectConfiguration();
                });


            services.AddAuthorization(options =>
            {
                options.AddPolicy("Service-PD", policy =>
                {
                    policy.RequireAuthenticatedUser();
                    policy.RequireClaim("services", "PD");
                });
            });




            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Product API", Version = "v1" });
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


            services.AddScoped<IProductRepository, ProductRepository>();


            services.AddControllers();

        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {

            app.UseMiddleware<ExceptionMiddleware>();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Product API V1");
                c.RoutePrefix = string.Empty;

                c.DocumentTitle = "Product API";
                c.OAuthClientId("pd-swagger-client");
                c.OAuthAppName("PD Swagger Client");
            });


            app.UseRouting();
            app.UseMiddleware<JwtAuthenticationMiddleware>();


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
