namespace Inmo24.Application.ResponseDto.Bot;

public class BotConfigResponseDto
{
    public bool Activo { get; set; }
    public string NombreBot { get; set; } = string.Empty;
    public string TonoConversacion { get; set; } = string.Empty;
    public string SaludoInicial { get; set; } = string.Empty;
    public string TelefonoDerivacion { get; set; } = string.Empty;
    public string DirectricesExtra { get; set; } = string.Empty;
}
