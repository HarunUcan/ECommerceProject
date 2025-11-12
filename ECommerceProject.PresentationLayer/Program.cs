using ECommerceProject.BusinessLayer.Abstract;
using ECommerceProject.BusinessLayer.Concrete;
using ECommerceProject.DataAccessLayer.Abstract;
using ECommerceProject.DataAccessLayer.Concrete;
using ECommerceProject.DataAccessLayer.EntityFramework;
using ECommerceProject.EntityLayer.Concrete;
using ECommerceProject.PresentationLayer.Middlewares;
using ECommerceProject.PresentationLayer.Models;
using ECommerceProject.PresentationLayer.Options;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Localization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Globalization;

namespace ECommerceProject.PresentationLayer
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Kültürü Ýngilizce (ABD) olarak ayarla
            var cultureInfo = new CultureInfo("en-US");
            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

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

            builder.Services.Configure<AdminAccountOptions>(builder.Configuration.GetSection("AdminAccount"));
            builder.Services.Configure<Iyzipay.Options>(builder.Configuration.GetSection("Iyzico"));

            builder.Services.Configure<DataProtectionTokenProviderOptions>(options =>
            {
                options.TokenLifespan = TimeSpan.FromDays(2);
            });
            builder.Services.AddScoped<IAdressService, AdressManager>();
            builder.Services.AddScoped<IAdressDal, EfAdressDal>();

            builder.Services.AddScoped<ICategoryService, CategoryManager>();
            builder.Services.AddScoped<ICategoryDal, EfCategoryDal>();

            builder.Services.AddScoped<IProductService, ProductManager>();
            builder.Services.AddScoped<IProductDal, EfProductDal>();

            builder.Services.AddScoped<IProductImageService, ProductImageManager>();
            builder.Services.AddScoped<IProductImageDal, EfProductImageDal>();

            builder.Services.AddScoped<IProductGroupService, ProductGroupManager>();
            builder.Services.AddScoped<IProductGroupDal, EfProductGroupDal>();

            builder.Services.AddScoped<ICouponService, CouponManager>();
            builder.Services.AddScoped<ICouponDal, EfCouponDal>();

            builder.Services.AddScoped<ICartService, CartManager>();
            builder.Services.AddScoped<ICartDal, EfCartDal>();

            builder.Services.AddScoped<IStaticPageService, StaticPageManager>();
            builder.Services.AddScoped<IStaticPageDal, EfStaticPageDal>();

            builder.Services.AddScoped<ISaleService, SaleManager>();
            builder.Services.AddScoped<ISaleDal, EfSaleDal>();

            builder.Services.AddScoped<IAppUserService, AppUserManager>();
            builder.Services.AddScoped<IAppUserDal, EfAppUserDal>();

            builder.Services.AddScoped<IMailSenderService, MailSenderManager>();


            builder.Services.ConfigureApplicationCookie(options =>
            {
                options.LoginPath = "/User/Login/Index";
                options.LogoutPath = "/Account/Logout";
                options.AccessDeniedPath = "/User/Home/NotFound";

                options.Events = new CookieAuthenticationEvents
                {
                    // Giriþ yapmamýþ kullanýcý admin sayfasýna eriþmeye çalýþýrsa
                    OnRedirectToLogin = context =>
                    {
                        if (context.Request.Path.StartsWithSegments("/Admin"))
                        {
                            context.Response.Redirect("/User/Home/NotFound");
                            return Task.CompletedTask;
                        }

                        // Diðer sayfalar için normal login yönlendirmesi
                        context.Response.Redirect(context.RedirectUri);
                        return Task.CompletedTask;
                    },

                    // Giriþ yapmýþ ama yetkisi olmayan kullanýcý admin sayfasýna eriþmeye çalýþýrsa
                    OnRedirectToAccessDenied = context =>
                    {
                        if (context.Request.Path.StartsWithSegments("/Admin"))
                        {
                            // admin sayfalarýnýn ifþa olmamasý için returnUrl olmadan yönlendirme
                            context.Response.Redirect("/User/Home/NotFound");
                            return Task.CompletedTask;
                        }

                        // Diðer eriþim reddi yönlendirmeleri
                        context.Response.Redirect(context.RedirectUri);
                        return Task.CompletedTask;
                    }
                };
            });


            var app = builder.Build();

            // Kültürü HTTP isteðine göre zorla
            var supportedCultures = new[] { cultureInfo };
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture(cultureInfo),
                SupportedCultures = supportedCultures,
                SupportedUICultures = supportedCultures
            });

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

            // Middleware’i ekle
            app.UseMiddleware<CheckUserMiddleware>();
            app.UseMiddleware<RateLimitMiddleware>();

            //app.UseEndpoints(endpoints =>
            //{


            //    endpoints.MapDefaultControllerRoute();
            //});

            //Admin Routes
            //app.MapControllerRoute(
            //    name: "admin_default",
            //    pattern: "admin",
            //    defaults: new { area = "Admin", controller = "Dashboard", action = "Index" });

            //app.MapControllerRoute(
            //    name: "admin_route",
            //    pattern: "admin/{controller}/{action}/{id?}",
            //    defaults: new { area = "Admin", controller = "Dashboard", action = "Index" });

            //User Routes
            app.MapControllerRoute(
                name: "categoryRoute",
                pattern: "{slug}",
                defaults: new { area = "User", controller = "Home", action = "Category" });

            app.MapControllerRoute(
                name: "productDetailRoute",
                pattern: "urun/{slug}",
                defaults: new { area = "User", controller = "Home", action = "ProductDetail" });


            app.MapControllerRoute(
                   name: "areas",
                   pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

            app.MapControllerRoute(
                name: "default",
                pattern: "{area=User}/{controller=Home}/{action=Index}/{id?}");



            // Roller ve admin kullanýcýsýný oluþtur
            using (var scope = app.Services.CreateScope())
            {
                var serviceProvider = scope.ServiceProvider;
                var roleManager = serviceProvider.GetRequiredService<RoleManager<AppRole>>();
                var userManager = serviceProvider.GetRequiredService<UserManager<AppUser>>();
                var adminAccountOptions = serviceProvider.GetRequiredService<IOptions<AdminAccountOptions>>().Value;

                // 1. Roller varsa oluþtur
                foreach (var role in Enum.GetValues(typeof(UserRoles)))
                {
                    string roleName = role.ToString();
                    if (!await roleManager.RoleExistsAsync(roleName))
                    {
                        await roleManager.CreateAsync(new AppRole { Name = roleName });
                    }
                }

                // 2. Admin kullanýcýyý oluþtur
                string? adminEmail = adminAccountOptions.Email;
                string? adminPassword = adminAccountOptions.Password;
                string? adminPasswordHash = adminAccountOptions.PasswordHash;

                if (!string.IsNullOrWhiteSpace(adminEmail))
                {
                    var existingAdmin = await userManager.FindByEmailAsync(adminEmail);
                    if (existingAdmin == null)
                    {
                        var adminUser = new AppUser
                        {
                            Name = "Admin",
                            Surname = "1",
                            UserName = adminEmail,
                            Email = adminEmail,
                            EmailConfirmed = true
                        };

                        IdentityResult result;

                        if (!string.IsNullOrWhiteSpace(adminPassword))
                        {
                            result = await userManager.CreateAsync(adminUser, adminPassword);
                        }
                        else if (!string.IsNullOrWhiteSpace(adminPasswordHash))
                        {
                            adminUser.PasswordHash = adminPasswordHash;
                            result = await userManager.CreateAsync(adminUser);
                        }
                        else
                        {
                            result = IdentityResult.Failed(new IdentityError
                            {
                                Code = "AdminPasswordMissing",
                                Description = "Admin password configuration is missing."
                            });
                        }

                        if (result.Succeeded)
                        {
                            await userManager.AddToRoleAsync(adminUser, UserRoles.Admin.ToString());
                        }
                    }
                }
            }


            app.Run();
        }
    }
}
