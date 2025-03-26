using Microsoft.AspNetCore.Identity;

namespace TADS.API.Models;

public class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    
    public string? EmailConfirmationToken { get; set; }
    
    // Override to avoid warnings
    public override bool EmailConfirmed { get; set; }
}
