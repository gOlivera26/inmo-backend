namespace Inmo24.Application.ResponseDto.Propiedades;

public class PropiedadDetalleDto
{
    public Guid Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;
    public string ZonaNombre { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public string Operacion { get; set; } = string.Empty;
    public string FaseCarga { get; set; } = string.Empty;
    public string EstadoComercial { get; set; } = string.Empty;
    public int? ZonaId { get; set; }
    public short TipoId { get; set; }
    public short OperacionId { get; set; }
    public short FaseCargaId { get; set; }
    public short EstadoComercialId { get; set; }
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
    public DateTime CreadoEl { get; set; }
    public DateTime? ModificadoEl { get; set; }
    public string Titulo { get; set; } = string.Empty;
    public string? VideoUrl { get; set; }
    public double? Latitud { get; set; }
    public double? Longitud { get; set; }
    public short? Cocheras { get; set; }
    public List<PropiedadImagenDto> PropiedadImagenes { get; set; } = new();
}