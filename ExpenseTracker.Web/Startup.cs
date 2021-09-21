using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using ExpenseTracker.Core.Repositories;
using ExpenseTracker.Core.Services;
using ExpenseTracker.Persistence.DbContexts;
using ExpenseTracker.Persistence.Repositories;

namespace ExpenseTracker.Web
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // services.AddControllersWithViews(options => 
            // {
            //     options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
            // });
            services.AddControllersWithViews();

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            var serverVersion = new MySqlServerVersion(new Version(8, 0, 25));
            services.AddDbContext<ExpenseDbContext>(options => { options.UseMySql(Configuration["ConnectionStrings:ExpenseTestDatabase"], serverVersion); } );

            services.AddTransient<IExpenseRepository, ExpenseRepository>();
            services.AddTransient<ICategoryRepository, CategoryRepository>();
            services.AddTransient<IExpenseService, ExpenseService>(); 
            services.AddTransient<ICategoryService, CategoryService>(); 
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
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
