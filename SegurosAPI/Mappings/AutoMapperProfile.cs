using AutoMapper;
using SegurosAPI.Models;

namespace SegurosAPI.Mappings
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            // Mapeo 
            CreateMap<PolizaDTO, PolizaDTO>().ReverseMap();
            CreateMap<TipoPolizasDTO, TipoPolizasDTO>().ReverseMap();
            CreateMap<UsuariosDTO, UsuariosDTO>().ReverseMap();
            CreateMap<spCotizacion_polizasUsuarioDTO, spCotizacion_polizasUsuarioDTO>().ReverseMap();
            CreateMap<SiniestrosDTO, SiniestrosDTO>().ReverseMap();
        }
    }
}