namespace Inmo24.Application.ResponseDto.Dashboard;

public class VisitaDashboardDto
{
    public Guid Id { get; set; }
    public string ClienteNombre { get; set; } = string.Empty;
    public string PropiedadTitulo { get; set; } = string.Empty;
    public DateTime FechaHora { get; set; }
}