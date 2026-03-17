namespace Inmo24.Application.ResponseDto.Mensajes;

public class ChatMessageResponseDto
{
    public int Id { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string Contenido { get; set; } = string.Empty;
}
