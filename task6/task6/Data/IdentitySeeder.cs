using Microsoft.AspNetCore.Identity;
using task6.Models;

namespace task6.Data
{
    public static class IdentitySeeder
    {
        public static async Task SeedAsync(IServiceProvider services, IConfiguration configuration)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

            foreach (var roleName in new[] { "User", "Admin", "SuperAdmin" })
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                    await roleManager.CreateAsync(new IdentityRole(roleName));
            }

            var email = configuration["SeedSuperAdmin:Email"];
            var password = configuration["SeedSuperAdmin:Password"];
            var fullName = configuration["SeedSuperAdmin:FullName"] ?? "System Administrator";

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
                return;

            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                user = new ApplicationUser
                {
                    FullName = fullName,
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(user, password);
                if (!result.Succeeded)
                    throw new InvalidOperationException(string.Join("; ", result.Errors.Select(e => e.Description)));
            }

            if (!await userManager.IsInRoleAsync(user, "SuperAdmin"))
                await userManager.AddToRoleAsync(user, "SuperAdmin");
        }
    }
}
