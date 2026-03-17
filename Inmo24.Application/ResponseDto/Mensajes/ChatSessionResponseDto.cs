namespace Inmo24.Application.ResponseDto.Mensajes;

public class ChatSessionResponseDto
{
    public string SessionId { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string NombreCliente { get; set; } = string.Empty;
    public string UltimoMensaje { get; set; } = string.Empty;
}
