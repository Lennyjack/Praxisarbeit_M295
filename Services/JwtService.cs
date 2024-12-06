using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace Praxisarbeit_M295.Services
{
    // Interface für JWT-Token-Service
    public interface IJwtService
    {
        string GenerateToken(string username, string role); // Token generieren
    }

    // Implementierung des JWT-Service
    public class JwtService : IJwtService
    {
        private readonly string _key; // Geheimschlüssel für die Signierung
        private readonly int _expiryDurationInHours; // Dauer, wie lange der Token gültig ist

        // Konstruktor für den Service, Geheimschlüssel und Ablaufzeit angeben
        public JwtService(string key, int expiryDurationInHours = 2)
        {
            _key = key;
            _expiryDurationInHours = expiryDurationInHours; // Standardwert: 2 Stunden
        }

        // Token-Generierung
        public string GenerateToken(string username, string role)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.UTF8.GetBytes(_key); // Geheimschlüssel konvertieren

            // Claims (Informationen) für den Token
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, username), // Benutzername
                new Claim(ClaimTypes.Role, role) // Rolle des Benutzers
            };

            // Token-Descriptor erstellen (enthält alle Details des Tokens)
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims), // Claims hinzufügen
                Expires = DateTime.UtcNow.AddHours(_expiryDurationInHours), // Ablaufzeit
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key), // Geheimschlüssel
                    SecurityAlgorithms.HmacSha256Signature // Signaturalgorithmus
                )
            };

            // Token erstellen und zurückgeben
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }
}
