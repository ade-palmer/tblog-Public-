using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TBlog.Core.Contexts;
using TBlog.Core.Entities;
using TBlog.Core.Models;
using TBlog.Core.Services;

namespace TBlog.Core
{
    public class Startup
    {
        // Branch
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            // Developer Database connection
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("LocalDbConnection")));

            // Release Database connection
            //services.AddDbContext<ApplicationDbContext>(options =>
            //   options.UseSqlServer(Configuration.GetConnectionString("ReleaseConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>(config =>
            {
                config.SignIn.RequireConfirmedEmail = true;
                config.User.RequireUniqueEmail = true;
                config.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(5);
                config.Lockout.MaxFailedAccessAttempts = 5;
                config.Lockout.AllowedForNewUsers = true;
            })
            .AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            services.Configure<BlogSettings>(Configuration.GetSection("BlogSettings"));
            services.Configure<EmailSettings>(Configuration.GetSection("EmailSettings"));

            // As the AddDbContext is registered as Scoped, the Blog Repository should also as issues will arise if Singleton or transient used
            services.AddScoped<IBlogRepository, BlogRepository>();

            services.AddSingleton<IEmailSender, EmailSender>();

            services.AddMvc(options =>
            {
                // Filter used instead of [ValidateAntiForgeryToken] on all Post Methods
                options.Filters.Add(new AutoValidateAntiforgeryTokenAttribute());
            });
        }


        public void Configure(IApplicationBuilder app,  IHostingEnvironment env, UserManager<ApplicationUser> _userManager, RoleManager<IdentityRole> _roleManager, ApplicationDbContext _dataManager)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");  // This detects exceptions
                app.UseStatusCodePagesWithReExecute("/Error/{0}"); // This detects bad routes

                // Basic view of error code
                //app.UseStatusCodePages();

                app.UseHsts();
            }

            // Seed Database for Development
            // Use ModelBuilder instead - Keep for knowledge reminder
            //_dataManager.ClearSeededData();
            //_userManager.ClearSeededUsers();
            //_roleManager.CreateSeedRoles();
            //_userManager.CreateSeedUsers();
            //_dataManager.CreateSeedData();

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Blog}/{action=Index}/{id?}");

                routes.MapRoute(
                    name: "post",
                    template: "post/{slug?}",
                    defaults: new { controller = "Blog", action = "Post" });

                routes.MapRoute(
                    name: "paging",
                    template: "page/{currentpage?}",
                    defaults: new { controller = "Blog", action = "Index" });
            });
        }
    }
}
