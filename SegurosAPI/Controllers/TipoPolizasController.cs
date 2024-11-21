using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using SegurosAPI.Data;
using SegurosAPI.Models;

namespace SegurosAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TipoPolizasController : Controller
    {
        private readonly SegurosDbContext _context;
        private readonly IMapper _mapper;
        public TipoPolizasController(SegurosDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> Contultar_TipoPolizas()
        {
            var TipoPolizas = await _context.Contultar_TipoPolizas();
            var TipoPolizasDTO = _mapper.Map<IEnumerable<TipoPolizasDTO>>(TipoPolizas);
            return Ok(TipoPolizas);
        }


    }
}
