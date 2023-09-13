using E_Commerce_ITI_Final_Project.Data;
using E_Commerce_ITI_Final_Project.Entities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce_ITI_Final_Project
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllersWithViews();
            builder.Services.AddDbContext<StoreContext>(options =>
            {
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            builder.Services.AddIdentity<Account, IdentityRole>(options =>
            {
                options.Password.RequiredLength = 6;
                options.Password.RequireDigit = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;
                options.Password.RequireLowercase = false;
            })
                .AddRoleManager<RoleManager<IdentityRole>>()
                .AddEntityFrameworkStores<StoreContext>()
                .AddDefaultTokenProviders();

            builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme);

            var app = builder.Build();

            using var scope = app.Services.CreateScope();
            var services = scope.ServiceProvider;
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

            if (!roleManager.Roles.Any())
            {
                await roleManager.CreateAsync(new IdentityRole("Seller"));
                await roleManager.CreateAsync(new IdentityRole("User"));
            }

            var storeContext = services.GetRequiredService<StoreContext>();

            if (!storeContext.Categories.Any())
            {
                await storeContext.Categories.AddRangeAsync(
                    new Category() { Name = "Laptops" },
                    new Category() { Name = "Tablets" },
                    new Category() { Name = "Cameras" },
                    new Category() { Name = "Mobiles" },
                    new Category() { Name = "TVs" },
                    new Category() { Name = "Headphones" },
                    new Category() { Name = "Accessories" }
                );
                await storeContext.SaveChangesAsync();
            }

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}"
            );

            app.Run();
        }
    }
}