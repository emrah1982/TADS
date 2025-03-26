using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TADS.API.Models;
using TADS.API.Services;

namespace TADS.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IConfiguration _configuration;
    private readonly IEmailService _emailService;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager,
        IConfiguration configuration,
        IEmailService emailService)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
        _emailService = emailService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterModel model)
    {
        try
        {
            var userExists = await _userManager.FindByEmailAsync(model.Email);
            if (userExists != null)
                return BadRequest(new { success = false, message = "Email already exists!" });

            var user = new ApplicationUser
            {
                Email = model.Email,
                UserName = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                EmailConfirmationToken = Guid.NewGuid().ToString()
            };

            var result = await _userManager.CreateAsync(user, model.Password);
            if (result.Succeeded)
            {
                // İlk kullanıcıyı superadmin yap, diğerlerini user
                var isFirstUser = !_userManager.Users.Any();
                var roleToAssign = isFirstUser ? "superadmin" : "user";

                // Rol yoksa oluştur
                if (!await _roleManager.RoleExistsAsync(roleToAssign))
                    await _roleManager.CreateAsync(new IdentityRole(roleToAssign));

                // Rolü ata
                await _userManager.AddToRoleAsync(user, roleToAssign);

                // Send confirmation email
                var confirmationLink = $"http://localhost:3000/confirm-email?token={user.EmailConfirmationToken}&email={user.Email}";
                var emailBody = $@"
                    <h2>Welcome to TADS!</h2>
                    <p>Please confirm your email by clicking the link below:</p>
                    <a href='{confirmationLink}'>Confirm Email</a>";

                await _emailService.SendEmailAsync(user.Email, "Confirm your email", emailBody);

                return Ok(new { success = true, message = "User created successfully. Please check your email to confirm your account." });
            }

            return BadRequest(new { success = false, message = string.Join(", ", result.Errors.Select(e => e.Description)) });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { success = false, message = $"Registration failed: {ex.Message}" });
        }
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        try 
        {
            // Gelen isteği logla
            Console.WriteLine($"Login attempt with email: {model.Email}");
            
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user == null)
            {
                Console.WriteLine("User not found");
                return Unauthorized(new { success = false, message = "Invalid email or password" });
            }

            if (!await _userManager.CheckPasswordAsync(user, model.Password))
            {
                Console.WriteLine("Password check failed");
                return Unauthorized(new { success = false, message = "Invalid email or password" });
            }

            // E-posta onayı kontrolünü geçici olarak devre dışı bırakıyoruz
            // if (!user.EmailConfirmed)
            //    return Unauthorized(new { success = false, message = "Please confirm your email first" });

            var userRoles = await _userManager.GetRolesAsync(user);
            var token = GenerateJwtToken(user, userRoles);
            
            Console.WriteLine("Login successful, token generated");
            return Ok(new { success = true, token = token });
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Login error: {ex.Message}");
            return StatusCode(500, new { success = false, message = $"Login failed: {ex.Message}" });
        }
    }

    [HttpGet("confirm-email")]
    public async Task<IActionResult> ConfirmEmail([FromQuery] string token, [FromQuery] string email)
    {
        var user = await _userManager.FindByEmailAsync(email);
        if (user == null)
            return BadRequest(new { success = false, message = "User not found" });

        if (user.EmailConfirmationToken != token)
            return BadRequest(new { success = false, message = "Invalid token" });

        user.EmailConfirmed = true;
        user.EmailConfirmationToken = null;
        await _userManager.UpdateAsync(user);

        return Ok(new { success = true, message = "Email confirmed successfully" });
    }

    private string GenerateJwtToken(ApplicationUser user, IList<string> roles)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email ?? string.Empty),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}")
        };

        // Add roles to claims
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"] ?? string.Empty));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
        var expires = DateTime.Now.AddDays(1);

        var token = new JwtSecurityToken(
            issuer: _configuration["JWT:ValidIssuer"],
            audience: _configuration["JWT:ValidAudience"],
            claims: claims,
            expires: expires,
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
