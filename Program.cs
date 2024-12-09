using Microsoft.EntityFrameworkCore;
using Praxisarbeit_M295.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Praxisarbeit_M295.Services;

var builder = WebApplication.CreateBuilder(args);

// **JWT-Parameter aus Konfiguration**
var jwtSettings = builder.Configuration.GetSection("JwtSettings");
var key = jwtSettings["Key"] ?? "SuperSecretKey123!";
var issuer = jwtSettings["Issuer"] ?? "DefaultIssuer";
var audience = jwtSettings["Audience"] ?? "DefaultAudience";
var expiryHours = int.Parse(jwtSettings["ExpiryHours"] ?? "2");

// **Datenbank-Konfiguration (SQL Server)**
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// **JWT-Authentifizierung konfigurieren**
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true, // Herausgeber validieren
            ValidIssuer = issuer, // Herausgeber
            ValidateAudience = true, // Zielgruppe validieren
            ValidAudience = audience, // Zielgruppe
            ValidateLifetime = true, // Ablaufzeit validieren
            ValidateIssuerSigningKey = true, // Signatur validieren
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key)) // Geheimschlüssel
        };

        // **JWT-Fehlerprotokollierung**
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"Token-Validierung fehlgeschlagen: {context.Exception.Message}");
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                Console.WriteLine($"Token für Benutzer {context.Principal.Identity?.Name} validiert.");
                return Task.CompletedTask;
            }
        };
    });

// **JwtService registrieren**
builder.Services.AddSingleton<IJwtService>(sp =>
    new JwtService(key, expiryHours, issuer, audience));

// **Controller-Dienste hinzufügen**
builder.Services.AddControllers();

// **Swagger für API-Dokumentation**
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// **Anwendung erstellen**
var app = builder.Build();

// **HTTP-Pipeline konfigurieren**
if (app.Environment.IsDevelopment())
{
    // Swagger nur in der Entwicklungsumgebung aktivieren
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    // **Fehlerseiten und Sicherheit in der Produktion**
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// **Middleware aktivieren**
app.UseMiddleware<RequestLoggingMiddleware>(); // Eigene Middleware für Protokollierung
app.UseHttpsRedirection(); // HTTPS erzwingen
app.UseAuthentication(); // JWT-Authentifizierung aktivieren
app.UseAuthorization(); // Autorisierung aktivieren

// **Controller-Routing**
app.MapControllers();

// **Anwendung starten**
app.Run();
