using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TADS.API.Data;
using TADS.API.Models;

namespace TADS.API.Services
{
    public class MenuPermissionService
    {
        private readonly ApplicationDbContext _context;

        public MenuPermissionService(ApplicationDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Belirtilen rollere sahip kullanıcı için menü öğelerini getirir
        /// </summary>
        public async Task<List<MenuDto>> GetMenuItemsForRolesAsync(List<string> userRoles)
        {
            // Kullanıcının rollerine göre erişebileceği ana menüleri bul
            var accessibleMenuIds = await _context.MenuRolePermissions
                .Where(mrp => userRoles.Contains(mrp.RoleName))
                .Select(mrp => mrp.MenuId)
                .Distinct()
                .ToListAsync();

            // Ana menüleri çek
            var menus = await _context.Menus
                .Where(m => m.IsActive && accessibleMenuIds.Contains(m.Id))
                .OrderBy(m => m.DisplayOrder)
                .ToListAsync();

            // Kullanıcının rollerine göre erişebileceği alt menü öğelerini bul
            var accessibleMenuItemIds = await _context.MenuItemRolePermissions
                .Where(mirp => userRoles.Contains(mirp.RoleName))
                .Select(mirp => mirp.MenuItemId)
                .Distinct()
                .ToListAsync();

            // Menüleri DTO'lara dönüştür
            var result = new List<MenuDto>();

            foreach (var menu in menus)
            {
                // Menüye ait erişilebilir alt öğeleri çek
                var menuItems = await _context.MenuItems
                    .Where(mi => mi.MenuId == menu.Id && mi.IsActive && accessibleMenuItemIds.Contains(mi.Id))
                    .OrderBy(mi => mi.DisplayOrder)
                    .ToListAsync();

                // Menünün alt öğeleri yoksa ekleme
                if (!menuItems.Any())
                    continue;

                // Menüyü ve alt öğelerini DTO'ya dönüştür
                var menuDto = new MenuDto
                {
                    Title = menu.Title,
                    IconName = menu.IconName,
                    Items = menuItems.Select(mi => new MenuItemDto
                    {
                        Title = mi.Title,
                        IconName = mi.IconName,
                        Path = mi.Path
                    }).ToList()
                };

                result.Add(menuDto);
            }

            return result;
        }

        /// <summary>
        /// Belirtilen rotalara kullanıcının erişim izni olup olmadığını kontrol eder
        /// </summary>
        public async Task<bool> CheckRouteAccessAsync(string path, List<string> userRoles)
        {
            // Rota veritabanında var mı kontrol et
            var route = await _context.RoutePermissions
                .FirstOrDefaultAsync(rp => rp.Path == path && rp.IsActive);

            if (route == null)
                return false;

            // Kullanıcının bu rotaya erişim izni var mı kontrol et
            var hasAccess = await _context.RouteRolePermissions
                .AnyAsync(rrp => rrp.RoutePermissionId == route.Id && userRoles.Contains(rrp.RoleName));

            return hasAccess;
        }
    }

    // API yanıtları için DTO (Data Transfer Object) sınıfları
    public class MenuDto
    {
        public string Title { get; set; }
        public string IconName { get; set; }
        public List<MenuItemDto> Items { get; set; }
    }

    public class MenuItemDto
    {
        public string Title { get; set; }
        public string IconName { get; set; }
        public string Path { get; set; }
    }
}
