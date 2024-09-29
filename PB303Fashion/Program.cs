using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authentication.Cookies;
using NuGet.ContentModel;
using PB303Fashion.DataAccessLayer;
using PB303Fashion.Models;
using static System.Net.Mime.MediaTypeNames;
using System.Reflection;
using PB303Fashion.DataAccessLayer.Entities;
using Microsoft.AspNetCore.Identity;

namespace PB303Fashion
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();

            builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("Default")));

            builder.Services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(10);
            });
            builder.Services.AddIdentity<AppUser,IdentityRole<int>>(options =>
            {
                options.Password.RequiredLength = 3;
                options.Password.RequireUppercase=false;
                options.Password.RequireLowercase=false;
                options.Password.RequireNonAlphanumeric=false;
                options.Lockout.MaxFailedAccessAttempts = 4;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(3);
                options.User.RequireUniqueEmail=true;

            }).AddEntityFrameworkStores<AppDbContext>().AddDefaultTokenProviders();
            Constants.TopTrendingImagePath = Path.Combine(builder.Environment.WebRootPath, "assets", "images", "fashion" , "home-banner");

            var app = builder.Build();
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = GoogleDefaults.AuthenticationScheme;
            })
                .AddCookie()
                .AddGoogle(googleOptions =>{
                googleOptions.ClientId = builder.Configuration["Authentication:Google:ClientId"];
                googleOptions.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
            });
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSession();
          
            app.UseRouting();
            app.UseAuthentication();
                
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "areas",
                    pattern: "{area:exists}/{controller=TopTrending}/{action=Index}/{id?}");

                app.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });           

            app.Run();
        }
    }
}