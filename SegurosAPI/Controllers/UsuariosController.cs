using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SegurosAPI.Data;
using SegurosAPI.Models;

namespace SegurosAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsuariosController : Controller
    {
        private readonly SegurosDbContext _context;
        private readonly IMapper _mapper;

        public UsuariosController(SegurosDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpPost("Usuarios_CrearUsuarioNuevo")]
        public async Task<IActionResult> Usuarios_CrearUsuarioNuevoAsync(UsuariosDTO usuario)
        {
            var resultado = await _context.Usuarios_CrearUsuarioNuevoAsync(usuario);
            if (resultado == 1)
            {
                return StatusCode(200, new { resultado = 1, Message = "Usuario creada exitosamente." });
            }
            else if (resultado == 2)
            {
                return StatusCode(200, new { resultado = 2, Message = "El correo registrado ya existe." });
            }
            else
            {
                return StatusCode(500, new { resultado = 0, Message = "Error al crear el usuario." });
            }
        }

        [HttpPost("Usuarios_ConsultaUsuarioCorreo")]
        public async Task<IActionResult> Usuarios_ConsultaUsuarioCorreo(UsuariosDTO usuario)
        {
            var resultado = await _context.Usuarios_ConsultaUsuarioCorreo(usuario.CorreoElectronico);
            if (resultado.UsuarioId == 0)
            {
                return NotFound(new { message = "Usuario no encontrado.", resultado });
            }
            return StatusCode(200, new { resultado });
        }

        [HttpGet("GetUsuarios_Obtener")]
        public async Task<IActionResult> GetUsuarios_Obtener()
        {
            var Usuarios = await _context.Usuarios_Obtener();
            var UsuariosDTO = _mapper.Map<IEnumerable<UsuariosDTO>>(Usuarios);
            return Ok(UsuariosDTO);
        }

    }
}
