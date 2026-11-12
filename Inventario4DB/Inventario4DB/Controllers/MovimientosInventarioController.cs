using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Inventario4DB.Models;

namespace Inventario4DB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MovimientosInventarioController : ControllerBase
    {
        private readonly Data.InventarioDBContext _context;

        public MovimientosInventarioController(Data.InventarioDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<MovimientoInventario>>> GetMovimientos()
        {
            var movimientos = await _context.MovimientosInventario.AsNoTracking().ToListAsync();
            return Ok(movimientos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<MovimientoInventario>> GetMovimiento(int id)
        {
            var movimiento = await _context.MovimientosInventario.AsNoTracking().FirstOrDefaultAsync(m => m.IdMovimiento == id);
            if (movimiento == null)
            {
                return NotFound();
            }
            return Ok(movimiento);
        }

        [HttpPost]
        public async Task<ActionResult<MovimientoInventario>> CrearMovimiento(MovimientoInventario movimiento)
        {
            // Ajustar existencia si existe registro para el producto y la bodega
            var existencia = await _context.Existencias
                .FirstOrDefaultAsync(e => e.IdProducto == movimiento.IdProducto && e.IdBodega == movimiento.IdBodega);

            if (existencia != null)
            {
                if (movimiento.Tipo.Equals("Entrada", StringComparison.OrdinalIgnoreCase))
                {
                    existencia.Stock += movimiento.Cantidad;
                }
                else if (movimiento.Tipo.Equals("Salida", StringComparison.OrdinalIgnoreCase))
                {
                    existencia.Stock -= movimiento.Cantidad;
                }
            }

            _context.MovimientosInventario.Add(movimiento);
            await _context.SaveChangesAsync();
            return Ok(movimiento);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<MovimientoInventario>> ActualizarMovimiento(int id, MovimientoInventario movimiento)
        {
            if (id != movimiento.IdMovimiento)
            {
                return BadRequest();
            }
            _context.Entry(movimiento).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarMovimiento(int id)
        {
            var movimiento = await _context.MovimientosInventario.FindAsync(id);
            if (movimiento == null)
            {
                return NotFound();
            }
            _context.MovimientosInventario.Remove(movimiento);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
