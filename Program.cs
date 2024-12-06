using Microsoft.EntityFrameworkCore;
using Praxisarbeit_M295.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Praxisarbeit_M295.Services;

var builder = WebApplication.CreateBuilder(args);

// Geheimschlüssel für JWT aus der Konfigurationsdatei
var key = builder.Configuration["JwtSettings:Key"] ?? "SuperSecretKey123!"; // Fallback, falls der Key fehlt

// Konfiguriere den DbContext für SQL Server
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Konfiguriere Authentifikation mit JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true, // Optional: Herausgeber (Issuer) validieren
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"], // Herausgeber festlegen
            ValidateAudience = true, // Optional: Zielgruppe (Audience) validieren
            ValidAudience = builder.Configuration["JwtSettings:Audience"], // Zielgruppe festlegen
            ValidateLifetime = true, // Ablaufzeit des Tokens validieren
            ValidateIssuerSigningKey = true, // Signatur des Tokens validieren
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)) // Geheimschlüssel
        };
    });

// Füge den JwtService als Singleton hinzu
builder.Services.AddSingleton<IJwtService>(sp =>
    new JwtService(key, int.Parse(builder.Configuration["JwtSettings:ExpiryHours"] ?? "2"))); // Ablaufzeit aus der Konfiguration

// Füge Controller-Dienste hinzu
builder.Services.AddControllers();

// Konfiguriere Swagger für API-Dokumentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Erstelle die Anwendung
var app = builder.Build();

// Konfiguriere die HTTP-Pipeline für die Entwicklungsumgebung
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Aktiviere HTTPS-Umleitungen
app.UseHttpsRedirection();

// Aktiviere Authentifikations- und Autorisierungs-Middleware
app.UseAuthentication();
app.UseAuthorization();

// Mappe die Controller
app.MapControllers();

// Starte die Anwendung
app.Run();
