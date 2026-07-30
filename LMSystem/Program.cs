using LMSystem.Data;
using LMSystem.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<LibraryContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// Identity DB
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("SQLServerIdentityConnection")));

// Identity
builder.Services.AddIdentity<IdentityUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

var app = builder.Build();

// Middleware
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
    pattern: "{controller=Home}/{action=Index}/{id?}");


// 🔥 SAFE ROLE + ADMIN CREATION
using (var scope = app.Services.CreateScope())
{
    try
    {
        var roleManager = scope.ServiceProvider
            .GetRequiredService<RoleManager<IdentityRole>>();

        var userManager = scope.ServiceProvider
            .GetRequiredService<UserManager<IdentityUser>>();

        // Create roles
        string[] roles = { "Admin", "Librarian", "Member" };

        foreach (var role in roles)
        {
            if (!roleManager.RoleExistsAsync(role).Result)
            {
                roleManager.CreateAsync(new IdentityRole(role)).Wait();
            }
        }

        // Create admin
        string adminEmail = "admin@lms.com";
        string adminPassword = "Admin@123";

        var user = userManager.FindByEmailAsync(adminEmail).Result;

        if (user == null)
        {
            var newUser = new IdentityUser
            {
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };

            var result = userManager.CreateAsync(newUser, adminPassword).Result;

            if (result.Succeeded)
            {
                userManager.AddToRoleAsync(newUser, "Admin").Wait();
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine("ERROR: " + ex.Message);
    }
}

// 🚀 KEEP THIS LAST
app.Run();