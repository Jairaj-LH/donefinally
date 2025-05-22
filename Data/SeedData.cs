//using Microsoft.AspNetCore.Identity;
//using System.Threading.Tasks;

//public class SeedData
//{
//    public static async Task Initialize(IServiceProvider serviceProvider, UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
//    {
//        // Define the roles you want to create
//        var roleNames = new[] { "Admin", "User" };

//        // Create roles if they don't exist
//        foreach (var roleName in roleNames)
//        {
//            var roleExist = await roleManager.RoleExistsAsync(roleName);
//            if (!roleExist)
//            {
//                await roleManager.CreateAsync(new IdentityRole(roleName));
//            }
//        }

//        // Create an admin user if one doesn't exist
//        var adminUser = await userManager.FindByEmailAsync("admin@example.com");
//        if (adminUser == null)
//        {
//            var user = new IdentityUser { UserName = "admin@example.com", Email = "admin@example.com" };
//            var result = await userManager.CreateAsync(user, "Password123!");
//            if (result.Succeeded)
//            {
//                await userManager.AddToRoleAsync(user, "Admin");
//            }
//        }
//    }
//}
