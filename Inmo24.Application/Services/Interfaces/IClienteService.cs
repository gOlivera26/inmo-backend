using Inmo24.Application.RequestDto.Clientes;
using Inmo24.Application.ResponseDto.Clientes;

namespace Inmo24.Application.Services.Interfaces;

public interface IClienteService
{
    Task<OperationResponse<CrmResumenResponseDto>> ObtenerResumenCrmAsync();
    Task<OperationResponse<List<ClienteResponseDto>>> ObtenerClientesAsync(ClienteFilterRequest request);
    Task<OperationResponse<ClienteDetalleResponseDto>> ObtenerClientePorIdAsync(Guid id);
    Task<OperationResponse<ClienteResponseDto>> CrearClienteAsync(ClienteCreateRequestDto request);
    Task<OperationResponse<ClienteResponseDto>> ActualizarClienteAsync(Guid id, ClienteUpdateRequestDto request);
}
