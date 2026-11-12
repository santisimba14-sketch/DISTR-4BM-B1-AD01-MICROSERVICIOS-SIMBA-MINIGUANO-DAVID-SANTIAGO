using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Pedidos.Models;

namespace Pedidos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DetallesPedidoController : ControllerBase
    {
        private readonly Data.PedidosDBContext _context;

        public DetallesPedidoController(Data.PedidosDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DetallePedido>>> GetDetalles()
        {
            var detalles = await _context.DetallesPedido.AsNoTracking().ToListAsync();
            return Ok(detalles);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DetallePedido>> GetDetalle(int id)
        {
            var detalle = await _context.DetallesPedido.AsNoTracking().FirstOrDefaultAsync(d => d.IdDetalle == id);
            if (detalle == null)
            {
                return NotFound();
            }
            return Ok(detalle);
        }

        [HttpGet("por-pedido/{idPedido:int}")]
        public async Task<ActionResult<IEnumerable<DetallePedido>>> GetDetallesPorPedido(int idPedido)
        {
            var detalles = await _context.DetallesPedido.AsNoTracking().Where(d => d.IdPedido == idPedido).ToListAsync();
            return Ok(detalles);
        }

        [HttpPost]
        public async Task<ActionResult<DetallePedido>> CrearDetalle(DetallePedido detalle)
        {
            _context.DetallesPedido.Add(detalle);
            await _context.SaveChangesAsync();
            return Ok(detalle);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<DetallePedido>> ActualizarDetalle(int id, DetallePedido detalle)
        {
            if (id != detalle.IdDetalle)
            {
                return BadRequest();
            }
            _context.Entry(detalle).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarDetalle(int id)
        {
            var detalle = await _context.DetallesPedido.FindAsync(id);
            if (detalle == null)
            {
                return NotFound();
            }
            _context.DetallesPedido.Remove(detalle);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
