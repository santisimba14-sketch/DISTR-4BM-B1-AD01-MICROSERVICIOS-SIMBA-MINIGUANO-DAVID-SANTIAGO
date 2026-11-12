using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Inventario4DB.Models
{
    [Table("ReservasStock")]
    public class ReservaStock
    {
        [Key]
        [Column("IdReserva")]
        public int IdReserva { get; set; }

        [Column("IdProducto")]
        public int IdProducto { get; set; }

        [Column("IdBodega")]
        public int IdBodega { get; set; }

        [Column("Cantidad")]
        public int Cantidad { get; set; }

        [Required]
        [StringLength(50)]
        [Column("Estado")]
        public string Estado { get; set; } = "Pendiente";
    }
}
