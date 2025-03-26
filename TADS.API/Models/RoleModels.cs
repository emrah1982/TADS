using System.ComponentModel.DataAnnotations;

namespace TADS.API.Models;

public class AssignRoleModel
{
    [Required]
    public string UserId { get; set; } = string.Empty;

    [Required]
    public string Role { get; set; } = string.Empty;
}

public class CreateRoleModel
{
    [Required]
    public string RoleName { get; set; } = string.Empty;
}
