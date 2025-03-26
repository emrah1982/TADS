using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;
using TADS.API.Data;
using TADS.API.Models;

namespace TADS.API
{
    public class CheckSoilTypes
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("Veritabanındaki toprak türleri kontrol ediliyor...");
            
            try
            {
                // Veritabanı bağlantı bilgilerini al
                var connectionString = "server=localhost;database=tadsdb;user=root;password=secgem;port=3306";
                var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
                optionsBuilder.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));

                // ApplicationDbContext oluştur
                using (var context = new ApplicationDbContext(optionsBuilder.Options))
                {
                    // Toprak türlerini kontrol et
                    var soilTypes = await context.SoilTypes.ToListAsync();
                    
                    if (soilTypes.Any())
                    {
                        Console.WriteLine($"Toplam {soilTypes.Count} toprak türü bulundu:");
                        
                        foreach (var soilType in soilTypes)
                        {
                            Console.WriteLine($"ID: {soilType.Id}, Ad: {soilType.Name}, Açıklama: {soilType.Description}");
                        }
                    }
                    else
                    {
                        Console.WriteLine("Veritabanında hiç toprak türü bulunamadı.");
                        
                        // Toprak türlerini ekle
                        Console.WriteLine("Toprak türleri ekleniyor...");
                        
                        var newSoilTypes = new[]
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
                        
                        await context.SoilTypes.AddRangeAsync(newSoilTypes);
                        await context.SaveChangesAsync();
                        
                        Console.WriteLine("Toprak türleri başarıyla eklendi.");
                        
                        // Eklenen toprak türlerini listele
                        soilTypes = await context.SoilTypes.ToListAsync();
                        Console.WriteLine($"Toplam {soilTypes.Count} toprak türü eklendi:");
                        
                        foreach (var soilType in soilTypes)
                        {
                            Console.WriteLine($"ID: {soilType.Id}, Ad: {soilType.Name}, Açıklama: {soilType.Description}");
                        }
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
