using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace TADS.API.Models.Generated;

public partial class GeneratedDbContext : DbContext
{
    public GeneratedDbContext(DbContextOptions<GeneratedDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Aspnetrole> Aspnetroles { get; set; }

    public virtual DbSet<Aspnetroleclaim> Aspnetroleclaims { get; set; }

    public virtual DbSet<Aspnetuser> Aspnetusers { get; set; }

    public virtual DbSet<Aspnetuserclaim> Aspnetuserclaims { get; set; }

    public virtual DbSet<Aspnetuserlogin> Aspnetuserlogins { get; set; }

    public virtual DbSet<Aspnetusertoken> Aspnetusertokens { get; set; }

    public virtual DbSet<Croptype> Croptypes { get; set; }

    public virtual DbSet<Efmigrationshistory> Efmigrationshistories { get; set; }

    public virtual DbSet<Fertilization> Fertilizations { get; set; }

    public virtual DbSet<Fertilizertype> Fertilizertypes { get; set; }

    public virtual DbSet<Field> Fields { get; set; }

    public virtual DbSet<Fieldcroprotation> Fieldcroprotations { get; set; }

    public virtual DbSet<Fieldhistory> Fieldhistories { get; set; }

    public virtual DbSet<Fieldimage> Fieldimages { get; set; }

    public virtual DbSet<Fieldsensordatum> Fieldsensordata { get; set; }

    public virtual DbSet<Irrigation> Irrigations { get; set; }

    public virtual DbSet<Menu> Menus { get; set; }

    public virtual DbSet<Menuitem> Menuitems { get; set; }

    public virtual DbSet<Menuitemrolepermission> Menuitemrolepermissions { get; set; }

    public virtual DbSet<Menurolepermission> Menurolepermissions { get; set; }

    public virtual DbSet<Routepermission> Routepermissions { get; set; }

    public virtual DbSet<Routerolepermission> Routerolepermissions { get; set; }

    public virtual DbSet<Soiltype> Soiltypes { get; set; }

    public virtual DbSet<Yield> Yields { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder
            .UseCollation("utf8mb4_0900_ai_ci")
            .HasCharSet("utf8mb4");

        modelBuilder.Entity<Aspnetrole>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("aspnetroles");

            entity.HasIndex(e => e.NormalizedName, "RoleNameIndex").IsUnique();

            entity.Property(e => e.Name).HasMaxLength(256);
            entity.Property(e => e.NormalizedName).HasMaxLength(256);
        });

        modelBuilder.Entity<Aspnetroleclaim>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("aspnetroleclaims");

            entity.HasIndex(e => e.RoleId, "IX_AspNetRoleClaims_RoleId");

