using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pedidos.Models
{
    [Table("Pedidos")]
    public class Pedido
    {
        [Key]
        [Column("IdPedido")]
        public int IdPedido { get; set; }

        [Column("IdCliente")]
        public int IdCliente { get; set; }

        [Column("Fecha")]
        public DateTime Fecha { get; set; } = DateTime.Now;

        [Column("Total", TypeName = "decimal(10,2)")]
        public decimal Total { get; set; }

        [Required]
        [StringLength(50)]
        [Column("Estado")]
        public string Estado { get; set; } = string.Empty;
    }
}
