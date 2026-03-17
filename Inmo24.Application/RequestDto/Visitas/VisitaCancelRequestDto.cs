namespace Inmo24.Application.RequestDto.Visitas;

public class VisitaCancelRequestDto
{
    [Required]
    public string Telefono { get; set; } = string.Empty;
    [Required]
    public DateTime DiaVisita { get; set; }
    public string? DireccionCasa { get; set; }
}
