using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using MVCWithBlazor.Data;
using MVCWithBlazor.Services;

namespace MVCWithBlazor
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
            services.AddSingleton<Auxiliar>(); // Added by me for Test purpose
            // @"Server=172.16.4.165\SQLEXPRESS;Database=Don_DashboardReports;User Id=user; Password=Calarasi81; MultipleActiveResultSets=true;")
            services.AddDbContext<ReportDbContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("DefaultConnection")));

            services.AddIdentity<IdentityUser, IdentityRole>().AddEntityFrameworkStores<ReportDbContext>().AddDefaultTokenProviders(); // Add Identity 
            // Set different options for Identity
            services.Configure<IdentityOptions>(options =>
            {
                options.Password.RequiredLength = 4;
                options.Password.RequireUppercase = false;
                options.Password.RequireDigit = true;
                options.Password.RequireNonAlphanumeric = false;

                options.Lockout.MaxFailedAccessAttempts = 3; // After 3 attempts lock account
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10); // for a period of time

                //options.SignIn.RequireConfirmedEmail = true; // Nedded to confirm email adress acount
            });
            services.ConfigureApplicationCookie(options => // Set authentification to redirect to login and accessdenied actions 
            {
                options.LoginPath = "/Identity/Signin";
                options.AccessDeniedPath = "/Identity/AccessDenied";
                options.ExpireTimeSpan = TimeSpan.FromDays(1);
            });
            services.AddRazorPages(); // Added for Razor Pages
            services.AddServerSideBlazor(); // Added for Blazor

            services.Configure<SmtpOptions>(Configuration.GetSection("Smtp")); // Load Options for Mail sender
            services.AddSingleton<IEmailSender, SmtpEmailSender>();
            services.AddAuthorization(option =>
            {
                option.AddPolicy("Dep", p =>
                {
                    // You can put only type of claim
                    // If you want values of claim add them after comma
                    p.RequireClaim("Department", "Mentenanta", "Calitate", "tech"); // You can add also role: .RequireRole("Member");
                });
                // You can add more policies

            });
            services.AddControllersWithViews(); // Add MVC
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MjY3Mzg2QDMxMzgyZTMxMmUzMFZpbGFaRlptWjVMa25ONnpsOHozZFkvZmVXbXh4OGViL2xoT2l0RDRxTE09;MjY3Mzg3QDMxMzgyZTMxMmUzMFdUcVYwSkRFcTM2R1pEMGJNWVB0V0dSYjRJMTBuSCtxTElnL1BzL3BCR1U9");
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
            app.UseAuthentication(); // Add Identity
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages(); // Razor pages
                endpoints.MapBlazorHub(); // Added for blazor
            });
        }
    }
}
