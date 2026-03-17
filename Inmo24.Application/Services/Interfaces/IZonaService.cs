using Inmo24.Application.RequestDto.Zonas;
using Inmo24.Application.ResponseDto.Zonas;

namespace Inmo24.Application.Services.Interfaces;

public interface IZonaService
{
    Task<OperationResponse<List<ZonaResponseDto>>> ObtenerZonasAsync();
    Task<OperationResponse<ZonaResponseDto>> CrearZonaAsync(ZonaCreateRequestDto request);
    Task<OperationResponse<ZonaResponseDto>> ActualizarZonaAsync(int id, ZonaUpdateRequestDto request);
    Task<OperationResponse<bool>> EliminarZonaAsync(int id);
}
