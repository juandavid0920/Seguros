using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SegurosAPI.Data;
using SegurosAPI.Models;

namespace SegurosAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PolizasController : ControllerBase
    {
        private readonly SegurosDbContext _context;
        private readonly IMapper _mapper;

        public PolizasController(SegurosDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetPolizas()
        {
            var polizas = await _context.Contultar_PolizasAsync();
            var polizasDTO = _mapper.Map<IEnumerable<PolizaDTO>>(polizas);
            return Ok(polizasDTO);
        }

        [HttpPost("Polizas_CrearPoliza")]
        public async Task<IActionResult> Polizas_CrearPolizaAsync(PolizaDTO polizas)
        {
            var resultado = await _context.Polizas_CrearPoliza(polizas);
            if (resultado)
            {
                return StatusCode(200, new { resultado = 1, Message = "Cotizacion Registrada con Exito." });
            }
            
            else
            {
                return StatusCode(500, new { resultado = 0, Message = "Error al Registrar la Poliza." });
            }
        }

        [HttpGet("Cotizacion_polizasUsuario")]
        public async Task<IActionResult> Cotizacion_polizasUsuario(int usuarioId)
        {
            var Cotizacion_polizasUsuario = await _context.spCotizacion_polizasUsuario(usuarioId);
            var listaPolizas = _mapper.Map<IEnumerable<spCotizacion_polizasUsuarioDTO>>(Cotizacion_polizasUsuario);
            return Ok(listaPolizas);
        }

        [HttpPost("Polizas_PolizaPagar")]
        public async Task<IActionResult> Polizas_PolizaPagar(PolizaDTO polizas)
        {
            var resultado = await _context.Polizas_PolizaPagar(polizas);
            if (resultado)
            {
                return StatusCode(200, new { resultado = 1, Message = "Poliza Pagada con Exito." });
            }

            else
            {
                return StatusCode(500, new { resultado = 0, Message = "No se pude registrar el Pago." });
            }
        }

        [HttpPost("Polizas_PolizaNoAdquirir")]
        public async Task<IActionResult> Polizas_PolizaNoAdquirir(PolizaDTO polizas)
        {
            var resultado = await _context.Polizas_PolizaNoAdquirir(polizas);
            if (resultado)
            {
                return StatusCode(200, new { resultado = 1, Message = "Poliza Pagada con Exito." });
            }

            else
            {
                return StatusCode(500, new { resultado = 0, Message = "No se pude registrar el Pago." });
            }
        }


        #region Siniestros
        [HttpPost("Siniestro_AgregarNuevoPoliza")]
        public async Task<IActionResult> Siniestro_AgregarNuevoPoliza(SiniestrosDTO siniestro)
        {
            var resultado = await _context.Siniestro_AgregarNuevoPoliza(siniestro);
            if (resultado)
            {
                return StatusCode(200, new { resultado = 1, Message = "Siniestro registrado con Exito." });
            }

            else
            {
                return StatusCode(500, new { resultado = 0, Message = "No se pude registrar el Siniestro." });
            }
        }



        [HttpGet("Siniestro_ConsultaPoliza")]
        public async Task<IActionResult> Siniestro_ConsultaPoliza(int PolizaId)
        {
            var Siniestros = await _context.Siniestro_ConsultaPoliza(PolizaId);
            var listaSiniestros = _mapper.Map<IEnumerable<SiniestrosDTO>>(Siniestros);
            return Ok(listaSiniestros);
        }

        #endregion


    }
}