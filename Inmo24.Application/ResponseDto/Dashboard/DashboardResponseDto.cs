namespace Inmo24.Application.ResponseDto.Dashboard;

public class DashboardResponseDto
{
    public int PropiedadesPublicadasMes { get; set; }
    public KpiDto IngresosTotales { get; set; } = new();
    public KpiDto StockActivo { get; set; } = new();
    public KpiDto BaseClientes { get; set; } = new();
    public KpiDto VisitasMes { get; set; } = new();
    public List<PropiedadDestacadaDashboardDto> PropiedadesDestacadas { get; set; } = new();
    public List<VisitaDashboardDto> ProximasVisitas { get; set; } = new();
    public List<ActividadRecienteDto> ActividadReciente { get; set; } = new();
    public Dictionary<string, int> DistribucionEstados { get; set; } = new();
}
