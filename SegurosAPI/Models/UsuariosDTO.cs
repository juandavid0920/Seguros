using System.ComponentModel.DataAnnotations;

namespace SegurosAPI.Models
{
    public class UsuariosDTO
    {
        [Key] //PK
        public int UsuarioId { get; set; }
        public string Nombre { get; set; }
        public string CorreoElectronico { get; set; }
        public string Rol { get; set; }
        public string Contraseña { get; set; }
        public string Estado { get; set; }

        // Constructor para valores predeterminados
        public UsuariosDTO()
        {
            UsuarioId = 0;      
            Nombre = string.Empty;
            CorreoElectronico = string.Empty;
            Rol = string.Empty;
            Contraseña = string.Empty;
            Estado = "Inactivo";

        }
    }
}
