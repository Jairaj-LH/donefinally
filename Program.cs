using charac.Data;
using charac.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using DinkToPdf;
using DinkToPdf.Contracts;
using System.IO;
using charac.Services;

var builder = WebApplication.CreateBuilder(args);

// Load native library for DinkToPdf (Windows)
// Make sure wkhtmltox.dll is placed under "NativeLibs" folder in your project and set to Copy if newer
var context = new CustomAssemblyLoadContext();
var nativeLibraryPath = Path.Combine(AppContext.BaseDirectory, "NativeLibs", "libwkhtmltox.dll");
context.LoadUnmanagedLibrary(nativeLibraryPath);

// Add services to the container.
builder.Services.AddControllersWithViews(); // Only need to add this once

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Register DinkToPdf services
builder.Services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));
builder.Services.AddScoped<PdfService>();

// Add DbContext to the container for your application's database
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Identity services with Roles and token providers
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Add authentication and authorization services
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
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

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddScoped<IUserActivityLogger, UserActivityLogger>();
builder.Services.AddHostedService<SubjectReminderService>();
builder.Services.AddHostedService<HistoryCleanupService>();


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

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

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
