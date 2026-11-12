using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Pedidos.Models
{
    [Table("Clientes")]
    public class Cliente
    {
        [Key]
        [Column("IdCliente")]
        public int IdCliente { get; set; }

        [Required]
        [StringLength(150)]
        [Column("Nombres")]
        public string Nombres { get; set; } = string.Empty;

        [Required]
        [StringLength(20)]
        [Column("Cedula")]
        public string Cedula { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        [Column("Correo")]
        public string Correo { get; set; } = string.Empty;
    }
}
