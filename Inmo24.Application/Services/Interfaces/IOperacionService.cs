using Inmo24.Application.RequestDto.Operaciones;
using Inmo24.Application.ResponseDto.Operaciones;

namespace Inmo24.Application.Services.Interfaces;

public interface IOperacionComercialService
{
    Task<OperationResponse<bool>> CerrarOperacionAsync(CerrarOperacionRequestDto request);
    Task<OperationResponse<OperacionDetalleResponseDto>> ObtenerOperacionPorPropiedadAsync(Guid propiedadId);
}