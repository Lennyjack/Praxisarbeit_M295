using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Praxisarbeit_M295.Data;
using Praxisarbeit_M295.Models;

namespace Praxisarbeit_M295.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceOrdersController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ServiceOrdersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/ServiceOrders
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ServiceOrder>>> GetServiceOrders()
        {
            // Nur nicht gelöschte Aufträge zurückgeben
            return await _context.ServiceOrders
                .Where(order => !order.IsDeleted)
                .ToListAsync();
        }

        // GET: api/ServiceOrders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceOrder>> GetServiceOrder(int id)
        {
            var serviceOrder = await _context.ServiceOrders.FindAsync(id);

            if (serviceOrder == null || serviceOrder.IsDeleted)
            {
                return NotFound(new { Message = "Serviceauftrag nicht gefunden." });
            }

            return serviceOrder;
        }

        // GET: api/ServiceOrders/User/{userId}
        [Authorize] // Autorisierung erforderlich
        [HttpGet("User/{userId}")]
        public async Task<ActionResult<IEnumerable<ServiceOrder>>> GetOrdersByUser(int userId)
        {
            // Prüfen, ob der Benutzer existiert
            var userExists = await _context.Users.AnyAsync(u => u.UserId == userId);
            if (!userExists)
            {
                return NotFound(new { Message = "Benutzer nicht gefunden." });
            }

            // Aufträge des Benutzers abrufen
            var userOrders = await _context.ServiceOrders
                .Where(order => order.AssignedTo == userId && !order.IsDeleted)
                .ToListAsync();

            if (!userOrders.Any())
            {
                return NotFound(new { Message = "Keine Aufträge für diesen Benutzer gefunden." });
            }

            return Ok(userOrders);
        }

        // POST: api/ServiceOrders
        [HttpPost]
        public async Task<ActionResult<ServiceOrder>> PostServiceOrder(ServiceOrder serviceOrder)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.ServiceOrders.Add(serviceOrder);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetServiceOrder), new { id = serviceOrder.OrderId }, serviceOrder);
        }

        // PUT: api/ServiceOrders/5
        [Authorize] // Autorisierung erforderlich
        [HttpPut("{id}")]
        public async Task<IActionResult> PutServiceOrder(int id, ServiceOrder serviceOrder)
        {
            if (id != serviceOrder.OrderId)
            {
                return BadRequest("ID im URL stimmt nicht mit der ID des Objekts überein.");
            }

            // Sicherstellen, dass der Auftrag existiert und nicht gelöscht ist
            var existingOrder = await _context.ServiceOrders.FindAsync(id);
            if (existingOrder == null || existingOrder.IsDeleted)
            {
                return NotFound(new { Message = "Serviceauftrag nicht gefunden." });
            }

            // Auftrag aktualisieren
            existingOrder.Priority = serviceOrder.Priority;
            existingOrder.Status = serviceOrder.Status;
            existingOrder.Service = serviceOrder.Service;
            existingOrder.AssignedTo = serviceOrder.AssignedTo;
            existingOrder.Name = serviceOrder.Name;
            existingOrder.Email = serviceOrder.Email;
            existingOrder.Phone = serviceOrder.Phone;

            await _context.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: api/ServiceOrders/5
        [Authorize] // Autorisierung erforderlich
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteServiceOrder(int id)
        {
            var serviceOrder = await _context.ServiceOrders.FindAsync(id);
            if (serviceOrder == null)
            {
                return NotFound(new { Message = "Serviceauftrag nicht gefunden." });
            }

            // Auftrag als gelöscht markieren
            serviceOrder.IsDeleted = true;

            // Änderungen in der Datenbank speichern
            await _context.SaveChangesAsync();

            return NoContent();
        }

        [Authorize]
        [HttpPut("{id}/Assign")]
        public async Task<IActionResult> AssignServiceOrder(int id, [FromBody] int userId)
        {
            // Finde den Serviceauftrag
            var serviceOrder = await _context.ServiceOrders.FindAsync(id);
            if (serviceOrder == null)
            {
                return NotFound(new { Message = "Serviceauftrag nicht gefunden." });
            }

            // Finde den Benutzer
            var user = await _context.Users.FindAsync(userId);
            if (user == null)
            {
                return NotFound(new { Message = "Benutzer nicht gefunden." });
            }

            // Aktualisiere die Zuweisung
            serviceOrder.AssignedTo = user.UserId;
            serviceOrder.AssignedUser = user;

            // Speichere die Änderungen
            await _context.SaveChangesAsync();

            return Ok(new
            {
                Message = "Serviceauftrag erfolgreich zugewiesen.",
                ServiceOrderId = id,
                AssignedTo = userId
            });
        }


        // Helper-Methode, um zu prüfen, ob ein ServiceOrder existiert
        private bool ServiceOrderExists(int id)
        {
            return _context.ServiceOrders.Any(e => e.OrderId == id && !e.IsDeleted);
        }
    }
}
