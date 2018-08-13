using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using iCoursework.Data;
using iCoursework.Models;
using iCoursework.Services;
using Microsoft.AspNetCore.Authentication.Google;
using AspNet.Security.OAuth.Vkontakte;

namespace iCoursework
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
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddAuthentication()
                .AddFacebook(facebookOptions =>
                {
                    facebookOptions.AppId = "2008154512573596";
                    facebookOptions.AppSecret = "4ad3d51a343270e784a6311ef70b240a";
                })
                .AddGoogle(googleOptions =>
                {
                    googleOptions.ClientId = "955315305189-hkf4rt1neecpmuutauul0kbk5fc7c5uj.apps.googleusercontent.com";
                    googleOptions.ClientSecret = "vcw8-SyilAxkp-gZy4M_Zh3u";
                })
                .AddVkontakte(vkontakeOptions =>
                {
                    vkontakeOptions.ClientId = "6658943";
                    vkontakeOptions.ClientSecret = "jJWD6MA7kGfVFBuKUCzQ";
                    vkontakeOptions.Scope.Add("email");
                });

            // Add application services.
            services.AddTransient<IEmailSender, EmailSender>();

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
