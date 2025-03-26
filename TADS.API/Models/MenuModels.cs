using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TADS.API.Models
{
    /// <summary>
    /// Ana menü öğelerini temsil eder
    /// </summary>
    public class Menu
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string IconName { get; set; } = string.Empty;
        
        public int DisplayOrder { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        // İlişkiler
        public virtual ICollection<MenuItem> MenuItems { get; set; } = new List<MenuItem>();
        public virtual ICollection<MenuRolePermission> RolePermissions { get; set; } = new List<MenuRolePermission>();
    }
    
    /// <summary>
    /// Alt menü öğelerini temsil eder
    /// </summary>
    public class MenuItem
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Title { get; set; } = string.Empty;
        
        [Required]
        [StringLength(50)]
        public string IconName { get; set; } = string.Empty;
        
        [StringLength(100)]
        public string Path { get; set; } = string.Empty;
        
        public int DisplayOrder { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        // İlişkiler
        [ForeignKey("Menu")]
        public int MenuId { get; set; }
        public virtual Menu Menu { get; set; } = null!;
        
        public virtual ICollection<MenuItemRolePermission> RolePermissions { get; set; } = new List<MenuItemRolePermission>();
    }
    
    /// <summary>
    /// Ana menülere erişim izinlerini temsil eder
    /// </summary>
    public class MenuRolePermission
    {
        [Key]
        public int Id { get; set; }
        
        [ForeignKey("Menu")]
        public int MenuId { get; set; }
        public virtual Menu Menu { get; set; } = null!;
        
        [Required]
        [StringLength(50)]
        public string RoleName { get; set; } = string.Empty;
    }
    
    /// <summary>
    /// Alt menü öğelerine erişim izinlerini temsil eder
    /// </summary>
    public class MenuItemRolePermission
    {
        [Key]
        public int Id { get; set; }
        
        [ForeignKey("MenuItem")]
        public int MenuItemId { get; set; }
        public virtual MenuItem MenuItem { get; set; } = null!;
        
        [Required]
        [StringLength(50)]
        public string RoleName { get; set; } = string.Empty;
    }
    
    /// <summary>
    /// Sayfa (rota) erişim izinlerini temsil eder
    /// </summary>
    public class RoutePermission
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Path { get; set; } = string.Empty;
        
        [Required]
        [StringLength(100)]
        public string Description { get; set; } = string.Empty;
        
        public bool IsActive { get; set; } = true;
        
        // İlişkiler
        public virtual ICollection<RouteRolePermission> RolePermissions { get; set; } = new List<RouteRolePermission>();
    }
    
    /// <summary>
    /// Sayfa (rota) erişim izinlerini rollere bağlar
    /// </summary>
    public class RouteRolePermission
    {
        [Key]
        public int Id { get; set; }
        
        [ForeignKey("RoutePermission")]
        public int RoutePermissionId { get; set; }
        public virtual RoutePermission RoutePermission { get; set; } = null!;
        
        [Required]
        [StringLength(50)]
        public string RoleName { get; set; } = string.Empty;
    }
}
