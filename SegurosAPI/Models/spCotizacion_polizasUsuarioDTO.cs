using System.ComponentModel.DataAnnotations;

namespace SegurosAPI.Models
{
    public class spCotizacion_polizasUsuarioDTO
    {
        [Key]
        public int Poliza_PolizaId { get; set; }
        public string TipoPoliza_Nombre { get; set; }
        public string FechaAdquisicion { get; set; }
        public decimal Costo { get; set; }
        public string Poliza_Estado { get; set; }
    }
}
