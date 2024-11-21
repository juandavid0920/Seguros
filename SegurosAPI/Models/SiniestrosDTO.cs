using System.ComponentModel.DataAnnotations;

namespace SegurosAPI.Models
{
    public class SiniestrosDTO
    {
        [Key]
        public int SiniestroId { get; set; } //PK
        public int PolizaId { get; set; }
        public DateTime FechaSiniestro { get; set; }
        public string Descripcion { get; set; }
        public string Estado { get; set; }
        public decimal MontoReclamo { get; set; }

    }
}
