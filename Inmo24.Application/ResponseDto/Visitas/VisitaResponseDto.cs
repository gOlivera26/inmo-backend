
namespace Inmo24.Application.ResponseDto.Visitas;

public class VisitaResponseDto
{
    public Guid Id { get; set; }
    public DateTime FechaVisita { get; set; }
    public string Estado { get; set; } = string.Empty; // AGENDADA, CANCELADA, CONFIRMADA
    public short DuracionMinutos { get; set; }
    public Guid? ClienteId { get; set; }
    public string NombreCliente { get; set; } = string.Empty;
    public string TelefonoCliente { get; set; } = string.Empty;
    public Guid? PropiedadId { get; set; }
    public string PropiedadTitulo { get; set; } = string.Empty;
    public string PropiedadImagenUrl { get; set; } = string.Empty;
    public string DireccionCasa { get; set; } = string.Empty;
}
