namespace Inmo24.Application.ResponseDto.Dashboard;

public class PropiedadDestacadaDashboardDto
{
    public Guid Id { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string Ubicacion { get; set; } = string.Empty;
    public decimal Precio { get; set; }
    public string Moneda { get; set; } = string.Empty;
    public string Estado { get; set; } = string.Empty;
    public string ImagenUrl { get; set; } = string.Empty;
}
