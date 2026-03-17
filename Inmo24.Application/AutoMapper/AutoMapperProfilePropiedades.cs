

using Inmo24.Application.RequestDto.Propiedades;
using Inmo24.Application.ResponseDto.Propiedades;

namespace Inmo24.Application.AutoMapper
{
    public class AutoMapperProfilePropiedades : Profile
    {
        public AutoMapperProfilePropiedades()
        {
            CreateMap<PropiedadCreateRequestDto, Propiedades>();

            CreateMap<PropiedadImagene, PropiedadImagenDto>();

            CreateMap<Propiedades, PropiedadBackofficeDto>()
                .ForMember(dest => dest.ZonaNombre, opt => opt.MapFrom(src => src.Zona != null ? src.Zona.Nombre : "Sin zona"))
                .ForMember(dest => dest.Tipo, opt => opt.MapFrom(src => src.Tipo != null ? src.Tipo.Nombre : ""))
                .ForMember(dest => dest.Operacion, opt => opt.MapFrom(src => src.Operacion != null ? src.Operacion.Nombre : ""))
                .ForMember(dest => dest.FaseCarga, opt => opt.MapFrom(src => src.FaseCarga != null ? src.FaseCarga.Nombre : ""))
                .ForMember(dest => dest.EstadoComercial, opt => opt.MapFrom(src => src.EstadoComercial != null ? src.EstadoComercial.Nombre : ""))
                .ForMember(dest => dest.ImagenPrincipalUrl, opt => opt.MapFrom(src => 
                src.PropiedadImagenes.Where(i => !i.IsDeleted && i.EsPrincipal).Select(i => i.Url).FirstOrDefault() ?? 
                src.PropiedadImagenes.Where(i => !i.IsDeleted).OrderBy(i => i.Orden).Select(i => i.Url).FirstOrDefault()));

            CreateMap<Propiedades, PropiedadPublicaDto>()
                .ForMember(dest => dest.ZonaNombre, opt => opt.MapFrom(src => src.Zona != null ? src.Zona.Nombre : "Sin zona"))
                .ForMember(dest => dest.Tipo, opt => opt.MapFrom(src => src.Tipo != null ? src.Tipo.Nombre : ""))
                .ForMember(dest => dest.Operacion, opt => opt.MapFrom(src => src.Operacion != null ? src.Operacion.Nombre : ""))
                .ForMember(dest => dest.InmobiliariaNombre, opt => opt.MapFrom(src => src.Tenant != null ? src.Tenant.Nombre : "Desconocida"))
                .ForMember(dest => dest.InmobiliariaTelefono, opt => opt.MapFrom(src => src.Tenant != null ? src.Tenant.Telefono : ""))
                .ForMember(dest => dest.ImagenPrincipalUrl, opt => opt.MapFrom(src =>
                src.PropiedadImagenes.Where(i => !i.IsDeleted && i.EsPrincipal).Select(i => i.Url).FirstOrDefault() ??
                src.PropiedadImagenes.Where(i => !i.IsDeleted).OrderBy(i => i.Orden).Select(i => i.Url).FirstOrDefault()));

            CreateMap<PropiedadUpdateRequestDto, Propiedades>()
            .ForMember(dest => dest.Id, opt => opt.Ignore())
            .ForMember(dest => dest.TenantId, opt => opt.Ignore())
            .ForMember(dest => dest.Codigo, opt => opt.Ignore());

            CreateMap<Propiedades, PropiedadDetalleDto>()
                .ForMember(dest => dest.ZonaNombre, opt => opt.MapFrom(src => src.Zona != null ? src.Zona.Nombre : "Sin zona"))
                .ForMember(dest => dest.Tipo, opt => opt.MapFrom(src => src.Tipo != null ? src.Tipo.Nombre : ""))
                .ForMember(dest => dest.Operacion, opt => opt.MapFrom(src => src.Operacion != null ? src.Operacion.Nombre : ""))
                .ForMember(dest => dest.FaseCarga, opt => opt.MapFrom(src => src.FaseCarga != null ? src.FaseCarga.Nombre : ""))
                .ForMember(dest => dest.EstadoComercial, opt => opt.MapFrom(src => src.EstadoComercial != null ? src.EstadoComercial.Nombre : ""));
        }
    }
}
