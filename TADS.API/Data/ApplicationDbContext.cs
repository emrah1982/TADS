using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TADS.API.Models;

namespace TADS.API.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    // Menü ve izin yönetimi için DbSet'ler
    public DbSet<Menu> Menus { get; set; }
    public DbSet<MenuItem> MenuItems { get; set; }
    public DbSet<MenuRolePermission> MenuRolePermissions { get; set; }
    public DbSet<MenuItemRolePermission> MenuItemRolePermissions { get; set; }
    public DbSet<RoutePermission> RoutePermissions { get; set; }
    public DbSet<RouteRolePermission> RouteRolePermissions { get; set; }

    // Parsel/Alan yönetimi için DbSet'ler
    public DbSet<Field> Fields { get; set; }
    public DbSet<SoilType> SoilTypes { get; set; }
    public DbSet<CropType> CropTypes { get; set; }
    public DbSet<FieldHistory> FieldHistories { get; set; }
    public DbSet<FieldSensorData> FieldSensorData { get; set; }
    public DbSet<FieldImage> FieldImages { get; set; }
    public DbSet<FieldCropRotation> FieldCropRotations { get; set; }
    
    // Sulama ve Gübreleme yönetimi için DbSet'ler
    public DbSet<Irrigation> Irrigations { get; set; }
    public DbSet<Fertilization> Fertilizations { get; set; }
    public DbSet<FertilizerType> FertilizerTypes { get; set; }
    public DbSet<Yield> Yields { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Menü ve alt menü öğeleri arasındaki ilişkiyi yapılandır
        builder.Entity<MenuItem>()
            .HasOne(mi => mi.Menu)
            .WithMany(m => m.MenuItems)
            .HasForeignKey(mi => mi.MenuId)
            .OnDelete(DeleteBehavior.Cascade);

        // Menü rol izinleri ilişkisini yapılandır
        builder.Entity<MenuRolePermission>()
            .HasOne(mrp => mrp.Menu)
            .WithMany(m => m.RolePermissions)
            .HasForeignKey(mrp => mrp.MenuId)
            .OnDelete(DeleteBehavior.Cascade);

        // Alt menü rol izinleri ilişkisini yapılandır
        builder.Entity<MenuItemRolePermission>()
            .HasOne(mirp => mirp.MenuItem)
            .WithMany(mi => mi.RolePermissions)
            .HasForeignKey(mirp => mirp.MenuItemId)
            .OnDelete(DeleteBehavior.Cascade);

        // Rota rol izinleri ilişkisini yapılandır
        builder.Entity<RouteRolePermission>()
            .HasOne(rrp => rrp.RoutePermission)
            .WithMany(rp => rp.RolePermissions)
            .HasForeignKey(rrp => rrp.RoutePermissionId)
            .OnDelete(DeleteBehavior.Cascade);

        // Parsel ve toprak türü ilişkisini yapılandır
        builder.Entity<Field>()
            .HasOne(f => f.SoilType)
            .WithMany(st => st.Fields)
            .HasForeignKey(f => f.SoilTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        // Parsel ve ekin türü ilişkisini yapılandır
        builder.Entity<Field>()
            .HasOne(f => f.CropType)
            .WithMany(ct => ct.Fields)
            .HasForeignKey(f => f.CropTypeId)
            .OnDelete(DeleteBehavior.Restrict);

        // Parsel geçmişi ilişkisini yapılandır
        builder.Entity<FieldHistory>()
            .HasOne(fh => fh.Field)
            .WithMany(f => f.FieldHistories)
            .HasForeignKey(fh => fh.FieldId)
            .OnDelete(DeleteBehavior.Cascade);

        // Parsel sensör verileri ilişkisini yapılandır
        builder.Entity<FieldSensorData>()
            .HasOne(fsd => fsd.Field)
            .WithMany(f => f.SensorData)
            .HasForeignKey(fsd => fsd.FieldId)
            .OnDelete(DeleteBehavior.Cascade);

        // Parsel görüntüleri ilişkisini yapılandır
        builder.Entity<FieldImage>()
            .HasOne(fi => fi.Field)
            .WithMany(f => f.Images)
            .HasForeignKey(fi => fi.FieldId)
            .OnDelete(DeleteBehavior.Cascade);

        // Ekim nöbeti ilişkisini yapılandır
        builder.Entity<FieldCropRotation>()
            .HasOne(fcr => fcr.Field)
            .WithMany(f => f.CropRotations)
            .HasForeignKey(fcr => fcr.FieldId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Entity<FieldCropRotation>()
            .HasOne(fcr => fcr.CropType)
            .WithMany(ct => ct.CropRotations)
            .HasForeignKey(fcr => fcr.CropTypeId)
            .OnDelete(DeleteBehavior.Restrict);
            
        // Sulama ilişkisini yapılandır
        builder.Entity<Irrigation>()
            .HasOne(i => i.Field)
            .WithMany(f => f.Irrigations)
            .HasForeignKey(i => i.FieldId)
            .OnDelete(DeleteBehavior.Cascade);
            
        // Gübreleme ilişkisini yapılandır
        builder.Entity<Fertilization>()
            .HasOne(f => f.Field)
            .WithMany(f => f.Fertilizations)
            .HasForeignKey(f => f.FieldId)
            .OnDelete(DeleteBehavior.Cascade);
            
        builder.Entity<Fertilization>()
            .HasOne(f => f.FertilizerType)
            .WithMany(ft => ft.Fertilizations)
            .HasForeignKey(f => f.FertilizerTypeId)
            .OnDelete(DeleteBehavior.Restrict);
            
        // Verim ilişkisini yapılandır
        builder.Entity<Yield>()
            .HasOne(y => y.Field)
            .WithMany(f => f.Yields)
            .HasForeignKey(y => y.FieldId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
