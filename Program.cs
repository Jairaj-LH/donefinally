using charac.Data;
using charac.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using DinkToPdf;
using DinkToPdf.Contracts;
using System.IO;
using charac.Services;
using charac.Hubs;
using Prometheus;   // <-- Added for Prometheus

var builder = WebApplication.CreateBuilder(args);

// Load native library for DinkToPdf (Windows)
// Make sure wkhtmltox.dll is placed under "NativeLibs" folder in your project and set to Copy if newer
var context = new CustomAssemblyLoadContext();
var nativeLibraryPath = Path.Combine(AppContext.BaseDirectory, "NativeLibs", "libwkhtmltox.dll");
context.LoadUnmanagedLibrary(nativeLibraryPath);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Register DinkToPdf services
builder.Services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));
builder.Services.AddScoped<PdfService>();

// Add DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Identity services
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Configure cookies
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";
});

// Add Authorization policies
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy => policy.RequireRole("Admin"));
    options.AddPolicy("UserPolicy", policy => policy.RequireRole("User"));
    options.AddPolicy("ManagerPolicy", policy => policy.RequireRole("Manager"));
});

// Razor Pages support
builder.Services.AddRazorPages();

builder.Services.AddHttpClient();
builder.Services.AddSingleton<IEmailSender, EmailSender>();

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddScoped<IUserActivityLogger, UserActivityLogger>();
builder.Services.AddHostedService<SubjectReminderService>();
builder.Services.AddHostedService<HistoryCleanupService>();
builder.Services.AddSignalR();

var app = builder.Build();

// Seed roles into the database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
    await SeedRoles(roleManager);
}

// Configure HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}


app.MapHub<FeedbackHub>("/feedbackHub");

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

// **Prometheus HTTP metrics middleware added here**
app.UseHttpMetrics();  // <-- Added

app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

// **Expose the /metrics endpoint for Prometheus scraping**
app.MapMetrics();  // <-- Added

app.Run();

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
