using LibraryManagementSystem.Models;
using LibraryManagementSystem.Service;
using LibraryManagementSystem.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using LibraryManagementSystem.Hubs;
using Microsoft.AspNetCore.SignalR;

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.StaticFiles;
using OfficeOpenXml;

namespace LibraryManagementSystem
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

            ExcelPackage.License.SetNonCommercialOrganization("Bumbe Library");
            services.AddDbContext<LibraryDbContext>(options =>
           options.UseSqlServer(Configuration.GetConnectionString("LibraryDbContext")));

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
            {
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
            }
                )
                .AddEntityFrameworkStores<LibraryDbContext>()
                .AddDefaultTokenProviders();
            services.AddScoped<MyService>();
            services.AddScoped<IEmailSender, EmailSender>();
            services.AddHttpClient<DarajaService>();
            services.AddScoped<ISmsService, SmsService>();
            services.AddScoped<IRecommendationService, RecommendationService>();


            services.ConfigureApplicationCookie(options =>
            {
                
                options.LoginPath = "/Account/Login";
                options.LogoutPath = "/Account/Logout";
                options.AccessDeniedPath = "/Account/AccessDenied";
               

            });


            services.AddMvc(options => {

                options.EnableEndpointRouting = false;

            }).AddXmlSerializerFormatters();
            services.AddControllersWithViews();
            services.AddSignalR();


        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var services = scope.ServiceProvider;
                var dbContext = scope.ServiceProvider.GetRequiredService<LibraryDbContext>();

                dbContext.Database.Migrate();
                IdentitySeed.SeedIdentityRoles(services).GetAwaiter().GetResult();
                var provider = new FileExtensionContentTypeProvider();
                provider.Mappings[".mjs"] = "application/javascript";

                app.UseStaticFiles(new StaticFileOptions
                {
                    ContentTypeProvider = provider
                });

            }
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            if (!env.IsDevelopment())
            {
                app.UseHttpsRedirection();
            }

            app.UseStaticFiles();


            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");

                endpoints.MapHub<CollaborationHub>("/collaborationHub");

            });

        }
    }
}
