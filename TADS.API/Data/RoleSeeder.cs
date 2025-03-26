using Microsoft.AspNetCore.Identity;

namespace TADS.API.Data;

public static class RoleSeeder
{
    public static async Task SeedRoles(RoleManager<IdentityRole> roleManager)
    {
        string[] roles = { "superadmin", "admin", "user", "operator", "analyst", "viewer", "field-worker", "manager" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }
}
