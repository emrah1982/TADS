using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TADS.API.Models;

namespace TADS.API.Data
{
    public class CropTypeSeeder
    {
        private readonly ApplicationDbContext _context;

        public CropTypeSeeder(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task SeedAsync()
        {
            // Veritabanında ekim türleri var mı kontrol et
            var cropTypesExist = await _context.CropTypes.AnyAsync();

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
                await _context.CropTypes.AddRangeAsync(cropTypes);
                await _context.SaveChangesAsync();

                Console.WriteLine("Ekim türleri başarıyla eklendi.");
            }
            else
            {
                Console.WriteLine("Ekim türleri zaten mevcut.");
            }
        }
    }
}
