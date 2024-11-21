using Newtonsoft.Json;
using SegurosAPI.Models;
using Microsoft.AspNetCore.Http;


namespace SegurosMVC.Utilidades
{
    public static class UsuarioSessionHelper
    {
        // Almacenar la información del usuario en la sesión
        public static void SetUsuario(HttpContext httpContext, UsuariosDTO usuario)
        {
            // Serializamos el objeto de usuario a JSON
            var usuarioJson = JsonConvert.SerializeObject(usuario);
            // Guardamos el objeto JSON en la sesión
            httpContext.Session.SetString("usuario", usuarioJson);
        }

        // Obtener la información del usuario desde la sesión
        public static UsuariosDTO GetUsuario(HttpContext httpContext)
        {
            // Obtenemos el objeto JSON de la sesión
            var usuarioJson = httpContext.Session.GetString("usuario");

            // Si no hay datos en la sesión, retornamos null
            if (string.IsNullOrEmpty(usuarioJson))
            {
                return null;
            }

            // Deserializamos el JSON y retornamos el objeto UsuarioDTO
            return JsonConvert.DeserializeObject<UsuariosDTO>(usuarioJson);
        }

        // Verificar si el usuario está autenticado (si la sesión contiene datos)
        public static bool IsUsuarioAutenticado(HttpContext httpContext)
        {
            var usuario = GetUsuario(httpContext);
            return usuario != null; // Si el usuario existe, está autenticado
        }

        // Eliminar la información del usuario de la sesión (logout)
        public static void Logout(HttpContext httpContext)
        {
            httpContext.Session.Remove("usuario");
        }
    }
}
