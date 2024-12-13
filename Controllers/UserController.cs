using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Praxisarbeit_M295.Data;
using Praxisarbeit_M295.Models;
using Praxisarbeit_M295.Services;
using System.Security.Claims;

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

        // GET: api/Users/Me
        [Authorize]
        [HttpGet("Me")]
        public async Task<ActionResult<User>> GetCurrentUser()
        {
            var username = User.Identity?.Name;
            if (string.IsNullOrEmpty(username))
            {
                return Unauthorized(new { Message = "Benutzer ist nicht authentifiziert." });
            }

            var user = await _context.Users
                .SingleOrDefaultAsync(u => u.Username == username);

            if (user == null)
            {
                return NotFound(new { Message = "Benutzer wurde nicht gefunden." });
            }

            return Ok(new
            {
                user.UserId,
                user.Username,
                user.Role
            });
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

            // Rückgabe des Tokens und der Benutzer-ID
            return Ok(new
            {
                Token = token,
                UserId = user.UserId // Füge die userId zur Antwort hinzu
            });
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
                Role = "Kunde"
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

        // PUT: api/Users/{id}
        [Authorize]
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateUser(int id, [FromBody] UserRegisterDto userDto)
        {
            var user = await _context.Users.FindAsync(id);
            if (user == null)
            {
                return NotFound(new { Message = "Benutzer nicht gefunden." });
            }

            user.Username = userDto.Username;
            user.PasswordHash = CreatePasswordHash(userDto.Password);

            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Benutzer erfolgreich aktualisiert.",
                UserId = user.UserId,
                Username = user.Username,
                Role = user.Role
            });
        }

        // DELETE: api/Users/{id}
        [Authorize]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteUser(int id)
        {
            // Finde den Benutzer anhand der ID
            var user = await _context.Users.FindAsync(id);
            //Console.Write("Geht es?");
            if (user == null)
            {
                return NotFound(new { Message = "Benutzer wurde nicht gefunden." });
            }
            //Console.Write("Hallo Geht es bis Hier?");
            // Lösche den Benutzer aus der Datenbank
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Ok(new { Message = "Benutzer erfolgreich gelöscht." });
        }


        [Authorize]
        [HttpGet("DebugToken")]
        public IActionResult DebugToken()
        {
            var username = User.Identity?.Name;
            var role = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role)?.Value;
            return Ok(new { Username = username, Role = role });
        }


        private string CreatePasswordHash(string password)
        {
            return BCrypt.Net.BCrypt.HashPassword(password);
        }

        private bool VerifyPasswordHash(string password, string storedHash)
        {
            return BCrypt.Net.BCrypt.Verify(password, storedHash);
        }
    }

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
