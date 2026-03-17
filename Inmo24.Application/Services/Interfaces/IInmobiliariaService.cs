using Inmo24.Application.RequestDto.Inmobiliaria;
using Inmo24.Application.ResponseDto.Inmobiliaria;

namespace Inmo24.Application.Services.Interfaces;

public interface IInmobiliariaService
{
    Task<OperationResponse<InmobiliariaResponseDto>> ObtenerMiAgenciaAsync();
    Task<OperationResponse<InmobiliariaResponseDto>> ActualizarMiAgenciaAsync(InmobiliariaUpdateRequestDto request);
    Task<OperationResponse<string>> ActualizarLogoAsync(IFormFile file);
}
