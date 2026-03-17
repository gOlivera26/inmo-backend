namespace Inmo24.Application.RequestDto.Bot;

public class BotConfigUpdateRequestDto
{
    public bool Activo { get; set; }

    [Required]
    public string NombreBot { get; set; } = string.Empty;

    [Required]
    public string TonoConversacion { get; set; } = string.Empty;

    [Required]
    public string SaludoInicial { get; set; } = string.Empty;

    public string? TelefonoDerivacion { get; set; }
    public string? DirectricesExtra { get; set; }
}
