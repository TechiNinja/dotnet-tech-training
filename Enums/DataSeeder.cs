using Microsoft.AspNetCore.Identity;
using SportsManagementApp.Constants;
using SportsManagementApp.Data.Entities;
using SportsManagementApp.Repositories.Interfaces;
public static class DataSeeder
{
    public static async Task SeedAdminAsync(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();

        var authRepo = scope.ServiceProvider.GetRequiredService<IAuthRepository>();
        var roleRepo = scope.ServiceProvider.GetRequiredService<IRoleRepository>();

        // Check if admin already exists
        var existingAdmin = await authRepo.GetUserByEmailWithRoleAsync("admin@gmail.com");
        if (existingAdmin != null)
            return; // ✅ Do nothing if admin already exists

        var adminRole = await roleRepo.GetRoleByTypeAsync(RoleConstants.Admin);
        if (adminRole == null)
            throw new Exception("Admin role not found");

        var hasher = new PasswordHasher<User>();

        var admin = new User
        {
            Email = "admin@gmail.com",
            RoleId = adminRole.Id,
            Role = adminRole,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        admin.PasswordHash = hasher.HashPassword(admin, "Airtel@09876");

        await authRepo.AddUserAsync(admin);
    }
}