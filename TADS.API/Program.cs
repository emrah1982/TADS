using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TADS.API.Data;
using TADS.API.Models;
using TADS.API.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

// Configure Kestrel to use port 5000
builder.WebHost.UseUrls("http://localhost:5000");

// Add services to the container.
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });

// Configure CORS
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:3000", "http://localhost:3001", "http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

// Configure Database
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
Console.WriteLine($"Using connection string: {connectionString}");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
    // Enable detailed errors and sensitive data logging for debugging
    options.EnableDetailedErrors();
    options.EnableSensitiveDataLogging();
});

// Configure Identity
builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options => {
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders();

// Configure JWT Authentication
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.SaveToken = true;
    options.RequireHttpsMetadata = false;
    options.TokenValidationParameters = new TokenValidationParameters()
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidAudience = builder.Configuration["JWT:ValidAudience"],
        ValidIssuer = builder.Configuration["JWT:ValidIssuer"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:Secret"]))
    };
});

builder.Services.AddTransient<IEmailService, EmailService>();
builder.Services.AddScoped<MenuPermissionService>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Ensure database is created and apply migrations
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    try
    {
        var context = services.GetRequiredService<ApplicationDbContext>();
        Console.WriteLine("Ensuring database is created...");
        context.Database.EnsureCreated();
        
        // Seed roles
        Console.WriteLine("Seeding roles...");
        var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
        await RoleSeeder.SeedRoles(roleManager);
        Console.WriteLine("Roles seeded successfully");

        // Seed menus
        Console.WriteLine("Seeding menus...");
        var menuSeeder = new MenuSeeder(context);
        await menuSeeder.SeedAsync();
        Console.WriteLine("Menus seeded successfully");
        
        // Seed irrigation and fertilization menus
        Console.WriteLine("Seeding irrigation and fertilization menus...");
        var irrigationFertilizationMenuSeeder = new IrrigationFertilizationMenuSeeder(context);
        await irrigationFertilizationMenuSeeder.SeedAsync();
        Console.WriteLine("Irrigation and fertilization menus seeded successfully");
        
        // Seed soil types
        Console.WriteLine("Seeding soil types...");
        var soilTypeSeeder = new SoilTypeSeeder(context);
        await soilTypeSeeder.SeedAsync();
        Console.WriteLine("Soil types seeded successfully");
        
        // Seed crop types
        Console.WriteLine("Seeding crop types...");
        var cropTypeSeeder = new CropTypeSeeder(context);
        await cropTypeSeeder.SeedAsync();
        Console.WriteLine("Crop types seeded successfully");
        
        // Seed fertilizer types
        Console.WriteLine("Seeding fertilizer types...");
        var fertilizerTypeSeeder = new FertilizerTypeSeeder(context);
        await fertilizerTypeSeeder.SeedAsync();
        Console.WriteLine("Fertilizer types seeded successfully");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"An error occurred while setting up the database: {ex.Message}");
        Console.WriteLine($"Stack trace: {ex.StackTrace}");
    }
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // Detaylı hata sayfasını etkinleştir
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
