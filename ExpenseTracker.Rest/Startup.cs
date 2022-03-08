using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ExpenseTracker.Core.Repositories;
using ExpenseTracker.Persistence.DbContexts;
using Microsoft.EntityFrameworkCore;
using ExpenseTracker.Persistence.Repositories;
using ExpenseTracker.Core.Services;
using ExpenseTracker.Rest.Configuration;
using Microsoft.OpenApi.Models;
using ExpenseTracker.Identity.Infrastructure;
using ExpenseTracker.Rest.Middlewares;

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

            services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme() 
                { 
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "JWT Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT",
                    Scheme = "bearer"
                });
                options.AddSecurityRequirement(new OpenApiSecurityRequirement()
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

            // Mapper.Initialize(cgf => cfg => { AddExpressionMapping(); });

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            var serverVersion = new MySqlServerVersion(new Version(8, 0, 25));
            services.AddDbContext<ExpenseDbContext>(options => { options.UseMySql(Configuration["ConnectionStrings:ExpenseTestDatabase"], serverVersion); } );
            services.AddIdentityServices(Configuration);

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
                app.UseDeveloperExceptionPage();
            else
                app.UseExceptionsToResponse();

            app.UseHttpsRedirection();

            app.UseSwagger();

            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "ExpenseTracker API V1");
            });

            app.UseRouting();

            app.UseCors(_myAllowSpecificOrigins);

            app.UseAuthentication();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
