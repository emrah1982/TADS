using System;
using System.Threading.Tasks;
using TADS.API.Data;

namespace TADS.API
{
    public class SeedData
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("Toprak türleri veritabanına ekleniyor...");
            
            try
            {
                await SeedSoilTypes.SeedData();
                Console.WriteLine("İşlem tamamlandı.");
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
