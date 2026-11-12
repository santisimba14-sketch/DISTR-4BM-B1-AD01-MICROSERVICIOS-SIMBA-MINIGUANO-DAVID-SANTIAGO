using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Pedidos.Models;

namespace Pedidos.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PedidosController : ControllerBase
    {
        private readonly Data.PedidosDBContext _context;
        private readonly IHttpClientFactory _httpClientFactory;

        public PedidosController(Data.PedidosDBContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Pedido>>> GetPedidos()
        {
            var pedidos = await _context.Pedidos.AsNoTracking().ToListAsync();
            return Ok(pedidos);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Pedido>> GetPedido(int id)
        {
            var pedido = await _context.Pedidos.AsNoTracking().FirstOrDefaultAsync(p => p.IdPedido == id);
            if (pedido == null)
            {
                return NotFound();
            }
            return Ok(pedido);
        }

        [HttpGet("{id}/con-detalles")]
        public async Task<IActionResult> GetPedidoConDetalles(int id)
        {
            var pedido = await _context.Pedidos.AsNoTracking().FirstOrDefaultAsync(p => p.IdPedido == id);
            if (pedido == null)
            {
                return NotFound(new { Message = "Pedido no encontrado" });
            }

            var detalles = await _context.DetallesPedido
                .AsNoTracking()
                .Where(d => d.IdPedido == id)
                .ToListAsync();

            return Ok(new { pedido, detalles });
        }

        [HttpGet("validar-producto/{idProducto:int}")]
        public async Task<IActionResult> ValidarProducto(int idProducto)
        {
            var clienteCatalogo = _httpClientFactory.CreateClient("CatalogoApi");
            var clienteInventario = _httpClientFactory.CreateClient("InventarioApi");

            try
            {
                // 1. Consultar existencia y estado del producto en Catalogo.Api
                using var respuestaCat = await clienteCatalogo.GetAsync($"api/productos/{idProducto}");
                if (respuestaCat.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return NotFound(new { Message = "Producto no encontrado en CatalogoApi" });
                }

                if (!respuestaCat.IsSuccessStatusCode)
                {
                    return StatusCode(StatusCodes.Status502BadGateway,
                        new { Message = "CatalogoApi respondió con un error http." });
                }

                var producto = await respuestaCat.Content.ReadFromJsonAsync<JsonElement>();

                // 2. Consultar existencia de stock en Inventario.Api
                using var respuestaInv = await clienteInventario.GetAsync($"api/existencias/por-producto/{idProducto}");
                JsonElement? existencia = null;

                if (respuestaInv.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return Ok(new
                    {
                        producto,
                        existencia = (object?)null,
                        mensaje = "El producto existe en catálogo pero no tiene registro de existencia en inventario"
                    });
                }
                else if (respuestaInv.IsSuccessStatusCode)
                {
                    existencia = await respuestaInv.Content.ReadFromJsonAsync<JsonElement>();
                }
                else
                {
                    return StatusCode(StatusCodes.Status502BadGateway,
                        new { Message = "InventarioApi respondió con un error http." });
                }

                return Ok(new { producto, existencia });
            }
            catch (TaskCanceledException)
            {
                return StatusCode(StatusCodes.Status504GatewayTimeout,
                    new { Message = "Se agotó el tiempo de espera del servicio remoto" });
            }
            catch (HttpRequestException)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable,
                    new { Message = "Servicio remoto no disponible" });
            }
            catch (JsonException)
            {
                return StatusCode(StatusCodes.Status502BadGateway,
                    new { Message = "El servicio remoto devolvió un JSON inválido" });
            }
        }

        [HttpPost]
        public async Task<ActionResult<Pedido>> CrearPedido(Pedido pedido)
        {
            _context.Pedidos.Add(pedido);
            await _context.SaveChangesAsync();
            return Ok(pedido);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Pedido>> ActualizarPedido(int id, Pedido pedido)
        {
            if (id != pedido.IdPedido)
            {
                return BadRequest();
            }
            _context.Entry(pedido).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarPedido(int id)
        {
            var pedido = await _context.Pedidos.FindAsync(id);
            if (pedido == null)
            {
                return NotFound();
            }
            _context.Pedidos.Remove(pedido);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
