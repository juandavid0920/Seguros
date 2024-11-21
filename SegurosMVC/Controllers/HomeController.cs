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
    public class HomeController : Controller
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("http://localhost:5084/api/") // URL base de la API
            };
        }

        public  IActionResult IndexAsync()
        {            
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public HttpClient Get_httpClient()
        {
            return _httpClient;
        }

        #region Metodos Usuario

        [HttpPost]
        public async Task<IActionResult> Usuarios_CrearUsuarioNuevoAsync([FromBody] UsuariosDTO usuario)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Realizamos la solicitud POST hacia la API externa (Usando la URL base configurada)
                    var jsonContent = new StringContent(
                        JsonConvert.SerializeObject(usuario),
                        Encoding.UTF8,
                        "application/json"
                    );

                    var response = await _httpClient.PostAsync("Usuarios/Usuarios_CrearUsuarioNuevo", jsonContent);

                    // Verificamos si la respuesta fue exitosa

                    if (response.IsSuccessStatusCode)
                    {
                        // Leer la respuesta de la API (debería ser un JSON)
                        var resultadoJson = await response.Content.ReadAsStringAsync();

                        // Deserializar la respuesta JSON
                        var apiResponse = JsonConvert.DeserializeObject<ApiResponse>(resultadoJson);

                        // Validar el valor de resultado
                        if (apiResponse != null)
                        {
                            switch (apiResponse.Resultado)
                            {
                                case 1: // Usuario creado correctamente
                                    return Json(new { success = true, message = "Usuario registrado exitosamente." });

                                case 2: // Correo ya registrado
                                    return Json(new { success = false, message = "El correo ya se encuentra registrado." });

                                default: // Otro error de negocio
                                    return Json(new { success = false, message = "No se puede crear el usuario, contacte al administrador del sistema." });
                            }
                        }
                        else
                        {
                            return Json(new { success = false, message = "Respuesta inesperada del servidor." });
                        }
                    }
                    else
                    {
                        // Si la respuesta de la API no es exitosa (e.g., 400 o 500)
                        return Json(new { success = false, message = "Error al comunicarse con el servidor. Código: " + response.StatusCode });
                    }
                }
                catch (Exception ex)
                {
                    // Manejo de excepciones
                    return Json(new { success = false, message = $"Ocurrió un error: {ex.Message}" });
                }
            }

            return Json(new { success = false, message = "Error al registrar el usuario. Verifica los datos ingresados." });
        }

        [HttpPost]
        public async Task<IActionResult> LoginUsuario([FromBody] UsuariosDTO usuario)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Serializamos el objeto usuario a JSON para enviarlo en la solicitud POST
                    var jsonContent = new StringContent(
                        JsonConvert.SerializeObject(usuario),
                        Encoding.UTF8,
                        "application/json"
                    );

                    // Realizamos la solicitud POST hacia la API externa
                    var response = await _httpClient.PostAsync("Usuarios/Usuarios_ConsultaUsuarioCorreo", jsonContent);

                    // Verificamos si la respuesta fue exitosa
                    if (response.IsSuccessStatusCode)
                    {
                        // Leer la respuesta de la API (debería ser un JSON)
                        var resultadoJson = await response.Content.ReadAsStringAsync();

                        // Deserializamos la respuesta de la API a un objeto que contiene "resultado"
                        var apiResponse = JsonConvert.DeserializeObject<ApiResponseUsuarioDTO>(resultadoJson);

                        if (apiResponse != null && apiResponse.Resultado != null)
                        {
                            // Validamos si el correo electrónico y contraseña coinciden
                            if (apiResponse.Resultado.CorreoElectronico == usuario.CorreoElectronico && apiResponse.Resultado.Contraseña == usuario.Contraseña)
                            {                                
                                    // Guardamos la información del usuario en la sesión
                                    UsuarioSessionHelper.SetUsuario(HttpContext, apiResponse.Resultado);

                                if (apiResponse.Resultado.Rol == "Cliente")
                                {
                                    return Json(new { success = true, redireccion = "/Gestiones/GestionPoliza", message = $"Bienvenido {apiResponse.Resultado.Nombre} usted tiene el Rol {apiResponse.Resultado.Rol}." });
                                }
                                else if (apiResponse.Resultado.Rol == "Administrador")
                                {
                                    return Json(new { success = true, redireccion = "/Gestiones/GestionSiniestros", message = $"Bienvenido {apiResponse.Resultado.Nombre} usted tiene el Rol {apiResponse.Resultado.Rol}." });
                                }
                                else
                                {
                                    return Json(new { success = true, redireccion = "/Gestiones/AccesoDenegado", message = $"Bienvenido {apiResponse.Resultado.Nombre} usted tiene el Rol {apiResponse.Resultado.Rol}." });
                                }                              
                                
                            }
                            else
                            {
                                return Json(new { success = false, message = "Correo electrónico o contraseña no coinciden." });
                            }
                        }
                        else
                        {
                            return Json(new { success = false, message = "Usuario no encontrado." });
                        }
                    }
                    else
                    {
                        // Si la respuesta de la API no es exitosa (e.g., 400, 500)
                        return Json(new { success = false, message = "Error al comunicarse con el servidor. Código: " + response.StatusCode });
                    }
                }
                catch (Exception ex)
                {
                    // Manejo de excepciones
                    return Json(new { success = false, message = $"Ocurrió un error: {ex.Message}" });
                }
            }

            // Si los datos del modelo no son válidos, regresamos un mensaje de error
            return Json(new { success = false, message = "Error en el ingreso. Verifica los datos ingresados." });
        }

        #endregion

        #region Sesion Usuario

        public IActionResult Logout()
        {
            // Eliminar los datos del usuario de la sesión
            UsuarioSessionHelper.Logout(HttpContext);

            // Redirigir al inicio o a una página de login
            return RedirectToAction("Index", "Home");
        }

        //Ejemplo para poder acceder a la sesion desde otro controlador
        public IActionResult Dashboard()
        {
            // Obtener los datos del usuario de la sesión
            var usuario = UsuarioSessionHelper.GetUsuario(HttpContext);

            if (usuario != null)
            {
                ViewBag.UserName = usuario.Nombre;
                ViewBag.UserRole = usuario.Rol;
            }
            else
            {
                ViewBag.UserName = "Invitado";
                ViewBag.UserRole = "Sin rol";
            }

            return View();
        }

        #endregion


    }
}
