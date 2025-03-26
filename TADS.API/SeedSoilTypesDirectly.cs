using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using TADS.API.Data;
using TADS.API.Models;

namespace TADS.API
{
    public class SeedSoilTypesDirectly
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("Toprak türleri veritabanına ekleniyor...");
            
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

                // Veritabanında toprak türleri var mı kontrol et
                var soilTypesExist = await context.SoilTypes.AnyAsync();
                Console.WriteLine($"Veritabanında toprak türleri mevcut mu: {soilTypesExist}");

                if (!soilTypesExist)
                {
                    Console.WriteLine("Toprak türleri ekleniyor...");

                    // Önceden tanımlı toprak türleri
                    var soilTypes = new[]
                    {
                        new SoilType { Name = "Kumlu Toprak", Description = "Kumlu topraklar, kum oranı yüksek olan topraklardır. Su tutma kapasitesi düşüktür ancak iyi havalanır." },
                        new SoilType { Name = "Killi Toprak", Description = "Killi topraklar, kil oranı yüksek olan topraklardır. Su tutma kapasitesi yüksektir ancak havalanması zayıftır." },
                        new SoilType { Name = "Tınlı Toprak", Description = "Tınlı topraklar, kum, kil ve silt oranları dengeli olan topraklardır. Tarım için en ideal toprak türüdür." },
                        new SoilType { Name = "Kireçli Toprak", Description = "Kireçli topraklar, kireç oranı yüksek olan topraklardır. pH değeri yüksektir ve bazı bitkiler için uygun değildir." },
                        new SoilType { Name = "Humuslu Toprak", Description = "Humuslu topraklar, organik madde oranı yüksek olan topraklardır. Besin değeri yüksektir ve bitki gelişimi için idealdir." },
                        new SoilType { Name = "Çakıllı Toprak", Description = "Çakıllı topraklar, çakıl oranı yüksek olan topraklardır. Su tutma kapasitesi düşüktür ve tarım için uygun değildir." },
                        new SoilType { Name = "Balçık", Description = "Balçık, kil ve silt oranı yüksek olan topraklardır. Su tutma kapasitesi yüksektir ve tarım için uygundur." },
                        new SoilType { Name = "Alüvyal Toprak", Description = "Alüvyal topraklar, akarsuların taşıdığı malzemelerin birikmesiyle oluşan topraklardır. Besin değeri yüksektir." },
                        new SoilType { Name = "Volkanik Toprak", Description = "Volkanik topraklar, volkanik faaliyetler sonucu oluşan topraklardır. Mineral bakımından zengindir." },
                        new SoilType { Name = "Turba", Description = "Turba, organik maddenin bataklık koşullarında birikmesiyle oluşan topraklardır. Asitliği yüksektir." }
                    };

                    // Toprak türlerini veritabanına ekle
                    await context.SoilTypes.AddRangeAsync(soilTypes);
                    await context.SaveChangesAsync();

                    Console.WriteLine("Toprak türleri başarıyla eklendi.");
                }
                else
                {
                    Console.WriteLine("Toprak türleri zaten mevcut.");
                    
                    // Mevcut toprak türlerini listele
                    var existingSoilTypes = await context.SoilTypes.ToListAsync();
                    Console.WriteLine($"Toplam {existingSoilTypes.Count} toprak türü bulundu:");
                    
                    foreach (var soilType in existingSoilTypes)
                    {
                        Console.WriteLine($"ID: {soilType.Id}, Ad: {soilType.Name}, Açıklama: {soilType.Description}");
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
