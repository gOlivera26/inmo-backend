using Inmo24.Application.ResponseDto.Mensajes;

namespace Inmo24.Application.Services.Interfaces;

public interface IMensajeService
{
    Task<OperationResponse<List<ChatSessionResponseDto>>> ObtenerSesionesChatAsync();
    Task<OperationResponse<List<ChatMessageResponseDto>>> ObtenerMensajesPorSesionAsync(string sessionId);
}
