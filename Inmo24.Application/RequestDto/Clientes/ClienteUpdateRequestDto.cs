namespace Inmo24.Application.RequestDto.Clientes;

public class ClienteUpdateRequestDto
{
    [Required]
    public string NombreCompleto { get; set; } = string.Empty;
    public string? Email { get; set; }

    [Required]
    public string EtapaCrm { get; set; } = string.Empty;
}
