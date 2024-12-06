using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Praxisarbeit_M295.Data;
using Praxisarbeit_M295.Models;
using Praxisarbeit_M295.Services;
using System.Security.Cryptography;
using System.Text;

namespace Praxisarbeit_M295.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;
        private readonly IJwtService _jwtService;

        public UsersController(ApplicationDbContext context, IJwtService jwtService)
        {
            _context = context;
            _jwtService = jwtService;
        }

        // POST: api/Users/Login
        [HttpPost("Login")]
        public async Task<ActionResult<string>> Login([FromBody] UserLoginDto loginDto)
        {
            var user = await _context.Users
                .SingleOrDefaultAsync(u => u.Username == loginDto.Username);

            if (user == null || !VerifyPasswordHash(loginDto.Password, user.PasswordHash))
            {
                return Unauthorized("Falscher Benutzername oder Passwort.");
            }

            var token = _jwtService.GenerateToken(user.Username, user.Role);
            return Ok(new { Token = token });
        }

        // POST: api/Users/Register
        [HttpPost("Register")]
        public async Task<ActionResult<User>> Register([FromBody] UserRegisterDto registerDto)
        {
            if (await _context.Users.AnyAsync(u => u.Username == registerDto.Username))
            {
                return BadRequest("Benutzername bereits vergeben.");
            }

            var passwordHash = CreatePasswordHash(registerDto.Password);

            var newUser = new User
            {
                Username = registerDto.Username,
                PasswordHash = passwordHash,
                // Salt = salt, // Salt speichern
                Role = "Mitarbeiter"
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(Register), new { id = newUser.UserId }, newUser);
        }

        // Hilfsfunktionen für die Passwortverwaltung
        private string CreatePasswordHash(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
            /*
            using (var hmac = new HMACSHA256())
            {
                var salt = Convert.ToBase64String(hmac.Key); // Salt generieren
                var passwordHash = Convert.ToBase64String(hmac.ComputeHash(Encoding.UTF8.GetBytes(password)));
                return (passwordHash, salt);
            }
            */
        }

        private bool VerifyPasswordHash(string password, string storedHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, storedHash);
            /*   using (var hmac = new HMACSHA256(Convert.FromBase64String(salt)))
               {
                   var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                   return Convert.ToBase64String(computedHash) == storedHash;
              */
        }
    }
}

// DTOs (Datenobjekte für Login/Registrierung)
public class UserLoginDto
{
    public string Username { get; set; }
    public string Password { get; set; }
}

public class UserRegisterDto
{
    public string Username { get; set; }
    public string Password { get; set; }
}

