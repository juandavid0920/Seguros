using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using SegurosAPI.Models;
using SegurosMVC.Models;
using System.Text;
using SegurosMVC.Utilidades;

namespace SegurosMVC.Controllers
{
    public class GestionesController : Controller
    {
        private static List<TipoPolizasDTO> _tipoPolizasTemporales = new List<TipoPolizasDTO>();

        private readonly HttpClient _httpClient;

        public GestionesController()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:5084/api/") // URL base de la API
            };
        }

        #region Vistas

        // Acción para la vista de gestión de pólizas
        public async Task<IActionResult> GestionPolizaAsync()
        {
            var usuario = UsuarioSessionHelper.GetUsuario(HttpContext);

            if (usuario == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (usuario.Rol == "Cliente")
            {
                var response = await _httpClient.GetAsync("TipoPolizas");
                var jsonString = await response.Content.ReadAsStringAsync();
                var TipoPolizas = JsonConvert.DeserializeObject<List<TipoPolizasDTO>>(jsonString);

                var response2 = await _httpClient.GetAsync($"Polizas/Cotizacion_polizasUsuario?usuarioId={usuario.UsuarioId}");
                var jsonString2 = await response2.Content.ReadAsStringAsync();
                var CotizacionPolizasUsuario = JsonConvert.DeserializeObject<List<spCotizacion_polizasUsuarioDTO>>(jsonString2);

                // Guardar temporalmente los datos de las tipos de la poliza para uso más adelante
                _tipoPolizasTemporales = TipoPolizas;

                ViewBag.TipoPolizas = TipoPolizas;
                ViewBag.CotizacionPolizasUsuario = CotizacionPolizasUsuario;
                ViewBag.usuario = usuario;
                return View();
            }
            else
            {
                return View("AccesoDenegado");
            }
        }

        // Acción para la vista de gestión de siniestros
        public async Task<IActionResult> GestionSiniestrosAsync(int usuarioId_gestiona = 0)
        {
            var usuario = UsuarioSessionHelper.GetUsuario(HttpContext);

            if (usuario == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (usuario.Rol == "Administrador")
            {
                // Obtener la lista de usuarios
                var response = await _httpClient.GetAsync("Usuarios/GetUsuarios_Obtener");
                var jsonString = await response.Content.ReadAsStringAsync();
                var ListaUsuarios = JsonConvert.DeserializeObject<List<UsuariosDTO>>(jsonString);

                // Obtener pólizas solo si se selecciona un usuario
                List<spCotizacion_polizasUsuarioDTO> CotizacionPolizasUsuario = new List<spCotizacion_polizasUsuarioDTO>();
                if (usuarioId_gestiona > 0)
                {
                    var response2 = await _httpClient.GetAsync($"Polizas/Cotizacion_polizasUsuario?usuarioId={usuarioId_gestiona}");
                    var jsonString2 = await response2.Content.ReadAsStringAsync();
                    CotizacionPolizasUsuario = JsonConvert.DeserializeObject<List<spCotizacion_polizasUsuarioDTO>>(jsonString2);
                }

                // Enviar datos a la vista
                ViewBag.ListaUsuarios = ListaUsuarios;
                ViewBag.PolizasUsuario = CotizacionPolizasUsuario;
                ViewBag.UsuarioIdSeleccionado = usuarioId_gestiona;

                return View();
            }
            else
            {
                return View("AccesoDenegado");
            }
        }

        // Acción para la vista de gestión de pagos
        public IActionResult GestionPagos()
        {
            var usuario = UsuarioSessionHelper.GetUsuario(HttpContext);

            if (usuario == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (usuario.Rol == "Cliente")
            {
                return View(); // Mostrar vista de gestión de pagos para el Cliente
            }
            else
            {
                return View("AccesoDenegado");
            }
        }

        // Acción para la vista de acceso denegado
        public IActionResult AccesoDenegado()
        {
            return View();
        }

        #endregion

        #region Métodos Pólizas

        [HttpGet]
        public IActionResult ObtenerCostoPorTipoPoliza(int tipoPolizaId)
        {
            // Buscar el costo en la lista temporal
            var tipoPoliza = _tipoPolizasTemporales.FirstOrDefault(tp => tp.TipoPolizaId == tipoPolizaId);

            if (tipoPoliza != null)
            {
                return Json(new { success = true, costo = tipoPoliza.Costo });
            }
            else
            {
                return Json(new { success = false, message = "Tipo de póliza no encontrado." });
            }
        }

        // Método para crear una nueva póliza
        [HttpPost]
        public async Task<IActionResult> Polizas_CrearPoliza([FromBody] PolizaDTO poliza)
        {
            if (poliza == null)
            {
                return Json(new { success = false, message = "No se recibieron datos válidos en la solicitud." });
            }

            // Obtener el usuario de la sesión
            var usuario = UsuarioSessionHelper.GetUsuario(HttpContext);

            if (usuario == null)
            {
                return Json(new { success = false, message = "Usuario no autenticado. Por favor, inicie sesión." });
            }

            // Asignar el UsuarioId y el estado por defecto
            poliza.UsuarioId = usuario.UsuarioId;
            poliza.Estado = "Pendiente Pago";

            try
            {
                // Log para depurar la información que recibimos
                Console.WriteLine($"Recibido póliza: {JsonConvert.SerializeObject(poliza)}");

                // Realizamos la solicitud POST hacia la API externa para crear la póliza
                var jsonContent = new StringContent(
                    JsonConvert.SerializeObject(poliza),
                    Encoding.UTF8,
                    "application/json"
                );

                // Asegúrate de que la URL corresponda a la API que maneja la creación de pólizas
                var response = await _httpClient.PostAsync("Polizas/Polizas_CrearPoliza", jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    // Si la respuesta es exitosa, leer el contenido y retornar éxito
                    var resultadoJson = await response.Content.ReadAsStringAsync();
                    return Json(new { success = true, message = "Cotización generada con éxito.", data = resultadoJson });
                }
                else
                {
                    return Json(new { success = false, message = "Error al crear la cotización, intente nuevamente." });
                }
            }
            catch (Exception ex)
            {
                // En caso de error, retornamos el mensaje de error
                return Json(new { success = false, message = $"Ocurrió un error: {ex.Message}" });
            }
        }

        //Metodo para pagar una poliza
        [HttpPost]
        public async Task<IActionResult> Polizas_PolizaPagar([FromBody] PolizaDTO poliza)
        {
            if (poliza == null)
            {
                return Json(new { success = false, message = "No se recibieron datos válidos en la solicitud." });
            }

            // Obtener el usuario de la sesión
            var usuario = UsuarioSessionHelper.GetUsuario(HttpContext);

            if (usuario == null)
            {
                return Json(new { success = false, message = "Usuario no autenticado. Por favor, inicie sesión." });
            }

            try
            {
                // Log para depurar la información que recibimos
                Console.WriteLine($"Recibido póliza: {JsonConvert.SerializeObject(poliza)}");

                // Realizamos la solicitud POST hacia la API externa para crear la póliza
                var jsonContent = new StringContent(
                    JsonConvert.SerializeObject(poliza),
                    Encoding.UTF8,
                    "application/json"
                );

                // Asegúrate de que la URL corresponda a la API que maneja la creación de pólizas
                var response = await _httpClient.PostAsync("Polizas/Polizas_PolizaPagar", jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    var resultadoJson = await response.Content.ReadAsStringAsync();
                    return Json(new { success = true, message = "Poliza Pagada con Exito.", data = resultadoJson });
                }
                else
                {
                    return Json(new { success = false, message = "No se pude registrar el Pago." });
                }
            }
            catch (Exception ex)
            {
                // En caso de error, retornamos el mensaje de error
                return Json(new { success = false, message = $"Ocurrió un error: {ex.Message}" });
            }
        }

        //Metodo para pagar una poliza
        [HttpPost]
        public async Task<IActionResult> Polizas_PolizaNoAdquirir([FromBody] PolizaDTO poliza)
        {
            if (poliza == null)
            {
                return Json(new { success = false, message = "No se recibieron datos válidos en la solicitud." });
            }

            // Obtener el usuario de la sesión
            var usuario = UsuarioSessionHelper.GetUsuario(HttpContext);

            if (usuario == null)
            {
                return Json(new { success = false, message = "Usuario no autenticado. Por favor, inicie sesión." });
            }

            try
            {
                // Log para depurar la información que recibimos
                Console.WriteLine($"Recibido póliza: {JsonConvert.SerializeObject(poliza)}");

                // Realizamos la solicitud POST hacia la API externa para crear la póliza
                var jsonContent = new StringContent(
                    JsonConvert.SerializeObject(poliza),
                    Encoding.UTF8,
                    "application/json"
                );

                // Asegúrate de que la URL corresponda a la API que maneja la creación de pólizas
                var response = await _httpClient.PostAsync("Polizas/Polizas_PolizaNoAdquirir", jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    var resultadoJson = await response.Content.ReadAsStringAsync();
                    return Json(new { success = true, message = "Poliza no adquirida.", data = resultadoJson });
                }
                else
                {
                    return Json(new { success = false, message = "No en el servicio,intente mas tarde." });
                }
            }
            catch (Exception ex)
            {
                // En caso de error, retornamos el mensaje de error
                return Json(new { success = false, message = $"Ocurrió un error: {ex.Message}" });
            }
        }

        //Metodo para pagar una poliza
        [HttpPost]
        public async Task<IActionResult> Siniestro_AgregarNuevoPoliza([FromBody] SiniestrosDTO siniestro)
        {
            if (siniestro == null)
            {
                return Json(new { success = false, message = "No se recibieron datos válidos en la solicitud." });
            }

            // Obtener el usuario de la sesión
            var usuario = UsuarioSessionHelper.GetUsuario(HttpContext);

            if (usuario == null)
            {
                return Json(new { success = false, message = "Usuario no autenticado. Por favor, inicie sesión." });
            }

            try
            {
                // Log para depurar la información que recibimos
                Console.WriteLine($"Recibido póliza: {JsonConvert.SerializeObject(siniestro)}");

                // Realizamos la solicitud POST hacia la API externa para crear la póliza
                var jsonContent = new StringContent(
                    JsonConvert.SerializeObject(siniestro),
                    Encoding.UTF8,
                    "application/json"
                );

                // Asegúrate de que la URL corresponda a la API que maneja la creación de pólizas
                var response = await _httpClient.PostAsync("Polizas/Siniestro_AgregarNuevoPoliza", jsonContent);

                if (response.IsSuccessStatusCode)
                {
                    var resultadoJson = await response.Content.ReadAsStringAsync();
                    return Json(new { success = true, message = "Siniestro registrado.", data = resultadoJson });
                }
                else
                {
                    return Json(new { success = false, message = "No en el servicio,intente mas tarde." });
                }
            }
            catch (Exception ex)
            {
                // En caso de error, retornamos el mensaje de error
                return Json(new { success = false, message = $"Ocurrió un error: {ex.Message}" });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Siniestro_ConsultaPoliza(int polizaId)
        {
            if (polizaId <= 0)
            {
                return Json(new { success = false, message = "ID de póliza no válido." });
            }

            try
            {
                // Realizamos la solicitud GET hacia la API externa para obtener los siniestros de la póliza
                var response = await _httpClient.GetAsync($"Polizas/Siniestro_ConsultaPoliza?PolizaId={polizaId}");

                if (response.IsSuccessStatusCode)
                {
                    // Parseamos la respuesta en JSON
                    var siniestrosJson = await response.Content.ReadAsStringAsync();

                    // Convertimos el JSON en una lista de objetos DTO
                    var siniestros = JsonConvert.DeserializeObject<List<SiniestrosDTO>>(siniestrosJson);

                    return Json(new { success = true, data = siniestros });
                }
                else
                {
                    return Json(new { success = false, message = "No se pudieron obtener los siniestros de la póliza." });
                }
            }
            catch (Exception ex)
            {
                // En caso de error, retornamos el mensaje de error
                return Json(new { success = false, message = $"Ocurrió un error al cargar los siniestros: {ex.Message}" });
            }
        }

        #endregion
    }
}