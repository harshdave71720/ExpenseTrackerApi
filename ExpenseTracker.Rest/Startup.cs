using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ExpenseTracker.Core.Repositories;
using ExpenseTracker.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;
using ExpenseTracker.Persistence.Repositories;
using Pomelo.EntityFrameworkCore.MySql;
using AutoMapper;
using ExpenseTracker.Core.Services;
using ExpenseTracker.Rest.Configuration;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Identity;

namespace ExpenseTracker.Rest
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        private readonly string _myAllowSpecificOrigins = "MyAllowSpecificOrigins";

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<JwtConfiguration>(Configuration.GetSection("JwtConfiguration"));

            services.AddCors(options => {
                options.AddPolicy(name: _myAllowSpecificOrigins,
                                    builder =>
                                    {
                                        builder.WithOrigins("http://localhost:4200");
										builder.AllowAnyMethod();
										builder.AllowAnyHeader();
                                    }
                                );
            });

            services.AddControllers();

            services.AddSwaggerGen();

            // Mapper.Initialize(cgf => cfg => { AddExpressionMapping(); });

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            var serverVersion = new MySqlServerVersion(new Version(8, 0, 25));
            services.AddDbContext<ExpenseDbContext>(options => { options.UseMySql(Configuration["ConnectionStrings:ExpenseTestDatabase"], serverVersion); } );

            services.AddTransient<IExpenseRepository, ExpenseRepository>();
            services.AddTransient<ICategoryRepository, CategoryRepository>();
            services.AddTransient<IUserRepository, UserRepository>();

            services.AddTransient<IExpenseService, ExpenseService>();  
            services.AddTransient<ICategoryService, CategoryService>();
            services.AddTransient<ITemplateService, TemplateService>();
            services.AddTransient<IUserService, UserService>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/error");
            }

            app.UseHttpsRedirection();

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "ExpenseTracker API V1");
            });

            app.UseRouting();

            app.UseCors(_myAllowSpecificOrigins);

            // app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
