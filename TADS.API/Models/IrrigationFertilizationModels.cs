using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TADS.API.Models
{
    /// <summary>
    /// Sulama kaydını temsil eder
    /// </summary>
    public class Irrigation
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int FieldId { get; set; }
        
        [ForeignKey("FieldId")]
        public virtual Field? Field { get; set; }
        
        [Required]
        [Column(TypeName = "date")]
        public DateTime Date { get; set; } = DateTime.Now;
        
        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Amount { get; set; } = 0; // Litre veya m³ cinsinden
        
        [Required]
        [StringLength(50)]
        public string Unit { get; set; } = "litre"; // "Litre" veya "m³"
        
        [StringLength(50)]
        public string Method { get; set; } = "Damla"; // Damla, yağmurlama, salma vb.
        
        [StringLength(500)]
        public string Notes { get; set; } = string.Empty;
        
        // Toprak nem değeri (sensörden)
        [Column(TypeName = "decimal(5, 2)")]
        public decimal? SoilMoistureBeforeIrrigation { get; set; } // %
        
        [Column(TypeName = "decimal(5, 2)")]
        public decimal? SoilMoistureAfterIrrigation { get; set; } // %
        
        // Sezon bilgisi
        [Required]
        public int Season { get; set; } = DateTime.Now.Year; // Yıl olarak (örn. 2025)
        
        // Oluşturma ve güncelleme bilgileri
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        [StringLength(450)]
        public string? CreatedBy { get; set; }
        
        public DateTime? UpdatedAt { get; set; }
        
        [StringLength(450)]
        public string? UpdatedBy { get; set; }
    }
    
    /// <summary>
    /// Gübreleme kaydını temsil eder
    /// </summary>
    public class Fertilization
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int FieldId { get; set; }
        
        [ForeignKey("FieldId")]
        public virtual Field? Field { get; set; }
        
        [Required]
        [Column(TypeName = "date")]
        public DateTime Date { get; set; } = DateTime.Now;
        
        [Required]
        public int FertilizerTypeId { get; set; }
        
        [ForeignKey("FertilizerTypeId")]
        public virtual FertilizerType? FertilizerType { get; set; }
        
        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Amount { get; set; } = 0; // kg veya lt
        
        [StringLength(50)]
        public string? Unit { get; set; } = "kg"; // "kg/da" veya "lt/da"
        
        [StringLength(50)]
        public string? Method { get; set; } = "Manuel"; // Serpme, damla, yaprak gübresi vb.
        
        [StringLength(500)]
        public string? Notes { get; set; } = string.Empty;
        
        // Sezon bilgisi
        [Required]
        public int Season { get; set; } = DateTime.Now.Year; // Yıl olarak (örn. 2025)
        
        // Oluşturma ve güncelleme bilgileri
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        [StringLength(450)]
        public string? CreatedBy { get; set; }
        
        public DateTime? UpdatedAt { get; set; }
        
        [StringLength(450)]
        public string? UpdatedBy { get; set; }
    }
    
    /// <summary>
    /// Gübre türlerini temsil eder
    /// </summary>
    public class FertilizerType
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty; // Azot, Fosfor, Potasyum vb.
        
        [StringLength(50)]
        public string Category { get; set; } = string.Empty; // Organik, Kimyasal vb.
        
        [StringLength(10)]
        public string NPK { get; set; } = string.Empty; // NPK değeri (örn: 15-15-15)
        
        [StringLength(500)]
        public string Description { get; set; } = string.Empty;
        
        // İlişkiler
        public virtual ICollection<Fertilization> Fertilizations { get; set; }
    }
    
    /// <summary>
    /// Sensör verilerini temsil eder (IoT entegrasyonu için)
    /// </summary>
    // Bu sınıf başka bir dosyada tanımlandığı için burada yorum satırına alındı
    /*
    public class FieldSensorData
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int FieldId { get; set; }
        
        [ForeignKey("FieldId")]
        public virtual Field? Field { get; set; }
        
        [Required]
        public DateTime Timestamp { get; set; } = DateTime.Now;
        
        [Column(TypeName = "decimal(5, 2)")]
        public decimal? SoilMoisture { get; set; }
        
        [Column(TypeName = "decimal(5, 2)")]
        public decimal? SoilTemperature { get; set; }
        
        [Column(TypeName = "decimal(5, 2)")]
        public decimal? AirTemperature { get; set; }
        
        [Column(TypeName = "decimal(5, 2)")]
        public decimal? Humidity { get; set; }
        
        [Column(TypeName = "decimal(5, 2)")]
        public decimal? Rainfall { get; set; }
        
        [StringLength(50)]
        public string SensorId { get; set; } = string.Empty;
        
        [StringLength(500)]
        public string Notes { get; set; } = string.Empty;
        
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        [StringLength(100)]
        public string CreatedBy { get; set; } = string.Empty;
    }
    */
    
    /// <summary>
    /// Verim kaydını temsil eder
    /// </summary>
    public class Yield
    {
        [Key]
        public int Id { get; set; }
        
        [Required]
        public int FieldId { get; set; }
        
        [ForeignKey("FieldId")]
        public virtual Field? Field { get; set; }
        
        [Required]
        public int Season { get; set; } = DateTime.Now.Year; // Yıl
        
        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Amount { get; set; } = 0; // kg veya ton
        
        [Required]
        [StringLength(50)]
        public string Unit { get; set; } = "kg"; // "kg/da" veya "ton/da"
        
        [StringLength(500)]
        public string Notes { get; set; } = string.Empty;
        
        // Oluşturma ve güncelleme bilgileri
        public DateTime CreatedAt { get; set; } = DateTime.Now;
        
        [StringLength(450)]
        public string? CreatedBy { get; set; }
    }
}