            entity.HasOne(d => d.Role).WithMany(p => p.Aspnetroleclaims)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK_AspNetRoleClaims_AspNetRoles_RoleId");
        });

        modelBuilder.Entity<Aspnetuser>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("aspnetusers");

            entity.HasIndex(e => e.NormalizedEmail, "EmailIndex");

            entity.HasIndex(e => e.NormalizedUserName, "UserNameIndex").IsUnique();

            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.LockoutEnd).HasMaxLength(6);
            entity.Property(e => e.NormalizedEmail).HasMaxLength(256);
            entity.Property(e => e.NormalizedUserName).HasMaxLength(256);
            entity.Property(e => e.UserName).HasMaxLength(256);

            entity.HasMany(d => d.Roles).WithMany(p => p.Users)
                .UsingEntity<Dictionary<string, object>>(
                    "Aspnetuserrole",
                    r => r.HasOne<Aspnetrole>().WithMany()
                        .HasForeignKey("RoleId")
                        .HasConstraintName("FK_AspNetUserRoles_AspNetRoles_RoleId"),
                    l => l.HasOne<Aspnetuser>().WithMany()
                        .HasForeignKey("UserId")
                        .HasConstraintName("FK_AspNetUserRoles_AspNetUsers_UserId"),
                    j =>
                    {
                        j.HasKey("UserId", "RoleId")
                            .HasName("PRIMARY")
                            .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });
                        j.ToTable("aspnetuserroles");
                        j.HasIndex(new[] { "RoleId" }, "IX_AspNetUserRoles_RoleId");
                    });
        });

        modelBuilder.Entity<Aspnetuserclaim>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("aspnetuserclaims");

            entity.HasIndex(e => e.UserId, "IX_AspNetUserClaims_UserId");

            entity.HasOne(d => d.User).WithMany(p => p.Aspnetuserclaims)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_AspNetUserClaims_AspNetUsers_UserId");
        });

        modelBuilder.Entity<Aspnetuserlogin>(entity =>
        {
            entity.HasKey(e => new { e.LoginProvider, e.ProviderKey })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0 });

            entity.ToTable("aspnetuserlogins");

            entity.HasIndex(e => e.UserId, "IX_AspNetUserLogins_UserId");

            entity.HasOne(d => d.User).WithMany(p => p.Aspnetuserlogins)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_AspNetUserLogins_AspNetUsers_UserId");
        });

        modelBuilder.Entity<Aspnetusertoken>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.LoginProvider, e.Name })
                .HasName("PRIMARY")
                .HasAnnotation("MySql:IndexPrefixLength", new[] { 0, 0, 0 });

            entity.ToTable("aspnetusertokens");

            entity.HasOne(d => d.User).WithMany(p => p.Aspnetusertokens)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK_AspNetUserTokens_AspNetUsers_UserId");
        });

        modelBuilder.Entity<Croptype>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("croptypes");

            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.GrowingSeason).HasMaxLength(50);
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Efmigrationshistory>(entity =>
        {
            entity.HasKey(e => e.MigrationId).HasName("PRIMARY");

            entity.ToTable("__efmigrationshistory");

            entity.Property(e => e.MigrationId).HasMaxLength(150);
            entity.Property(e => e.ProductVersion).HasMaxLength(32);
        });

        modelBuilder.Entity<Fertilization>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("fertilizations");

            entity.HasIndex(e => e.FertilizerTypeId, "IX_Fertilizations_FertilizerTypeId");

            entity.HasIndex(e => e.FieldId, "IX_Fertilizations_FieldId");

            entity.Property(e => e.Amount).HasPrecision(10, 2);
            entity.Property(e => e.CreatedAt).HasMaxLength(6);
            entity.Property(e => e.CreatedBy).HasMaxLength(450);
            entity.Property(e => e.Method).HasMaxLength(50);
            entity.Property(e => e.Notes).HasMaxLength(500);
            entity.Property(e => e.Unit).HasMaxLength(50);
            entity.Property(e => e.UpdatedAt).HasMaxLength(6);
            entity.Property(e => e.UpdatedBy).HasMaxLength(450);

            entity.HasOne(d => d.FertilizerType).WithMany(p => p.Fertilizations)
                .HasForeignKey(d => d.FertilizerTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Fertilizations_FertilizerTypes_FertilizerTypeId");

            entity.HasOne(d => d.Field).WithMany(p => p.Fertilizations)
                .HasForeignKey(d => d.FieldId)
                .HasConstraintName("FK_Fertilizations_Fields_FieldId");
        });

        modelBuilder.Entity<Fertilizertype>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("fertilizertypes");

            entity.Property(e => e.Category).HasMaxLength(50);
            entity.Property(e => e.Description).HasMaxLength(500);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Npk)
                .HasMaxLength(10)
                .HasColumnName("NPK");
        });

        modelBuilder.Entity<Field>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("fields");

            entity.HasIndex(e => e.CropTypeId, "IX_Fields_CropTypeId");

            entity.HasIndex(e => e.SoilTypeId, "IX_Fields_SoilTypeId");

            entity.Property(e => e.Area).HasPrecision(10, 2);
            entity.Property(e => e.CreatedAt).HasMaxLength(6);
            entity.Property(e => e.CreatedBy).HasMaxLength(450);
            entity.Property(e => e.Latitude).HasPrecision(10, 6);
            entity.Property(e => e.Location).HasMaxLength(200);
            entity.Property(e => e.Longitude).HasPrecision(10, 6);
            entity.Property(e => e.Name).HasMaxLength(100);
            entity.Property(e => e.Notes).HasMaxLength(500);
            entity.Property(e => e.UpdatedAt).HasMaxLength(6);
            entity.Property(e => e.UpdatedBy).HasMaxLength(450);

            entity.HasOne(d => d.CropType).WithMany(p => p.Fields)
                .HasForeignKey(d => d.CropTypeId)
                .HasConstraintName("FK_Fields_CropTypes_CropTypeId");

            entity.HasOne(d => d.SoilType).WithMany(p => p.Fields)
                .HasForeignKey(d => d.SoilTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Fields_SoilTypes_SoilTypeId");
        });

        modelBuilder.Entity<Fieldcroprotation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("fieldcroprotations");

            entity.HasIndex(e => e.CropTypeId, "IX_FieldCropRotations_CropTypeId");

            entity.HasIndex(e => e.FieldId, "IX_FieldCropRotations_FieldId");

            entity.Property(e => e.Notes).HasMaxLength(500);

            entity.HasOne(d => d.CropType).WithMany(p => p.Fieldcroprotations)
                .HasForeignKey(d => d.CropTypeId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_FieldCropRotations_CropTypes_CropTypeId");

            entity.HasOne(d => d.Field).WithMany(p => p.Fieldcroprotations)
                .HasForeignKey(d => d.FieldId)
                .HasConstraintName("FK_FieldCropRotations_Fields_FieldId");
        });

        modelBuilder.Entity<Fieldhistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("fieldhistories");

            entity.HasIndex(e => e.FieldId, "IX_FieldHistories_FieldId");

            entity.Property(e => e.ActionBy).HasMaxLength(450);
            entity.Property(e => e.ActionDate).HasMaxLength(6);
            entity.Property(e => e.ActionType).HasMaxLength(50);
            entity.Property(e => e.Cost).HasPrecision(10, 2);
            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.Notes).HasMaxLength(500);

            entity.HasOne(d => d.Field).WithMany(p => p.Fieldhistories)
                .HasForeignKey(d => d.FieldId)
                .HasConstraintName("FK_FieldHistories_Fields_FieldId");
        });

        modelBuilder.Entity<Fieldimage>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("fieldimages");

            entity.HasIndex(e => e.FieldId, "IX_FieldImages_FieldId");

            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.ImagePath).HasMaxLength(255);
            entity.Property(e => e.ImageType).HasMaxLength(50);
            entity.Property(e => e.Title).HasMaxLength(100);
            entity.Property(e => e.UploadDate).HasMaxLength(6);

            entity.HasOne(d => d.Field).WithMany(p => p.Fieldimages)
                .HasForeignKey(d => d.FieldId)
                .HasConstraintName("FK_FieldImages_Fields_FieldId");
        });

        modelBuilder.Entity<Fieldsensordatum>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("fieldsensordata");

            entity.HasIndex(e => e.FieldId, "IX_FieldSensorData_FieldId");

            entity.Property(e => e.Location).HasMaxLength(200);
            entity.Property(e => e.ReadingTime).HasMaxLength(6);
            entity.Property(e => e.SensorId).HasMaxLength(100);
            entity.Property(e => e.SensorType).HasMaxLength(50);
            entity.Property(e => e.Unit).HasMaxLength(20);
            entity.Property(e => e.Value).HasPrecision(10, 2);

            entity.HasOne(d => d.Field).WithMany(p => p.Fieldsensordata)
                .HasForeignKey(d => d.FieldId)
                .HasConstraintName("FK_FieldSensorData_Fields_FieldId");
        });

        modelBuilder.Entity<Irrigation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("irrigations");

            entity.HasIndex(e => e.FieldId, "IX_Irrigations_FieldId");

            entity.Property(e => e.Amount).HasPrecision(10, 2);
            entity.Property(e => e.CreatedAt).HasMaxLength(6);
            entity.Property(e => e.CreatedBy).HasMaxLength(450);
            entity.Property(e => e.Method).HasMaxLength(50);
            entity.Property(e => e.Notes).HasMaxLength(500);
            entity.Property(e => e.SoilMoistureAfterIrrigation).HasPrecision(5, 2);
            entity.Property(e => e.SoilMoistureBeforeIrrigation).HasPrecision(5, 2);
            entity.Property(e => e.Unit).HasMaxLength(50);
            entity.Property(e => e.UpdatedAt).HasMaxLength(6);
            entity.Property(e => e.UpdatedBy).HasMaxLength(450);

            entity.HasOne(d => d.Field).WithMany(p => p.Irrigations)
                .HasForeignKey(d => d.FieldId)
                .HasConstraintName("FK_Irrigations_Fields_FieldId");
        });

        modelBuilder.Entity<Menu>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("menus");

            entity.Property(e => e.IconName).HasMaxLength(50);
            entity.Property(e => e.Title).HasMaxLength(100);
        });

        modelBuilder.Entity<Menuitem>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("menuitems");

            entity.HasIndex(e => e.MenuId, "IX_MenuItems_MenuId");

            entity.Property(e => e.IconName).HasMaxLength(50);
            entity.Property(e => e.Path).HasMaxLength(100);
            entity.Property(e => e.Title).HasMaxLength(100);

            entity.HasOne(d => d.Menu).WithMany(p => p.Menuitems)
                .HasForeignKey(d => d.MenuId)
                .HasConstraintName("FK_MenuItems_Menus_MenuId");
        });

        modelBuilder.Entity<Menuitemrolepermission>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("menuitemrolepermissions");

            entity.HasIndex(e => e.MenuItemId, "IX_MenuItemRolePermissions_MenuItemId");

            entity.Property(e => e.RoleName).HasMaxLength(50);

            entity.HasOne(d => d.MenuItem).WithMany(p => p.Menuitemrolepermissions)
                .HasForeignKey(d => d.MenuItemId)
                .HasConstraintName("FK_MenuItemRolePermissions_MenuItems_MenuItemId");
        });

        modelBuilder.Entity<Menurolepermission>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("menurolepermissions");

            entity.HasIndex(e => e.MenuId, "IX_MenuRolePermissions_MenuId");

            entity.Property(e => e.RoleName).HasMaxLength(50);

            entity.HasOne(d => d.Menu).WithMany(p => p.Menurolepermissions)
                .HasForeignKey(d => d.MenuId)
                .HasConstraintName("FK_MenuRolePermissions_Menus_MenuId");
        });

        modelBuilder.Entity<Routepermission>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("routepermissions");

            entity.Property(e => e.Description).HasMaxLength(100);
            entity.Property(e => e.Path).HasMaxLength(100);
        });

        modelBuilder.Entity<Routerolepermission>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("routerolepermissions");

            entity.HasIndex(e => e.RoutePermissionId, "IX_RouteRolePermissions_RoutePermissionId");

            entity.Property(e => e.RoleName).HasMaxLength(50);

            entity.HasOne(d => d.RoutePermission).WithMany(p => p.Routerolepermissions)
                .HasForeignKey(d => d.RoutePermissionId)
                .HasConstraintName("FK_RouteRolePermissions_RoutePermissions_RoutePermissionId");
        });

        modelBuilder.Entity<Soiltype>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("soiltypes");

            entity.Property(e => e.Description).HasMaxLength(200);
            entity.Property(e => e.Name).HasMaxLength(50);
        });

        modelBuilder.Entity<Yield>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PRIMARY");

            entity.ToTable("yields");

            entity.HasIndex(e => e.FieldId, "IX_Yields_FieldId");

            entity.Property(e => e.Amount).HasPrecision(10, 2);
            entity.Property(e => e.CreatedAt).HasMaxLength(6);
            entity.Property(e => e.CreatedBy).HasMaxLength(450);
            entity.Property(e => e.Notes).HasMaxLength(500);
            entity.Property(e => e.Unit).HasMaxLength(50);

            entity.HasOne(d => d.Field).WithMany(p => p.Yields)
                .HasForeignKey(d => d.FieldId)
                .HasConstraintName("FK_Yields_Fields_FieldId");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
