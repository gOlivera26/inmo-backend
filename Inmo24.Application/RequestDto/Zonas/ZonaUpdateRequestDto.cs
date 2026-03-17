namespace Inmo24.Application.RequestDto.Zonas;

public class ZonaUpdateRequestDto
{
    [Required(ErrorMessage = "El nombre de la zona es obligatorio")]
    public string Nombre { get; set; } = string.Empty;
    public string? Descripcion { get; set; }
    public bool Activa { get; set; }
}
