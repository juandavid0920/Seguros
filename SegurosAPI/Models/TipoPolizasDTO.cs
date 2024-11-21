using System.ComponentModel.DataAnnotations;

namespace SegurosAPI.Models
{
    public class TipoPolizasDTO
    {
        [Key] //PK
        public int TipoPolizaId { get; set; }
        public string Descripcion { get; set; }
        public decimal Costo { get; set; }
        public bool Estado { get; set; }
    }
}
