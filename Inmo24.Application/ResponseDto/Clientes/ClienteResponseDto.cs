namespace Inmo24.Application.ResponseDto.Clientes;

public class ClienteResponseDto
{
    public Guid Id { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string EtapaCrm { get; set; } = string.Empty;
    public DateTime CreadoEl { get; set; }
}
