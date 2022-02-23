using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Text;
using Microsoft.AspNetCore.Identity;
using ExpenseTracker.Identity.Entities;
using ExpenseTracker.Identity.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using ExpenseTracker.Identity.DbContexts;

namespace ExpenseTracker.Identity
{
    public static class RegisterServices
    {
        public static IServiceCollection AddIdentityServices(this IServiceCollection services, IConfiguration Configuration)
        {
            var serverVersion = new MySqlServerVersion(new Version(8, 0, 25));
            services.AddDbContext<IdentityContext>(options => { options.UseMySql(Configuration["ConnectionStrings:ExpenseTestDatabase"], serverVersion); });
            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
                    {
                        options.Password.RequireNonAlphanumeric = false;
                        options.Password.RequireUppercase = false;
                        options.User.RequireUniqueEmail = true;
                    })
                    .AddEntityFrameworkStores<IdentityContext>();

            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    //options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters.ValidateIssuer = true;
                    options.TokenValidationParameters.ValidateAudience = true;
                    options.TokenValidationParameters.ValidIssuer = Configuration["JwtConfiguration:Issuer"];
                    options.TokenValidationParameters.ValidAudience = Configuration["JwtConfiguration:Audience"];
                    options.TokenValidationParameters.ValidateIssuerSigningKey = true;
                    options.SaveToken = false;
                    options.TokenValidationParameters.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(@Configuration["JwtConfiguration:Secret"]));
                });

            services.AddAuthorization(options =>
            {
                options.AddPolicy("FirstNamePolicy", policy => policy.RequireClaim("FirstName"));
                options.AddPolicy("LastNamePolicy", policy => policy.RequireClaim("LastName"));
            });
            services.AddScoped<UserManager<ApplicationUser>>();
            services.AddScoped<IIdentiyUserService, IdentityUserService>();
            return services;
        }
    }
}
