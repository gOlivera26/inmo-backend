using Inmo24.Application.RequestDto.Visitas;
using Inmo24.Application.ResponseDto.Visitas;

namespace Inmo24.Application.Services.Interfaces;

public interface IVisitaService
{
    Task<OperationResponse<VisitaResponseDto>> AgendarVisitaAsync(VisitaCreateRequestDto request);
    Task<OperationResponse<List<VisitaResponseDto>>> ObtenerVisitasMesAsync(int anio, int mes);
    Task<OperationResponse<bool>> CambiarEstadoVisitaAsync(Guid id, string nuevoEstado);
    Task<OperationResponse<bool>> CancelarVisitaBotAsync(VisitaCancelRequestDto request);
}
