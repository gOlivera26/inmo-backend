namespace Inmo24.Application.ResponseDto.Propiedades;

public class PropiedadPublicaDto
{
    public Guid Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;
    public string ZonaNombre { get; set; } = string.Empty; // AutoMapper lo saca de Zona.Nombre
    public string Tipo { get; set; } = string.Empty;
    public string Operacion { get; set; } = string.Empty;
    public decimal? PrecioUsd { get; set; }
    public decimal? PrecioArs { get; set; }
    public string Moneda { get; set; } = string.Empty;
    public short? Dormitorios { get; set; }
    public string? DescripcionBreve { get; set; }

    public string InmobiliariaNombre { get; set; } = string.Empty;
    public string InmobiliariaTelefono { get; set; } = string.Empty;
}