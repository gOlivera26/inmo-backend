namespace Inmo24.Application.ResponseDto.Bot;

public class BotPromptActivoDto
{
    public bool BotActivo { get; set; }
    public string PromptArmado { get; set; } = string.Empty;
    public string TelefonoDerivacion { get; set; } = string.Empty;
    public Guid TenantId { get; set; }
}
