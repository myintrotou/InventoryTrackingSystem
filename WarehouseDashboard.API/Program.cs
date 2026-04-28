using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;
using WarehouseDashboard.API.Data;
using WarehouseDashboard.API.Models;
using WarehouseDashboard.API.Services;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Configuration
var port = Environment.GetEnvironmentVariable("PORT") ?? "5000";
builder.WebHost.UseUrls($"http://0.0.0.0:{port}");
var keyString = builder.Configuration.GetSection("AppSettings:Token").Value ?? "default_backup_key_for_local_dev_only_32_chars";
var secretKeyObj = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyString));

// Services
builder.Services.AddControllers().AddJsonOptions(o => o.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles);
builder.Services.AddDbContext<WarehouseContext>(o => o.UseSqlite("Data Source=warehouse.db"));
builder.Services.AddScoped<IPdfReportService, PdfReportService>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Auth & CORS
var allowedOrigins = builder.Configuration.GetValue<string>("ALLOWED_ORIGINS")?.Split(',') 
    ?? new[] { "http://localhost:4200", "https://inventory-tracking-system-bykggyajy.vercel.app" };

builder.Services.AddCors(o => o.AddPolicy("AllowAngular", p => p
    .WithOrigins(allowedOrigins)
    .AllowAnyMethod()
    .AllowAnyHeader()));
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(o => {
    o.TokenValidationParameters = new TokenValidationParameters { ValidateIssuerSigningKey = true, IssuerSigningKey = secretKeyObj, ValidateIssuer = false, ValidateAudience = false };
});
builder.Services.AddAuthorization();

var app = builder.Build();

// Middleware
if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }
app.UseCors("AllowAngular");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Smart Seeder
using (var scope = app.Services.CreateScope()) {
    var context = scope.ServiceProvider.GetRequiredService<WarehouseContext>();
    context.Database.EnsureCreated();

    if (!context.Users.Any()) {
        context.Users.Add(new User { Username = "admin", PasswordHash = BCrypt.Net.BCrypt.HashPassword("admin123"), Role = "Admin" });
        context.SaveChanges();
    }

    if (!context.Products.Any()) {
        var admin = context.Users.First(u => u.Username == "admin");
        context.Products.AddRange(new List<Product> {
            new Product { ProductName = "Intel Core i9-13900K", StockQuantity = 12, ReorderLevel = 5, UserID = admin.UserID },
            new Product { ProductName = "NVIDIA RTX 4090", StockQuantity = 3, ReorderLevel = 5, UserID = admin.UserID },
            new Product { ProductName = "Samsung 980 Pro 2TB", StockQuantity = 25, ReorderLevel = 10, UserID = admin.UserID },
            new Product { ProductName = "Corsair Vengeance 32GB RAM", StockQuantity = 18, ReorderLevel = 10, UserID = admin.UserID },
            new Product { ProductName = "ASUS ROG Swift Monitor", StockQuantity = 4, ReorderLevel = 5, UserID = admin.UserID }
        });
        context.SaveChanges();
    }
}

app.Run();
