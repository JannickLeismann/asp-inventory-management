using InventoryManagement.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagement
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<ApplicationDbContext>(
                options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
            );

            builder.Services.AddDefaultIdentity<IdentityUser>
                (options => options.SignIn.RequireConfirmedAccount = false)
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<ApplicationDbContext>();

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var services = scope.ServiceProvider;

                var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
                var userManager = services.GetRequiredService<UserManager<IdentityUser>>();

                var roleCheck = await roleManager.RoleExistsAsync("Admin");

                if(roleCheck == false)
                {
                    await roleManager.CreateAsync(new IdentityRole("Admin"));
                }

                var adminUser = await userManager.FindByEmailAsync("admin@admin.de");

                if(adminUser == null)
                {
                    var user = new IdentityUser
                    {
                        UserName = "admin@admin.de",
                        Email = "admin@admin.de",
                        EmailConfirmed = true
                    };

                    var createUserResult = await userManager.CreateAsync(user, "Admin123.");

                    if (createUserResult.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, "Admin");
                    }

                }
            }

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

            app.UseAuthorization();

            app.MapRazorPages();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Items}/{action=Overview}");

            app.Run();
        }
    }
}