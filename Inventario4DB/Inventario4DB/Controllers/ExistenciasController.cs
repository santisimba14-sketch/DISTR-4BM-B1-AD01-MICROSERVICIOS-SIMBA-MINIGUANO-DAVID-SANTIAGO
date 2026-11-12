using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Inventario4DB.Models;

namespace Inventario4DB.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExistenciasController : ControllerBase
    {
        private readonly Data.InventarioDBContext _context;

        public ExistenciasController(Data.InventarioDBContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Existencia>>> GetExistencias()
        {
            var existencias = await _context.Existencias.AsNoTracking().ToListAsync();
            return Ok(existencias);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Existencia>> GetExistencia(int id)
        {
            var existencia = await _context.Existencias.AsNoTracking().FirstOrDefaultAsync(e => e.IdExistencia == id);
            if (existencia == null)
            {
                return NotFound();
            }
            return Ok(existencia);
        }

        [HttpGet("por-producto/{idProducto:int}")]
        public async Task<ActionResult<Existencia>> GetExistenciaPorProducto(int idProducto)
        {
            var existencia = await _context.Existencias.AsNoTracking().FirstOrDefaultAsync(e => e.IdProducto == idProducto);
            if (existencia == null)
            {
                return NotFound();
            }
            return Ok(existencia);
        }

        [HttpGet("por-producto/{idProducto:int}/todas")]
        public async Task<ActionResult<IEnumerable<Existencia>>> GetExistenciasPorProducto(int idProducto)
        {
            var existencias = await _context.Existencias.AsNoTracking().Where(e => e.IdProducto == idProducto).ToListAsync();
            return Ok(existencias);
        }

        [HttpPost]
        public async Task<ActionResult<Existencia>> CrearExistencia(Existencia existencia)
        {
            _context.Existencias.Add(existencia);
            await _context.SaveChangesAsync();
            return Ok(existencia);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Existencia>> ActualizarExistencia(int id, Existencia existencia)
        {
            if (id != existencia.IdExistencia)
            {
                return BadRequest();
            }
            _context.Entry(existencia).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarExistencia(int id)
        {
            var existencia = await _context.Existencias.FindAsync(id);
            if (existencia == null)
            {
                return NotFound();
            }
            _context.Existencias.Remove(existencia);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
