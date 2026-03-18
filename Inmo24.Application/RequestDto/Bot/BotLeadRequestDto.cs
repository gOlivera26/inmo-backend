namespace Inmo24.Application.RequestDto.Bot;

public class BotLeadRequestDto
{
    public Guid TenantId { get; set; }
    public string Nombre { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string? Nota { get; set; }
    public string? EtapaCrm { get; set; }
}
