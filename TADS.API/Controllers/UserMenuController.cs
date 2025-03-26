using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TADS.API.Services;

namespace TADS.API.Controllers
{
    [ApiController]
    [Route("api/user")]
    [Authorize]
    public class UserMenuController : ControllerBase
    {
        private readonly MenuPermissionService _menuPermissionService;

        public UserMenuController(MenuPermissionService menuPermissionService)
        {
            _menuPermissionService = menuPermissionService;
        }

        [HttpGet("menu-items")]
        public async Task<IActionResult> GetUserMenuItems()
        {
            // Kullanıcının rollerini al
            var userRoles = User.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();

            // Servis aracılığıyla kullanıcının erişebileceği menü öğelerini getir
            var menuItems = await _menuPermissionService.GetMenuItemsForRolesAsync(userRoles);

            return Ok(menuItems);
        }

        [HttpGet("check-access")]
        public async Task<IActionResult> CheckRouteAccess([FromQuery] string path)
        {
            // Kullanıcının rollerini al
            var userRoles = User.Claims
                .Where(c => c.Type == ClaimTypes.Role)
                .Select(c => c.Value)
                .ToList();

            // Servis aracılığıyla rota erişim kontrolü yap
            bool hasAccess = await _menuPermissionService.CheckRouteAccessAsync(path, userRoles);

            return Ok(new { hasAccess });
        }
    }
}
