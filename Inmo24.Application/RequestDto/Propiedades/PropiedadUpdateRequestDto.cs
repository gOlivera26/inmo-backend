namespace Inmo24.Application.RequestDto.Propiedades;

public class PropiedadUpdateRequestDto
{
    public string Codigo { get; set; } = string.Empty;
    public int? ZonaId { get; set; }
    [Required]
    public string Direccion { get; set; } = string.Empty;
    [Required] public short TipoId { get; set; }
    [Required] public short OperacionId { get; set; }
    [Required] public short FaseCargaId { get; set; }
    [Required] public short EstadoComercialId { get; set; }
    public decimal? PrecioUsd { get; set; }
    public decimal? PrecioArs { get; set; }
    public string Moneda { get; set; } = "USD";
    public decimal? SuperficieTotal { get; set; }
    public decimal? SuperficieCubierta { get; set; }
    public short? Ambientes { get; set; }
    public short? Dormitorios { get; set; }
    public short? Banios { get; set; }
    public short? Antiguedad { get; set; }
    public string? DescripcionBreve { get; set; }
    public string? Descripcion { get; set; }
    public bool Destacada { get; set; }
    public bool NotificarLeads { get; set; }
    public string Titulo { get; set; } = string.Empty;
}