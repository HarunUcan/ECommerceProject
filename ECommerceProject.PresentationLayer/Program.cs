using ECommerceProject.BusinessLayer.Abstract;
using ECommerceProject.BusinessLayer.Concrete;
using ECommerceProject.DataAccessLayer.Abstract;
using ECommerceProject.DataAccessLayer.Concrete;
using ECommerceProject.DataAccessLayer.EntityFramework;
using ECommerceProject.EntityLayer.Concrete;
using ECommerceProject.PresentationLayer.Models;
using Microsoft.AspNetCore.Identity;

namespace ECommerceProject.PresentationLayer
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<Context>();
            builder.Services.AddIdentity<AppUser, AppRole>(options =>
            {
                options.Password.RequiredLength = 6;
                options.Password.RequireDigit = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultEmailProvider;
            }).AddEntityFrameworkStores<Context>().AddErrorDescriber<CustomIdentityValidator>().AddDefaultTokenProviders();

            builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
            {
                options.TokenLifespan = TimeSpan.FromDays(2);
            });
            builder.Services.AddTransient<IAdressService, AdressManager>();
            builder.Services.AddTransient<IAdressDal, EfAdressDal>();

            builder.Services.AddTransient<ICategoryService, CategoryManager>();
            builder.Services.AddTransient<ICategoryDal, EfCategoryDal>();

            builder.Services.AddTransient<IMailSenderService, MailSenderManager>();

            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/User/Login/Index";
                options.LogoutPath = "/Login/Logout";
                options.AccessDeniedPath = "/Login/AccessDenied";
            });
            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();    
            app.UseAuthorization();

            //app.UseEndpoints(endpoints =>
            //{
               

            //    endpoints.MapDefaultControllerRoute();
            //});

            app.MapControllerRoute(
                   name: "areas",
                   pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}
