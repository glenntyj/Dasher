using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Rotativa.AspNetCore;

namespace Dasher
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            //Use "UserSecurity" authentication scheme
            services.AddMvc(options => options.EnableEndpointRouting = false);
            services
                  .AddAuthentication("UserSecurity")
                  .AddCookie("UserSecurity",
                     options =>
                     {
                         //Configure paths to Account controller
                         options.LoginPath = "/Account/Login/";
                         options.AccessDeniedPath = "/Account/Forbidden/";
                     });
        }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseStaticFiles();
            app.UseMvc(
               routes =>
               {
                   routes.MapRoute(
                    name: "default",
                    //template: "{controller=Account}/{action=Login}/{id?}");
                    template: "{controller=Web}/{action=Index}/{id?}");
               });
        }
    }
}
