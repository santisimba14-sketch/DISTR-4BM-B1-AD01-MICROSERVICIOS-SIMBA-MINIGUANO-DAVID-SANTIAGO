using Microsoft.EntityFrameworkCore;
using Inventario4DB.Models;

namespace Inventario4DB.Data
{
    public class InventarioDBContext : DbContext
    {
        public InventarioDBContext(DbContextOptions<InventarioDBContext> options) : base(options)
        {
        }

        public DbSet<Bodega> Bodegas { get; set; }
        public DbSet<Existencia> Existencias { get; set; }
        public DbSet<MovimientoInventario> MovimientosInventario { get; set; }
        public DbSet<ReservaStock> ReservasStock { get; set; }
    }
}
