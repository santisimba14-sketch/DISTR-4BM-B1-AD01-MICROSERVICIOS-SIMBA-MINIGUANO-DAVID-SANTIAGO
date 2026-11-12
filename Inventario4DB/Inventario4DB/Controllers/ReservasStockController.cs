using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Inventario4DB.Models;

namespace Inventario4DB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservasStockController : ControllerBase
    {
        private readonly Data.InventarioDBContext _context;

        public ReservasStockController(Data.InventarioDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ReservaStock>>> GetReservas()
        {
            var reservas = await _context.ReservasStock.AsNoTracking().ToListAsync();
            return Ok(reservas);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ReservaStock>> GetReserva(int id)
        {
            var reserva = await _context.ReservasStock.AsNoTracking().FirstOrDefaultAsync(r => r.IdReserva == id);
            if (reserva == null)
            {
                return NotFound();
            }
            return Ok(reserva);
        }

        [HttpPost]
        public async Task<ActionResult<ReservaStock>> CrearReserva(ReservaStock reserva)
        {
            _context.ReservasStock.Add(reserva);
            await _context.SaveChangesAsync();
            return Ok(reserva);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ReservaStock>> ActualizarReserva(int id, ReservaStock reserva)
        {
            if (id != reserva.IdReserva)
            {
                return BadRequest();
            }
            _context.Entry(reserva).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarReserva(int id)
        {
            var reserva = await _context.ReservasStock.FindAsync(id);
            if (reserva == null)
            {
                return NotFound();
            }
            _context.ReservasStock.Remove(reserva);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
