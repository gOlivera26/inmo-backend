using Inmo24.Application.RequestDto.Clientes;
using Inmo24.Application.ResponseDto.Clientes;

namespace Inmo24.Application.AutoMapper;

public class AutoMapperProfileClientes : Profile
{
    public AutoMapperProfileClientes()
    {
        CreateMap<ClienteCreateRequestDto, Cliente>();

        CreateMap<Cliente, ClienteResponseDto>()
            .ForMember(dest => dest.NombreCompleto, opt =>
                opt.MapFrom(src => string.IsNullOrWhiteSpace(src.Apellido) ? src.Nombre : $"{src.Nombre} {src.Apellido}"));
    }
}