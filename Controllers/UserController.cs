using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Praxisarbeit_M295.Data;
using Praxisarbeit_M295.Models;
using Praxisarbeit_M295.Services;

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

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetUsers()
        {
            var users = await _context.Users
                .Select(user => new
                {
                    user.UserId,
                    user.Username,
                    user.Role
                })
                .ToListAsync();

            return Ok(users);
        }

        // POST: api/Users/Login
        [HttpPost("Login")]
        public async Task<ActionResult<object>> Login([FromBody] UserLoginDto loginDto)
        {
            var user = await _context.Users
                .SingleOrDefaultAsync(u => u.Username == loginDto.Username);

            if (user == null || !VerifyPasswordHash(loginDto.Password, user.PasswordHash))
            {
                return Unauthorized(new { Message = "Falscher Benutzername oder Passwort." });
            }

            var token = _jwtService.GenerateToken(user.Username, user.Role);
            return Ok(new { Token = token });
        }

        // POST: api/Users/Register
        [HttpPost("Register")]
        public async Task<ActionResult<object>> Register([FromBody] UserRegisterDto registerDto)
        {
            if (await _context.Users.AnyAsync(u => u.Username == registerDto.Username))
            {
                return BadRequest(new { Message = "Benutzername bereits vergeben." });
            }

            var passwordHash = CreatePasswordHash(registerDto.Password);

            var newUser = new User
            {
                Username = registerDto.Username,
                PasswordHash = passwordHash,
                Role = "Kunde" // Standardrolle
            };

            _context.Users.Add(newUser);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetUsers), new { id = newUser.UserId }, new
            {
                newUser.UserId,
                newUser.Username,
                newUser.Role
            });
        }

        // Hilfsfunktion: Erzeugt einen sicheren Passwort-Hash
        private string CreatePasswordHash(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        // Hilfsfunktion: Überprüft ein Passwort gegen einen gespeicherten Hash
        private bool VerifyPasswordHash(string password, string storedHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, storedHash);
        }
    }

    // DTOs (Datenobjekte für Login und Registrierung)
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
}
