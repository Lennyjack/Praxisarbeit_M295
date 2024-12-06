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
            return await _context.ServiceOrders.ToListAsync();
        }

        // GET: api/ServiceOrders/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceOrder>> GetServiceOrder(int id)
        {
            var serviceOrder = await _context.ServiceOrders.FindAsync(id);

            if (serviceOrder == null)
            {
                return NotFound();
            }

            return serviceOrder;
        }

        // POST: api/ServiceOrders
        [HttpPost]
        public async Task<ActionResult<ServiceOrder>> PostServiceOrder(ServiceOrder serviceOrder)
        {
            // Validierung hinzufügen (optional)
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

            _context.Entry(serviceOrder).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ServiceOrderExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

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
                return NotFound();
            }

            _context.ServiceOrders.Remove(serviceOrder);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        // Helper-Methode, um zu prüfen, ob ein ServiceOrder existiert
        private bool ServiceOrderExists(int id)
        {
            return _context.ServiceOrders.Any(e => e.OrderId == id);
        }
    }
}
