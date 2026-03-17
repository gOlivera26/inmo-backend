
namespace Inmo24.Application.RequestDto.Clientes;

public class ClienteCreateRequestDto
{
    [Required][MaxLength(150)] public string Nombre { get; set; } = string.Empty;
    [MaxLength(150)] public string? Apellido { get; set; }
    [Required][MaxLength(50)] public string Telefono { get; set; } = string.Empty;
    [EmailAddress][MaxLength(255)] public string? Email { get; set; }
    public string? Notas { get; set; }
    public string? EtapaCrm { get; set; }
}