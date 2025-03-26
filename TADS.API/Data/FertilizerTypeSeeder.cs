using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TADS.API.Models;

namespace TADS.API.Data
{
    public class FertilizerTypeSeeder
    {
        private readonly ApplicationDbContext _context;

        public FertilizerTypeSeeder(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            // Veritabanında gübre türleri var mı kontrol et
            var fertilizerTypesExist = await _context.FertilizerTypes.AnyAsync();

            if (!fertilizerTypesExist)
            {
                Console.WriteLine("Gübre türleri ekleniyor...");

                // Önceden tanımlı gübre türleri
                var fertilizerTypes = new[]
                {
                    new FertilizerType { 
                        Name = "Üre", 
                        Category = "Azotlu Gübre", 
                        NPK = "46-0-0", 
                        Description = "Yüksek azot içerikli, yaygın olarak kullanılan bir gübre türüdür. Bitkilerin yeşil aksamının gelişimini destekler."
                    },
                    new FertilizerType { 
                        Name = "Amonyum Sülfat", 
                        Category = "Azotlu Gübre", 
                        NPK = "21-0-0", 
                        Description = "Azot ve kükürt içeren, asidik topraklar için uygun olmayan bir gübre türüdür."
                    },
                    new FertilizerType { 
                        Name = "Amonyum Nitrat", 
                        Category = "Azotlu Gübre", 
                        NPK = "33-0-0", 
                        Description = "Hem nitrat hem de amonyum formunda azot içeren, hızlı ve uzun süreli etki sağlayan bir gübre türüdür."
                    },
                    new FertilizerType { 
                        Name = "DAP (Diamonyum Fosfat)", 
                        Category = "Azotlu ve Fosforlu Gübre", 
                        NPK = "18-46-0", 
                        Description = "Yüksek fosfor ve azot içerikli, tohum ekimi öncesi yaygın olarak kullanılan bir gübre türüdür."
                    },
                    new FertilizerType { 
                        Name = "TSP (Triple Süper Fosfat)", 
                        Category = "Fosforlu Gübre", 
                        NPK = "0-46-0", 
                        Description = "Yüksek fosfor içerikli, bitkilerin kök gelişimini destekleyen bir gübre türüdür."
                    },
                    new FertilizerType { 
                        Name = "Potasyum Sülfat", 
                        Category = "Potasyumlu Gübre", 
                        NPK = "0-0-50", 
                        Description = "Yüksek potasyum içerikli, meyve kalitesini ve bitki dayanıklılığını artıran bir gübre türüdür."
                    },
                    new FertilizerType { 
                        Name = "Potasyum Nitrat", 
                        Category = "Potasyumlu ve Azotlu Gübre", 
                        NPK = "13-0-46", 
                        Description = "Potasyum ve nitrat azotu içeren, özellikle meyve ve sebze yetiştiriciliğinde kullanılan bir gübre türüdür."
                    },
                    new FertilizerType { 
                        Name = "NPK 15-15-15", 
                        Category = "Kompoze Gübre", 
                        NPK = "15-15-15", 
                        Description = "Azot, fosfor ve potasyumu eşit oranda içeren, genel amaçlı kullanılan dengeli bir gübre türüdür."
                    },
                    new FertilizerType { 
                        Name = "NPK 20-20-0", 
                        Category = "Kompoze Gübre", 
                        NPK = "20-20-0", 
                        Description = "Azot ve fosfor içeren, tahıl üretiminde yaygın olarak kullanılan bir gübre türüdür."
                    },
                    new FertilizerType { 
                        Name = "Çiftlik Gübresi", 
                        Category = "Organik Gübre", 
                        NPK = "Değişken", 
                        Description = "Hayvan dışkılarından oluşan, toprağı zenginleştiren ve yapısını iyileştiren doğal bir gübre türüdür."
                    },
                    new FertilizerType { 
                        Name = "Kompost", 
                        Category = "Organik Gübre", 
                        NPK = "Değişken", 
                        Description = "Bitkisel atıkların fermantasyonu sonucu oluşan, toprak yapısını iyileştiren organik bir gübre türüdür."
                    },
                    new FertilizerType { 
                        Name = "Vermikompost (Solucan Gübresi)", 
                        Category = "Organik Gübre", 
                        NPK = "Değişken", 
                        Description = "Solucanların organik atıkları işlemesiyle oluşan, besin değeri yüksek organik bir gübre türüdür."
                    },
                    new FertilizerType { 
                        Name = "Kalsiyum Nitrat", 
                        Category = "Kalsiyumlu ve Azotlu Gübre", 
                        NPK = "15.5-0-0", 
                        Description = "Kalsiyum ve nitrat azotu içeren, kalsiyum eksikliğini gidermek için kullanılan bir gübre türüdür."
                    },
                    new FertilizerType { 
                        Name = "Magnezyum Sülfat (Epsom Tuzu)", 
                        Category = "Magnezyumlu Gübre", 
                        NPK = "0-0-0", 
                        Description = "Magnezyum ve kükürt içeren, klorofil oluşumunu destekleyen bir gübre türüdür."
                    },
                    new FertilizerType { 
                        Name = "Demir Sülfat", 
                        Category = "Mikro Besin Gübresi", 
                        NPK = "0-0-0", 
                        Description = "Demir eksikliğini gidermek için kullanılan, klorofil oluşumunu destekleyen bir gübre türüdür."
                    }
                };

                // Gübre türlerini veritabanına ekle
                await _context.FertilizerTypes.AddRangeAsync(fertilizerTypes);
                await _context.SaveChangesAsync();

                Console.WriteLine("Gübre türleri başarıyla eklendi.");
            }
            else
            {
                Console.WriteLine("Gübre türleri zaten mevcut.");
            }
        }
    }
}
