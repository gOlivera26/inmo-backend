namespace Inmo24.Application.ResponseDto.Clientes;

public class ClienteDetalleResponseDto
{
    public Guid Id { get; set; }
    public string NombreCompleto { get; set; } = string.Empty;
    public string Telefono { get; set; } = string.Empty;
    public string? Email { get; set; }
    public string EtapaCrm { get; set; } = string.Empty;
    public string Notas { get; set; } = string.Empty;
    public DateTime CreadoEl { get; set; }
    public List<ClienteVisitaDto> Visitas { get; set; } = new();
    public List<ClienteOperacionDto> Operaciones { get; set; } = new();
}

public class ClienteVisitaDto
{
    public Guid VisitaId { get; set; }
    public Guid PropiedadId { get; set; }
    public string PropiedadTitulo { get; set; } = string.Empty;
    public string PropiedadImagenUrl { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public DateTime FechaVisita { get; set; }
}

public class ClienteOperacionDto
{
    public Guid OperacionId { get; set; }
    public Guid PropiedadId { get; set; }
    public string Tipo { get; set; } = string.Empty;
    public string PropiedadTitulo { get; set; } = string.Empty;
    public decimal Monto { get; set; }
    public string Moneda { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
}
