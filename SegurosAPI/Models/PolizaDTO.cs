using System.ComponentModel.DataAnnotations;

namespace SegurosAPI.Models
{
    public class PolizaDTO
    {
        [Key] //Llave primaria
        public int PolizaId { get; set; }
        [Required] //FK
        public int UsuarioId { get; set; }
        [Required] //FK
        public int TipoPolizaId { get; set; }
        public DateTime FechaAdquisicion { get; set; }
        public decimal CostoPago { get; set; }
        public string Estado { get; set; }


    }
}
