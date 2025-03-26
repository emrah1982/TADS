using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TADS.API.Models;

namespace TADS.API.Data
{
    public class MenuSeeder
    {
        private readonly ApplicationDbContext _context;

        public MenuSeeder(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            // Menü ve rota verisi zaten varsa, sadece eksik olanları ekle
            if (await _context.Menus.AnyAsync())
            {
                // Sulama ve Gübreleme Analizi menü öğesini kontrol et
                var irrigationAnalysisMenuItem = await _context.MenuItems
                    .FirstOrDefaultAsync(m => m.Path == "/irrigation-analysis");

                // Sulama ve Gübreleme Analizi menü öğesi yoksa, analiz menüsünü bul ve ekle
                if (irrigationAnalysisMenuItem == null)
                {
                    var existingAnalysisMenu = await _context.Menus
                        .FirstOrDefaultAsync(m => m.Title == "Analiz ve Raporlama");

                    if (existingAnalysisMenu != null)
                    {
                        Console.WriteLine("Sulama ve Gübreleme Analizi menü öğesi ekleniyor...");
                        
                        // Yeni menü öğesini ekle
                        var newIrrigationAnalysisMenuItem = new MenuItem
                        {
                            MenuId = existingAnalysisMenu.Id,
                            Title = "Sulama ve Gübreleme Analizi",
                            IconName = "WaterDropOutlined",
                            Path = "/irrigation-analysis",
                            DisplayOrder = 2,
                            IsActive = true
                        };

                        await _context.MenuItems.AddAsync(newIrrigationAnalysisMenuItem);
                        await _context.SaveChangesAsync();
                        
                        Console.WriteLine($"Sulama ve Gübreleme Analizi menü öğesi eklendi. ID: {newIrrigationAnalysisMenuItem.Id}");
                    }
                    else
                    {
                        Console.WriteLine("Analiz ve Raporlama menüsü bulunamadı!");
                    }
                }
                else
                {
                    Console.WriteLine("Sulama ve Gübreleme Analizi menü öğesi zaten mevcut.");
                }
                
                return;
            }

            // Ana menüleri oluştur
            var monitoringMenu = new Menu
            {
                Title = "Anlık Takip ve Görüntü İşleme",
                IconName = "MonitorOutlined",
                DisplayOrder = 1,
                IsActive = true
            };

            var analysisMenu = new Menu
            {
                Title = "Analiz ve Raporlama",
                IconName = "BarChartOutlined",
                DisplayOrder = 2,
                IsActive = true
            };

            var managementMenu = new Menu
            {
                Title = "Yönetim ve Ayarlar",
                IconName = "BuildOutlined",
                DisplayOrder = 3,
                IsActive = true
            };

            var operationalMenu = new Menu
            {
                Title = "İşletme ve Operasyonel Modüller",
                IconName = "AssignmentOutlined",
                DisplayOrder = 4,
                IsActive = true
            };

            await _context.Menus.AddRangeAsync(monitoringMenu, analysisMenu, managementMenu, operationalMenu);
            await _context.SaveChangesAsync();

            // Menü rol izinleri
            var menuRolePermissions = new List<MenuRolePermission>
            {
                new MenuRolePermission { Menu = monitoringMenu, RoleName = "superadmin" },
                new MenuRolePermission { Menu = monitoringMenu, RoleName = "user" },
                new MenuRolePermission { Menu = analysisMenu, RoleName = "superadmin" },
                new MenuRolePermission { Menu = analysisMenu, RoleName = "user" },
                new MenuRolePermission { Menu = managementMenu, RoleName = "superadmin" },
                new MenuRolePermission { Menu = operationalMenu, RoleName = "superadmin" }
            };

            await _context.MenuRolePermissions.AddRangeAsync(menuRolePermissions);
            await _context.SaveChangesAsync();

            // Alt menü öğelerini oluştur - Anlık Takip ve Görüntü İşleme
            var monitoringMenuItems = new List<MenuItem>
            {
                new MenuItem 
                { 
                    Menu = monitoringMenu,
                    Title = "Canlı Kamera Görüntüsü",
                    IconName = "MonitorOutlined",
                    Path = "/live-camera",
                    DisplayOrder = 1,
                    IsActive = true
                },
                new MenuItem 
                { 
                    Menu = monitoringMenu,
                    Title = "Görüntü İşleme ile Hastalık Tespiti",
                    IconName = "BugReportOutlined",
                    Path = "/disease-detection",
                    DisplayOrder = 2,
                    IsActive = true
                },
                new MenuItem 
                { 
                    Menu = monitoringMenu,
                    Title = "Anlık İklim ve Sensör Verileri",
                    IconName = "ThermostatOutlined",
                    Path = "/sensor-data",
                    DisplayOrder = 3,
                    IsActive = true
                },
                new MenuItem 
                { 
                    Menu = monitoringMenu,
                    Title = "Harita Üzerinde Alan İzleme",
                    IconName = "LocationOnOutlined",
                    Path = "/area-monitoring",
                    DisplayOrder = 4,
                    IsActive = true
                }
            };

            // Alt menü öğelerini oluştur - Analiz ve Raporlama
            var analysisMenuItems = new List<MenuItem>
            {
                new MenuItem 
                { 
                    Menu = analysisMenu,
                    Title = "Bitki Sağlığı Analizi",
                    IconName = "BugReportOutlined",
                    Path = "/plant-health",
                    DisplayOrder = 1,
                    IsActive = true
                },
                new MenuItem 
                { 
                    Menu = analysisMenu,
                    Title = "Sulama ve Gübreleme Analizi",
                    IconName = "WaterDropOutlined",
                    Path = "/irrigation-analysis",
                    DisplayOrder = 2,
                    IsActive = true
                },
                new MenuItem 
                { 
                    Menu = analysisMenu,
                    Title = "Periyodik Raporlar",
                    IconName = "CalendarTodayOutlined",
                    Path = "/periodic-reports",
                    DisplayOrder = 3,
                    IsActive = true
                },
                new MenuItem 
                { 
                    Menu = analysisMenu,
                    Title = "Geçmiş Kayıtlar / Arşiv",
                    IconName = "HistoryOutlined",
                    Path = "/history",
                    DisplayOrder = 4,
                    IsActive = true
                }
            };

            // Alt menü öğelerini oluştur - Yönetim ve Ayarlar
            var managementMenuItems = new List<MenuItem>
            {
                new MenuItem 
                { 
                    Menu = managementMenu,
                    Title = "Kullanıcı ve Rol Yönetimi",
                    IconName = "PeopleOutlined",
                    Path = "/user-management",
                    DisplayOrder = 1,
                    IsActive = true
                },
                new MenuItem 
                { 
                    Menu = managementMenu,
                    Title = "Parsel / Alan Tanımlamaları",
                    IconName = "TerrainOutlined",
                    Path = "/area-management",
                    DisplayOrder = 2,
                    IsActive = true
                },
                new MenuItem 
                { 
                    Menu = managementMenu,
                    Title = "Bildirim Ayarları",
                    IconName = "NotificationsOutlined",
                    Path = "/notification-settings",
                    DisplayOrder = 3,
                    IsActive = true
                },
                new MenuItem 
                { 
                    Menu = managementMenu,
                    Title = "Test ve Kalibrasyon Paneli",
                    IconName = "BuildOutlined",
                    Path = "/calibration",
                    DisplayOrder = 4,
                    IsActive = true
                }
            };

            // Alt menü öğelerini oluştur - İşletme ve Operasyonel Modüller
            var operationalMenuItems = new List<MenuItem>
            {
                new MenuItem 
                { 
                    Menu = operationalMenu,
                    Title = "İş Planı ve Görev Yönetimi",
                    IconName = "AssignmentOutlined",
                    Path = "/task-management",
                    DisplayOrder = 1,
                    IsActive = true
                },
                new MenuItem 
                { 
                    Menu = operationalMenu,
                    Title = "İlaçlama & Gübreleme Kayıtları",
                    IconName = "PestControlOutlined",
                    Path = "/treatment-records",
                    DisplayOrder = 2,
                    IsActive = true
                },
                new MenuItem 
                { 
                    Menu = operationalMenu,
                    Title = "Ekipman Takibi",
                    IconName = "AgricultureOutlined",
                    Path = "/equipment-tracking",
                    DisplayOrder = 3,
                    IsActive = true
                }
            };

            // Tüm menü öğelerini ekle
            var allMenuItems = new List<MenuItem>();
            allMenuItems.AddRange(monitoringMenuItems);
            allMenuItems.AddRange(analysisMenuItems);
            allMenuItems.AddRange(managementMenuItems);
            allMenuItems.AddRange(operationalMenuItems);

            await _context.MenuItems.AddRangeAsync(allMenuItems);
            await _context.SaveChangesAsync();

            // Alt menü rol izinleri
            var menuItemRolePermissions = new List<MenuItemRolePermission>();

            // Anlık Takip ve Görüntü İşleme rol izinleri
            menuItemRolePermissions.Add(new MenuItemRolePermission { MenuItem = monitoringMenuItems[0], RoleName = "superadmin" }); // Canlı Kamera Görüntüsü
            menuItemRolePermissions.Add(new MenuItemRolePermission { MenuItem = monitoringMenuItems[1], RoleName = "superadmin" }); // Görüntü İşleme ile Hastalık Tespiti
            menuItemRolePermissions.Add(new MenuItemRolePermission { MenuItem = monitoringMenuItems[1], RoleName = "user" });       // Görüntü İşleme ile Hastalık Tespiti
            menuItemRolePermissions.Add(new MenuItemRolePermission { MenuItem = monitoringMenuItems[2], RoleName = "superadmin" }); // Anlık İklim ve Sensör Verileri
            menuItemRolePermissions.Add(new MenuItemRolePermission { MenuItem = monitoringMenuItems[2], RoleName = "user" });       // Anlık İklim ve Sensör Verileri
            menuItemRolePermissions.Add(new MenuItemRolePermission { MenuItem = monitoringMenuItems[3], RoleName = "superadmin" }); // Harita Üzerinde Alan İzleme

            // Analiz ve Raporlama rol izinleri
            menuItemRolePermissions.Add(new MenuItemRolePermission { MenuItem = analysisMenuItems[0], RoleName = "superadmin" }); // Bitki Sağlığı Analizi
            menuItemRolePermissions.Add(new MenuItemRolePermission { MenuItem = analysisMenuItems[0], RoleName = "user" });       // Bitki Sağlığı Analizi
            menuItemRolePermissions.Add(new MenuItemRolePermission { MenuItem = analysisMenuItems[1], RoleName = "superadmin" }); // Sulama ve Gübreleme Analizi
            menuItemRolePermissions.Add(new MenuItemRolePermission { MenuItem = analysisMenuItems[1], RoleName = "user" });       // Sulama ve Gübreleme Analizi
            menuItemRolePermissions.Add(new MenuItemRolePermission { MenuItem = analysisMenuItems[2], RoleName = "superadmin" }); // Periyodik Raporlar
            menuItemRolePermissions.Add(new MenuItemRolePermission { MenuItem = analysisMenuItems[2], RoleName = "user" });       // Periyodik Raporlar
            menuItemRolePermissions.Add(new MenuItemRolePermission { MenuItem = analysisMenuItems[3], RoleName = "superadmin" }); // Geçmiş Kayıtlar / Arşiv

            // Yönetim ve Ayarlar rol izinleri - Sadece superadmin
            foreach (var item in managementMenuItems)
            {
                menuItemRolePermissions.Add(new MenuItemRolePermission { MenuItem = item, RoleName = "superadmin" });
            }

            // İşletme ve Operasyonel Modüller rol izinleri - Sadece superadmin
            foreach (var item in operationalMenuItems)
            {
                menuItemRolePermissions.Add(new MenuItemRolePermission { MenuItem = item, RoleName = "superadmin" });
            }

            await _context.MenuItemRolePermissions.AddRangeAsync(menuItemRolePermissions);
            await _context.SaveChangesAsync();

            // Rota izinleri oluştur
            var routePermissions = new List<RoutePermission>
            {
                new RoutePermission { Path = "/dashboard", Description = "Ana Sayfa", IsActive = true },
                new RoutePermission { Path = "/live-camera", Description = "Canlı Kamera Görüntüsü", IsActive = true },
                new RoutePermission { Path = "/disease-detection", Description = "Görüntü İşleme ile Hastalık Tespiti", IsActive = true },
                new RoutePermission { Path = "/sensor-data", Description = "Anlık İklim ve Sensör Verileri", IsActive = true },
                new RoutePermission { Path = "/area-monitoring", Description = "Harita Üzerinde Alan İzleme", IsActive = true },
                new RoutePermission { Path = "/plant-health", Description = "Bitki Sağlığı Analizi", IsActive = true },
                new RoutePermission { Path = "/irrigation-analysis", Description = "Sulama ve Gübreleme Analizi", IsActive = true },
                new RoutePermission { Path = "/periodic-reports", Description = "Periyodik Raporlar", IsActive = true },
                new RoutePermission { Path = "/history", Description = "Geçmiş Kayıtlar / Arşiv", IsActive = true },
                new RoutePermission { Path = "/user-management", Description = "Kullanıcı ve Rol Yönetimi", IsActive = true },
                new RoutePermission { Path = "/area-management", Description = "Parsel / Alan Tanımlamaları", IsActive = true },
                new RoutePermission { Path = "/notification-settings", Description = "Bildirim Ayarları", IsActive = true },
                new RoutePermission { Path = "/calibration", Description = "Test ve Kalibrasyon Paneli", IsActive = true },
                new RoutePermission { Path = "/task-management", Description = "İş Planı ve Görev Yönetimi", IsActive = true },
                new RoutePermission { Path = "/treatment-records", Description = "İlaçlama & Gübreleme Kayıtları", IsActive = true },
                new RoutePermission { Path = "/equipment-tracking", Description = "Ekipman Takibi", IsActive = true }
            };

            await _context.RoutePermissions.AddRangeAsync(routePermissions);
            await _context.SaveChangesAsync();

            // Rota rol izinleri
            var routeRolePermissions = new List<RouteRolePermission>();
            var dashboardRoute = routePermissions.First(r => r.Path == "/dashboard");
            var diseaseDetectionRoute = routePermissions.First(r => r.Path == "/disease-detection");
            var sensorDataRoute = routePermissions.First(r => r.Path == "/sensor-data");
            var plantHealthRoute = routePermissions.First(r => r.Path == "/plant-health");
            var periodicReportsRoute = routePermissions.First(r => r.Path == "/periodic-reports");

            // Dashboard, tüm kullanıcıların erişebildiği bir sayfa
            routeRolePermissions.Add(new RouteRolePermission { RoutePermission = dashboardRoute, RoleName = "superadmin" });
            routeRolePermissions.Add(new RouteRolePermission { RoutePermission = dashboardRoute, RoleName = "user" });

            // User rolünün erişebildiği sayfalar
            routeRolePermissions.Add(new RouteRolePermission { RoutePermission = diseaseDetectionRoute, RoleName = "user" });
            routeRolePermissions.Add(new RouteRolePermission { RoutePermission = sensorDataRoute, RoleName = "user" });
            routeRolePermissions.Add(new RouteRolePermission { RoutePermission = plantHealthRoute, RoleName = "user" });
            routeRolePermissions.Add(new RouteRolePermission { RoutePermission = periodicReportsRoute, RoleName = "user" });
            
            // Sulama ve Gübreleme Analizi için user rolüne erişim izni
            var irrigationAnalysisRoute = routePermissions.First(r => r.Path == "/irrigation-analysis");
            routeRolePermissions.Add(new RouteRolePermission { RoutePermission = irrigationAnalysisRoute, RoleName = "user" });

            // Superadmin rolü tüm sayfalara erişebilir
            foreach (var route in routePermissions)
            {
                routeRolePermissions.Add(new RouteRolePermission { RoutePermission = route, RoleName = "superadmin" });
            }

            await _context.RouteRolePermissions.AddRangeAsync(routeRolePermissions);
            await _context.SaveChangesAsync();
        }
    }
}
