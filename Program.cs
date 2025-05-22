using charac.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews(); // Only need to add this once

// Add DbContext to the container for your application's database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Identity services with Roles and token providers
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;  // Set this to true if you want email confirmation
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();  // Add token providers for things like password reset tokens

// Add authentication and authorization services
builder.Services.AddAuthentication()
    .AddCookie(options =>
    {
        options.LoginPath = "/Identity/Account/Login";  // Path to login page
        options.AccessDeniedPath = "/Identity/Account/AccessDenied";  // Path for access denied
    });

// Add Authorization policies (for roles)
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
    options.AddPolicy("UserPolicy", policy => policy.RequireRole("User"));
    options.AddPolicy("ManagerPolicy", policy => policy.RequireRole("Manager"));
});

// Add Razor Pages support for Identity pages like AccessDenied, Login, etc.
builder.Services.AddRazorPages();

builder.Services.AddHttpClient(); // Registers IHttpClientFactory
builder.Services.AddSingleton<IEmailSender, EmailSender>();

var app = builder.Build();

// Seed roles into the database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    await SeedRoles(roleManager); // Seed roles at app startup
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

// Use Authentication middleware
app.UseAuthentication(); // Enables authentication

// Use Authorization middleware
app.UseAuthorization();  // Enables authorization

// Add default route for controllers
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Add Razor Pages route for Identity pages (this is important for AccessDenied, Login, etc.)
app.MapRazorPages();

app.Run();

// Method to seed roles
async Task SeedRoles(RoleManager<IdentityRole> roleManager)
{
    string[] roleNames = { "Admin", "User", "Manager" };

    foreach (var roleName in roleNames)
    {
        var roleExist = await roleManager.RoleExistsAsync(roleName);
        if (!roleExist)
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }
}
