namespace Inmo24.Application.ResponseDto.Propiedades;

public class PropiedadBackofficeDto
{
    public Guid Id { get; set; }
    public string Codigo { get; set; } = string.Empty;
    public string Direccion { get; set; } = string.Empty;
    public string ZonaNombre { get; set; } = string.Empty;
    public string Tipo { get; set; } = string.Empty;
    public string Operacion { get; set; } = string.Empty;
    public decimal? PrecioUsd { get; set; }
    public string FaseCarga { get; set; } = string.Empty;
    public string EstadoComercial { get; set; } = string.Empty;
    public DateTime CreadoEl { get; set; }
    public bool Destacada { get; set; }
}