using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventario4DB.Models
{
    [Table("MovimientosInventario")]
    public class MovimientoInventario
    {
        [Key]
        [Column("IdMovimiento")]
        public int IdMovimiento { get; set; }

        [Column("IdProducto")]
        public int IdProducto { get; set; }

        [Column("IdBodega")]
        public int IdBodega { get; set; }

        [Required]
        [StringLength(50)]
        [Column("Tipo")]
        public string Tipo { get; set; } = string.Empty;

        [Column("Cantidad")]
        public int Cantidad { get; set; }

        [Column("Fecha")]
        public DateTime Fecha { get; set; } = DateTime.Now;
    }
}
