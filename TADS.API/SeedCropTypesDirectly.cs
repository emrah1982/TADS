using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TADS.API.Data;
using TADS.API.Models;

namespace TADS.API
{
    public class SeedCropTypesDirectly
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("Ekim türleri veritabanına ekleniyor...");
            
            try
            {
                // Host oluştur
                using var host = Host.CreateDefaultBuilder(args)
                    .ConfigureServices((context, services) =>
                    {
                        // Veritabanı bağlantısını yapılandır
                        var connectionString = "server=localhost;database=tadsdb;user=root;password=secgem;port=3306";
                        services.AddDbContext<ApplicationDbContext>(options =>
                            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));
                    })
                    .Build();

                // Servis sağlayıcıdan DbContext al
                using var scope = host.Services.CreateScope();
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<ApplicationDbContext>();

                // Veritabanında ekim türleri var mı kontrol et
                var cropTypesExist = await context.CropTypes.AnyAsync();
                Console.WriteLine($"Veritabanında ekim türleri mevcut mu: {cropTypesExist}");

                if (!cropTypesExist)
                {
                    Console.WriteLine("Ekim türleri ekleniyor...");

                    // Önceden tanımlı ekim türleri
                    var cropTypes = new[]
                    {
                        new CropType { 
                            Name = "Buğday", 
                            Description = "Buğday, dünya çapında en yaygın olarak yetiştirilen tahıl bitkisidir.", 
                            GrowingSeason = "Sonbahar-İlkbahar", 
                            GrowingDays = 240 
                        },
                        new CropType { 
                            Name = "Arpa", 
                            Description = "Arpa, buğdaydan sonra en çok yetiştirilen tahıl bitkisidir.", 
                            GrowingSeason = "Sonbahar-İlkbahar", 
                            GrowingDays = 210 
                        },
                        new CropType { 
                            Name = "Mısır", 
                            Description = "Mısır, dünya çapında en önemli tahıl bitkilerinden biridir.", 
                            GrowingSeason = "İlkbahar-Yaz", 
                            GrowingDays = 120 
                        },
                        new CropType { 
                            Name = "Pamuk", 
                            Description = "Pamuk, tekstil endüstrisinin en önemli hammaddesidir.", 
                            GrowingSeason = "İlkbahar-Yaz", 
                            GrowingDays = 160 
                        },
                        new CropType { 
                            Name = "Ayçiçeği", 
                            Description = "Ayçiçeği, yağ üretimi için yetiştirilen önemli bir bitkidir.", 
                            GrowingSeason = "İlkbahar-Yaz", 
                            GrowingDays = 110 
                        },
                        new CropType { 
                            Name = "Domates", 
                            Description = "Domates, dünya çapında en çok tüketilen sebzelerden biridir.", 
                            GrowingSeason = "İlkbahar-Yaz", 
                            GrowingDays = 90 
                        },
                        new CropType { 
                            Name = "Patates", 
                            Description = "Patates, dünya çapında en önemli kök sebzelerinden biridir.", 
                            GrowingSeason = "İlkbahar-Yaz", 
                            GrowingDays = 100 
                        },
                        new CropType { 
                            Name = "Soğan", 
                            Description = "Soğan, dünya çapında en yaygın olarak yetiştirilen sebzelerden biridir.", 
                            GrowingSeason = "İlkbahar-Yaz", 
                            GrowingDays = 120 
                        },
                        new CropType { 
                            Name = "Çeltik (Pirinç)", 
                            Description = "Çeltik, dünya nüfusunun büyük bir kısmının temel gıda maddesidir.", 
                            GrowingSeason = "İlkbahar-Yaz", 
                            GrowingDays = 150 
                        },
                        new CropType { 
                            Name = "Nohut", 
                            Description = "Nohut, protein açısından zengin bir baklagil bitkisidir.", 
                            GrowingSeason = "İlkbahar-Yaz", 
                            GrowingDays = 100 
                        },
                        new CropType { 
                            Name = "Mercimek", 
                            Description = "Mercimek, protein açısından zengin bir baklagil bitkisidir.", 
                            GrowingSeason = "Sonbahar-İlkbahar", 
                            GrowingDays = 120 
                        },
                        new CropType { 
                            Name = "Fasulye", 
                            Description = "Fasulye, protein açısından zengin bir baklagil bitkisidir.", 
                            GrowingSeason = "İlkbahar-Yaz", 
                            GrowingDays = 90 
                        }
                    };

                    // Ekim türlerini veritabanına ekle
                    await context.CropTypes.AddRangeAsync(cropTypes);
                    await context.SaveChangesAsync();

                    Console.WriteLine("Ekim türleri başarıyla eklendi.");
                }
                else
                {
                    Console.WriteLine("Ekim türleri zaten mevcut.");
                    
                    // Mevcut ekim türlerini listele
                    var existingCropTypes = await context.CropTypes.ToListAsync();
                    Console.WriteLine($"Toplam {existingCropTypes.Count} ekim türü bulundu:");
                    
                    foreach (var cropType in existingCropTypes)
                    {
                        Console.WriteLine($"ID: {cropType.Id}, Ad: {cropType.Name}, Açıklama: {cropType.Description}, Büyüme Sezonu: {cropType.GrowingSeason}, Büyüme Günleri: {cropType.GrowingDays}");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Hata oluştu: {ex.Message}");
                Console.WriteLine(ex.StackTrace);
            }
            
            Console.WriteLine("Çıkmak için bir tuşa basın...");
            Console.ReadKey();
        }
    }
}
