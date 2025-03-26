using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TADS.API.Data;
using TADS.API.Models;

namespace TADS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
// Geçici olarak tüm kullanıcılar için erişim izni verelim
[Authorize(Roles = "superadmin,admin,user")]
public class UserManagementController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ApplicationDbContext _context;

    public UserManagementController(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        ApplicationDbContext context)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _context = context;
    }

    [HttpGet("users")]
    public async Task<IActionResult> GetUsers()
    {
        Console.WriteLine("GetUsers endpoint called");
        try
        {
            var users = await _userManager.Users.ToListAsync();
            Console.WriteLine($"Found {users.Count} users");
            var userList = new List<object>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                Console.WriteLine($"User {user.Email} has roles: {string.Join(", ", roles)}");
                userList.Add(new
                {
                    id = user.Id,
                    email = user.Email,
                    firstName = user.FirstName,
                    lastName = user.LastName,
                    roles = roles,
                    emailConfirmed = user.EmailConfirmed
                });
            }

            return Ok(userList);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error in GetUsers: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            return StatusCode(500, new { message = "Kullanıcılar alınırken bir hata oluştu", error = ex.Message });
        }
    }

    [HttpGet("roles")]
    [AllowAnonymous]
    public async Task<IActionResult> GetRoles()
    {
        try
        {
            Console.WriteLine("GetRoles endpoint called");

            // Veritabanı bağlantısını kontrol et
            if (!await _context.Database.CanConnectAsync())
            {
                Console.WriteLine("Veritabanına bağlanılamıyor!");
                return StatusCode(500, new { message = "Veritabanına bağlanılamıyor" });
            }

            // Rolleri direkt veritabanından çek
            var roles = await _roleManager.Roles.ToListAsync();
            
            if (roles == null || !roles.Any())
            {
                Console.WriteLine("Veritabanında hiç rol bulunamadı. Rolleri yeniden oluşturmayı deneyelim.");
                
                // Rolleri yeniden oluştur
                string[] defaultRoles = { "superadmin", "admin", "user", "operator", "analyst", "viewer", "field-worker", "manager" };
                foreach (var roleName in defaultRoles)
                {
                    if (!await _roleManager.RoleExistsAsync(roleName))
                    {
                        var result = await _roleManager.CreateAsync(new IdentityRole(roleName));
                        if (result.Succeeded)
                        {
                            Console.WriteLine($"{roleName} rolü başarıyla oluşturuldu.");
                        }
                        else
                        {
                            Console.WriteLine($"{roleName} rolü oluşturulurken hata: {string.Join(", ", result.Errors)}");
                        }
                    }
                }
                
                // Rolleri tekrar çek
                roles = await _roleManager.Roles.ToListAsync();
            }

            var roleNames = roles.Select(r => r.Name).ToList();
            Console.WriteLine($"Bulunan roller ({roleNames.Count}): {string.Join(", ", roleNames)}");
            
            return Ok(roleNames);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"GetRoles'da hata: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
            return StatusCode(500, new { message = "Roller alınırken bir hata oluştu", error = ex.Message });
        }
    }

    [HttpPost("assign-role")]
    [AllowAnonymous] // Yetkilendirme kısıtlamasını kaldırdık
    public async Task<IActionResult> AssignRole([FromBody] AssignRoleModel model)
    {
        var user = await _userManager.FindByIdAsync(model.UserId);
        if (user == null)
            return NotFound(new { success = false, message = "User not found" });

        if (!await _roleManager.RoleExistsAsync(model.Role))
            return BadRequest(new { success = false, message = "Role does not exist" });

        var currentRoles = await _userManager.GetRolesAsync(user);
        await _userManager.RemoveFromRolesAsync(user, currentRoles);
        await _userManager.AddToRoleAsync(user, model.Role);

        return Ok(new { success = true, message = "Role assigned successfully" });
    }

    [HttpPost("create-role")]
    [AllowAnonymous] // Yetkilendirme kısıtlamasını kaldırdık
    public async Task<IActionResult> CreateRole([FromBody] CreateRoleModel model)
    {
        if (await _roleManager.RoleExistsAsync(model.RoleName))
            return BadRequest(new { success = false, message = "Role already exists" });

        var result = await _roleManager.CreateAsync(new IdentityRole(model.RoleName));
        if (result.Succeeded)
            return Ok(new { success = true, message = "Role created successfully" });

        return BadRequest(new { success = false, message = string.Join(", ", result.Errors.Select(e => e.Description)) });
    }

    [HttpDelete("delete-role/{roleName}")]
    [AllowAnonymous] // Yetkilendirme kısıtlamasını kaldırdık
    public async Task<IActionResult> DeleteRole(string roleName)
    {
        if (roleName == "superadmin")
            return BadRequest(new { success = false, message = "Cannot delete superadmin role" });

        var role = await _roleManager.FindByNameAsync(roleName);
        if (role == null)
            return NotFound(new { success = false, message = "Role not found" });

        var result = await _roleManager.DeleteAsync(role);
        if (result.Succeeded)
            return Ok(new { success = true, message = "Role deleted successfully" });

        return BadRequest(new { success = false, message = string.Join(", ", result.Errors.Select(e => e.Description)) });
    }

    // Kullanıcı e-posta onayını güncelle
    [HttpPost("confirm-email/{userId}")]
    [AllowAnonymous] // Geçici olarak yetkilendirmeyi kaldırdık
    public async Task<IActionResult> ConfirmEmail(string userId)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
        {
            return NotFound("Kullanıcı bulunamadı.");
        }

        user.EmailConfirmed = true;
        var result = await _userManager.UpdateAsync(user);

        if (result.Succeeded)
        {
            return Ok("E-posta onayı başarıyla güncellendi.");
        }

        return BadRequest("E-posta onayı güncellenirken bir hata oluştu.");
    }

    // Tüm kullanıcıların e-posta onayını güncelle
    [HttpPost("confirm-all-emails")]
    [AllowAnonymous]
    public async Task<IActionResult> ConfirmAllEmails()
    {
        try
        {
            var users = _userManager.Users.Where(u => !u.EmailConfirmed).ToList();
            
            foreach (var user in users)
            {
                user.EmailConfirmed = true;
                await _userManager.UpdateAsync(user);
            }
            
            return Ok(new { success = true, message = $"{users.Count} kullanıcının e-posta adresi onaylandı" });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }
}
