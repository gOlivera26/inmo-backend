namespace Inmo24.Application.RequestDto.Propiedades;

public class PropiedadCreateRequestDto
{
    [Required]
    public string Codigo { get; set; } = string.Empty; // Ej: CASA001

    public int? ZonaId { get; set; }

    [Required]
    public string Direccion { get; set; } = string.Empty;

    [Required]
    public short TipoId { get; set; }

    [Required]
    public short OperacionId { get; set; }

    public decimal? PrecioUsd { get; set; }
    public decimal? PrecioArs { get; set; }
    public string Moneda { get; set; } = "USD";

    public short? Ambientes { get; set; }
    public short? Dormitorios { get; set; }
    public short? Banios { get; set; }

    public string? DescripcionBreve { get; set; }
    public string? Descripcion { get; set; }
}