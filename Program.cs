using Microsoft.EntityFrameworkCore;
using Praxisarbeit_M295.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Praxisarbeit_M295.Services;
using Praxisarbeit_M295.Models;

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
            ValidateIssuer = true,
            ValidIssuer = issuer,
            ValidateAudience = true,
            ValidAudience = audience,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
        };

        // **JWT-Fehlerprotokollierung**
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = async context =>
            {
                Console.WriteLine($"Token-Validierung fehlgeschlagen: {context.Exception.Message}");
                if (context.HttpContext.RequestServices.GetService<ApplicationDbContext>() is ApplicationDbContext dbContext)
                {
                    await dbContext.Logs.AddAsync(new Log
                    {
                        Endpoint = context.HttpContext.Request.Path,
                        HttpMethod = context.HttpContext.Request.Method,
                        Message = $"Token-Validierung fehlgeschlagen: {context.Exception.Message}",
                        StatusCode = "401",
                        Timestamp = DateTime.UtcNow
                    });
                    await dbContext.SaveChangesAsync();
                }
            },
            OnTokenValidated = async context =>
            {
                var username = context.Principal.Identity?.Name;
                Console.WriteLine($"Token für Benutzer {username} validiert.");
                if (context.HttpContext.RequestServices.GetService<ApplicationDbContext>() is ApplicationDbContext dbContext)
                {
                    await dbContext.Logs.AddAsync(new Log
                    {
                        Endpoint = context.HttpContext.Request.Path,
                        HttpMethod = context.HttpContext.Request.Method,
                        Message = $"Token validiert für Benutzer: {username}",
                        StatusCode = "200",
                        Timestamp = DateTime.UtcNow
                    });
                    await dbContext.SaveChangesAsync();
                }
            }
        };
    });

// **JwtService registrieren**
builder.Services.AddSingleton<IJwtService>(sp =>
    new JwtService(key, expiryHours, issuer, audience));

// **Controller-Dienste hinzufügen**
builder.Services.AddControllers();

// **CORS-Konfiguration hinzufügen**
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// **Swagger für API-Dokumentation**
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// **Anwendung erstellen**
var app = builder.Build();

// **HTTP-Pipeline konfigurieren**
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

// **Middleware aktivieren**
app.UseMiddleware<RequestLoggingMiddleware>();
app.UseCors("AllowAll"); // CORS direkt hier anwenden
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

// **Controller-Routing**
app.MapControllers();

// **Anwendung starten**
app.Run();
