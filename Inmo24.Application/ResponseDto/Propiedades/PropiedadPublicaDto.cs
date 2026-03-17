namespace Inmo24.Application.ResponseDto.Propiedades;

public class PropiedadPublicaDto
{
    public Guid Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;
    public string ZonaNombre { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public string Operacion { get; set; } = string.Empty;
    public double? PrecioUsd { get; set; }
    public double? PrecioArs { get; set; }
    public string Moneda { get; set; } = string.Empty;
    public int? Ambientes { get; set; }
    public int? Dormitorios { get; set; }
    public int? Banios { get; set; }
    public double? SuperficieTotal { get; set; }
    public double? SuperficieCubierta { get; set; }
    public int? Antiguedad { get; set; }
    public string Descripcion { get; set; } = string.Empty;
    public string DescripcionBreve { get; set; } = string.Empty;
    public string InmobiliariaNombre { get; set; } = string.Empty;
    public string InmobiliariaTelefono { get; set; } = string.Empty;
    public string Titulo { get; set; } = string.Empty;
    public string ImagenPrincipalUrl { get; set; } = string.Empty;
}