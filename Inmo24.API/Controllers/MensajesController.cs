using Inmo24.Application.ResponseDto.Mensajes;
using System.Net;

namespace Inmo24.API.Controllers;

[ApiController]
public class MensajesController(IMensajeService mensajeService) : BaseController
{
    private readonly IMensajeService _mensajeService = mensajeService;

    [HttpGet("sesiones")]
    [ProducesResponseType(typeof(OperationResponse<List<ChatSessionResponseDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSesiones() =>
        Return(await _mensajeService.ObtenerSesionesChatAsync());

    [HttpGet("{sessionId}")]
    [ProducesResponseType(typeof(OperationResponse<List<ChatMessageResponseDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMensajesSesion(string sessionId)
    {
        // Uri.UnescapeDataString es necesario porque el ID viaja en la URL y tiene una '@' (ej: numero@s.whatsapp.net)
        var decodedSessionId = WebUtility.UrlDecode(sessionId);
        return Return(await _mensajeService.ObtenerMensajesPorSesionAsync(decodedSessionId));
    }
}