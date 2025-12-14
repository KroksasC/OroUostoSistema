using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OroUostoSystem.Server.Data;
using OroUostoSystem.Server.DbInitializer;
using OroUostoSystem.Server.Interfaces;
using OroUostoSystem.Server.Models;
using OroUostoSystem.Server.Services;
using System.Security.Claims;


using System.Text;

namespace OroUostoSystem.Server.Extensions
{
    public static class ApplicationServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {
            var connectionString = config.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

            services.AddDbContext<DataContext>(options =>
            {
                options.UseSqlite(connectionString);
            });

            services.AddDistributedMemoryCache();

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(30);
                options.Cookie.HttpOnly = true;
                options.Cookie.IsEssential = true;
            });

            services.AddIdentity<User, IdentityRole>()
                .AddEntityFrameworkStores<DataContext>()
                .AddDefaultTokenProviders();
                
            services.ConfigureApplicationCookie(options =>
            {
                options.Events.OnRedirectToLogin = context =>
                {
                    if (context.Request.Path.StartsWithSegments("/api"))
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        return Task.CompletedTask;
                    }

                    context.Response.Redirect(context.RedirectUri);
                    return Task.CompletedTask;
                };
            });


            services.AddControllers();
            services.AddCors();
            services.AddScoped<IDbinitializer, DBInitializer>();
            services.AddScoped<ITokenService, TokenService>();

            //Ensure the token key is configured
            var secretKey = config["AppSettings:TokenKey"]
                ?? throw new InvalidOperationException("Token key is not configured.");

            //Config jwt authentication

            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        RoleClaimType = ClaimTypes.Role
                    };
                });

            return services;
        }
    }
}
