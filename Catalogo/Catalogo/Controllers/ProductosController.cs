using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Catalogo.Models;

namespace Catalogo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductosController : ControllerBase
    {
        private readonly Data.CatalogoDBContext _context;
        private readonly IHttpClientFactory _httpClientFactory;

        public ProductosController(Data.CatalogoDBContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Producto>>> GetProductos()
        {
            var productos = await _context.Productos.AsNoTracking().ToListAsync();
            return Ok(productos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Producto>> GetProducto(int id)
        {
            var producto = await _context.Productos.AsNoTracking().FirstOrDefaultAsync(p => p.IdProducto == id);
            if (producto == null)
            {
                return NotFound();
            }
            return Ok(producto);
        }

        [HttpGet("{id}/con-inventario")]
        public async Task<IActionResult> GetProductoConInventario(int id)
        {
            var producto = await _context.Productos
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.IdProducto == id);

            if (producto == null)
            {
                return NotFound(new { Message = "Producto no encontrado" });
            }

            var cliente = _httpClientFactory.CreateClient("InventarioApi");
            try
            {
                using var respuesta = await cliente.GetAsync($"api/existencias/por-producto/{id}");
                if (respuesta.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return Ok(new
                    {
                        producto,
                        inventario = (object?)null,
                        mensaje = "El producto existe, pero no tiene inventario registrado"
                    });
                }

                if (!respuesta.IsSuccessStatusCode)
                {
                    return StatusCode(StatusCodes.Status502BadGateway,
                        new { mensaje = "Inventario.Api respondió con un error http." });
                }

                var inventario = await respuesta.Content.ReadFromJsonAsync<JsonElement>();

                return Ok(new { producto, inventario });
            }
            catch (TaskCanceledException)
            {
                return StatusCode(StatusCodes.Status504GatewayTimeout, new { Message = "Se agotó el tiempo de espera de Inventario.Api" });
            }
            catch (HttpRequestException)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, new { Message = "Inventario.Api no disponible" });
            }
            catch (JsonException)
            {
                return StatusCode(StatusCodes.Status502BadGateway, new { Message = "Inventario.Api devolvió un JSON inválido" });
            }
        }

        [HttpPost]
        public async Task<ActionResult<Producto>> CrearProducto(Producto producto)
        {
            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();
            return Ok(producto);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Producto>> ActualizarProducto(int id, Producto producto)
        {
            if (id != producto.IdProducto)
            {
                return BadRequest();
            }
            _context.Entry(producto).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarProducto(int id)
        {
            var producto = await _context.Productos.FindAsync(id);
            if (producto == null)
            {
                return NotFound();
            }
            _context.Productos.Remove(producto);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
