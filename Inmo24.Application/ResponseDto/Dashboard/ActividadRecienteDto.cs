namespace Inmo24.Application.ResponseDto.Dashboard;

public class ActividadRecienteDto
{
    public string Tipo { get; set; } = string.Empty; // "propiedad", "contrato", "lead", "visita"
    public string Titulo { get; set; } = string.Empty;
    public string Descripcion { get; set; } = string.Empty;
    public DateTime Fecha { get; set; }
}