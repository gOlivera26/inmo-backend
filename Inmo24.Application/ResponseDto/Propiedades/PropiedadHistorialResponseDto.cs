namespace Inmo24.Application.ResponseDto.Propiedades;

public class PropiedadHistorialResponseDto
{
    public Guid Id { get; set; }
    public DateTime Fecha { get; set; }
    public string EstadoAnterior { get; set; } = string.Empty;
    public string EstadoNuevo { get; set; } = string.Empty;
    public string Observacion { get; set; } = string.Empty;
    public string Usuario { get; set; } = string.Empty;
}