using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TADS.API.Models;

namespace TADS.API.Data
{
    public class IrrigationFertilizationMenuSeeder
    {
        private readonly ApplicationDbContext _context;

        public IrrigationFertilizationMenuSeeder(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            Console.WriteLine("IrrigationFertilizationMenuSeeder başlatılıyor...");

            try
            {
                // Analiz ve Raporlama ana menüsünü bul
                var analysisMenu = await _context.Menus
                    .FirstOrDefaultAsync(m => m.Title == "Analiz ve Raporlama");

                if (analysisMenu == null)
                {
                    Console.WriteLine("Analiz ve Raporlama menüsü bulunamadı, yeni oluşturuluyor...");
                    // Analiz ve Raporlama menüsü yoksa oluştur
                    analysisMenu = new Menu
                    {
                        Title = "Analiz ve Raporlama",
                        IconName = "BarChartOutlined",
                        DisplayOrder = 2,
                        IsActive = true
                    };

                    await _context.Menus.AddAsync(analysisMenu);
                    await _context.SaveChangesAsync();
                    
                    // Menü rol izinlerini ekle
                    var menuRolePermissions = new List<MenuRolePermission>
                    {
                        new MenuRolePermission { MenuId = analysisMenu.Id, RoleName = "superadmin" },
                        new MenuRolePermission { MenuId = analysisMenu.Id, RoleName = "user" }
                    };

                    await _context.MenuRolePermissions.AddRangeAsync(menuRolePermissions);
                    await _context.SaveChangesAsync();
                    
                    Console.WriteLine($"Analiz ve Raporlama menüsü oluşturuldu. ID: {analysisMenu.Id}");
                }

                // Sulama ve Gübreleme Analizi menü öğesini kontrol et
                var irrigationAnalysisMenuItem = await _context.MenuItems
                    .FirstOrDefaultAsync(m => m.Path == "/irrigation-fertilization" && m.MenuId == analysisMenu.Id);

                if (irrigationAnalysisMenuItem == null)
                {
                    Console.WriteLine("Sulama ve Gübreleme Analizi menü öğesi bulunamadı, yeni oluşturuluyor...");
                    
                    // Mevcut en yüksek DisplayOrder değerini bul
                    var maxDisplayOrder = await _context.MenuItems
                        .Where(m => m.MenuId == analysisMenu.Id)
                        .Select(m => (int?)m.DisplayOrder)
                        .MaxAsync() ?? 0;
                    
                    // Yeni menü öğesini ekle
                    var newMenuItem = new MenuItem
                    {
                        MenuId = analysisMenu.Id,
                        Title = "Sulama ve Gübreleme Analizi",
                        IconName = "WaterDropOutlined",
                        Path = "/irrigation-fertilization",
                        DisplayOrder = maxDisplayOrder + 1,
                        IsActive = true
                    };

                    await _context.MenuItems.AddAsync(newMenuItem);
                    await _context.SaveChangesAsync();
                    
                    // Menü öğesi rol izinlerini ekle
                    var menuItemRolePermissions = new List<MenuItemRolePermission>
                    {
                        new MenuItemRolePermission { MenuItemId = newMenuItem.Id, RoleName = "superadmin" },
                        new MenuItemRolePermission { MenuItemId = newMenuItem.Id, RoleName = "admin" },
                        new MenuItemRolePermission { MenuItemId = newMenuItem.Id, RoleName = "operator" },
                        new MenuItemRolePermission { MenuItemId = newMenuItem.Id, RoleName = "user" }
                    };

                    await _context.MenuItemRolePermissions.AddRangeAsync(menuItemRolePermissions);
                    await _context.SaveChangesAsync();
                    
                    Console.WriteLine($"Sulama ve Gübreleme Analizi menü öğesi oluşturuldu. ID: {newMenuItem.Id}");
                }
                else
                {
                    // Eğer path değişmişse güncelle
                    if (irrigationAnalysisMenuItem.Path != "/irrigation-fertilization")
                    {
                        irrigationAnalysisMenuItem.Path = "/irrigation-fertilization";
                        _context.MenuItems.Update(irrigationAnalysisMenuItem);
                        await _context.SaveChangesAsync();
                        Console.WriteLine($"Sulama ve Gübreleme Analizi menü öğesi path'i güncellendi. ID: {irrigationAnalysisMenuItem.Id}");
                    }
                    else
                    {
                        Console.WriteLine($"Sulama ve Gübreleme Analizi menü öğesi zaten mevcut. ID: {irrigationAnalysisMenuItem.Id}");
                        
                        // Menü öğesi için rol izinleri var mı kontrol et
                        var existingPermissions = await _context.MenuItemRolePermissions
                            .Where(p => p.MenuItemId == irrigationAnalysisMenuItem.Id)
                            .ToListAsync();
                            
                        if (existingPermissions.Count == 0)
                        {
                            // Rol izinleri yoksa ekle
                            var menuItemRolePermissions = new List<MenuItemRolePermission>
                            {
                                new MenuItemRolePermission { MenuItemId = irrigationAnalysisMenuItem.Id, RoleName = "superadmin" },
                                new MenuItemRolePermission { MenuItemId = irrigationAnalysisMenuItem.Id, RoleName = "admin" },
                                new MenuItemRolePermission { MenuItemId = irrigationAnalysisMenuItem.Id, RoleName = "operator" },
                                new MenuItemRolePermission { MenuItemId = irrigationAnalysisMenuItem.Id, RoleName = "user" }
                            };

                            await _context.MenuItemRolePermissions.AddRangeAsync(menuItemRolePermissions);
                            await _context.SaveChangesAsync();
                            Console.WriteLine($"Sulama ve Gübreleme Analizi menü öğesi için rol izinleri eklendi.");
                        }
                        else
                        {
                            Console.WriteLine($"Sulama ve Gübreleme Analizi menü öğesi için rol izinleri zaten mevcut. İzin sayısı: {existingPermissions.Count}");
                        }
                    }
                }

                // Gübre Türleri için menü öğesi kontrol et
                var fertilizerTypesMenuItem = await _context.MenuItems
                    .FirstOrDefaultAsync(m => m.Path == "/fertilizer-types" && m.MenuId == analysisMenu.Id);

                if (fertilizerTypesMenuItem == null)
                {
                    Console.WriteLine("Gübre Türleri menü öğesi bulunamadı, yeni oluşturuluyor...");
                    
                    // Mevcut en yüksek DisplayOrder değerini bul
                    var maxDisplayOrder = await _context.MenuItems
                        .Where(m => m.MenuId == analysisMenu.Id)
                        .Select(m => (int?)m.DisplayOrder)
                        .MaxAsync() ?? 0;
                    
                    // Yeni menü öğesini ekle
                    var newMenuItem = new MenuItem
                    {
                        MenuId = analysisMenu.Id,
                        Title = "Gübre Türleri",
                        IconName = "SpaOutlined",
                        Path = "/fertilizer-types",
                        DisplayOrder = maxDisplayOrder + 1,
                        IsActive = true
                    };

                    await _context.MenuItems.AddAsync(newMenuItem);
                    await _context.SaveChangesAsync();
                    
                    // Menü öğesi rol izinlerini ekle
                    var menuItemRolePermissions = new List<MenuItemRolePermission>
                    {
                        new MenuItemRolePermission { MenuItemId = newMenuItem.Id, RoleName = "superadmin" },
                        new MenuItemRolePermission { MenuItemId = newMenuItem.Id, RoleName = "admin" },
                        new MenuItemRolePermission { MenuItemId = newMenuItem.Id, RoleName = "operator" },
                        new MenuItemRolePermission { MenuItemId = newMenuItem.Id, RoleName = "user" }
                    };

                    await _context.MenuItemRolePermissions.AddRangeAsync(menuItemRolePermissions);
                    await _context.SaveChangesAsync();
                    
                    Console.WriteLine($"Gübre Türleri menü öğesi oluşturuldu. ID: {newMenuItem.Id}");
                }
                else
                {
                    Console.WriteLine($"Gübre Türleri menü öğesi zaten mevcut. ID: {fertilizerTypesMenuItem.Id}");
                    
                    // Menü öğesi için rol izinleri var mı kontrol et
                    var existingPermissions = await _context.MenuItemRolePermissions
                        .Where(p => p.MenuItemId == fertilizerTypesMenuItem.Id)
                        .ToListAsync();
                        
                    if (existingPermissions.Count == 0)
                    {
                        // Rol izinleri yoksa ekle
                        var menuItemRolePermissions = new List<MenuItemRolePermission>
                        {
                            new MenuItemRolePermission { MenuItemId = fertilizerTypesMenuItem.Id, RoleName = "superadmin" },
                            new MenuItemRolePermission { MenuItemId = fertilizerTypesMenuItem.Id, RoleName = "admin" },
                            new MenuItemRolePermission { MenuItemId = fertilizerTypesMenuItem.Id, RoleName = "operator" },
                            new MenuItemRolePermission { MenuItemId = fertilizerTypesMenuItem.Id, RoleName = "user" }
                        };

                        await _context.MenuItemRolePermissions.AddRangeAsync(menuItemRolePermissions);
                        await _context.SaveChangesAsync();
                        Console.WriteLine($"Gübre Türleri menü öğesi için rol izinleri eklendi.");
                    }
                    else
                    {
                        Console.WriteLine($"Gübre Türleri menü öğesi için rol izinleri zaten mevcut. İzin sayısı: {existingPermissions.Count}");
                    }
                }

                Console.WriteLine("IrrigationFertilizationMenuSeeder tamamlandı.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"IrrigationFertilizationMenuSeeder hatası: {ex.Message}");
                Console.WriteLine($"İç hata: {ex.InnerException?.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }
    }
}
