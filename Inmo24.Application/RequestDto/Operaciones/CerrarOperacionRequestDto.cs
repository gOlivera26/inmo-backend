using System.ComponentModel.DataAnnotations;

namespace Inmo24.Application.RequestDto.Operaciones;

public class CerrarOperacionRequestDto
{
    [Required] public string TipoOperacion { get; set; } = string.Empty; // "VENTA" o "ALQUILER"
    [Required] public Guid PropiedadId { get; set; }
    [Required] public Guid ClienteId { get; set; }
    public string Moneda { get; set; } = "USD";
    public decimal? MontoTotal { get; set; }
    public decimal? ComisionInmobiliaria { get; set; }
    public DateTime? FechaVenta { get; set; }
    public string? Escribania { get; set; }
    public string? Notas { get; set; }
    public decimal? MontoMensual { get; set; }
    public DateTime? FechaInicio { get; set; }
    public DateTime? FechaFin { get; set; }
    public short? DiaVencimientoPago { get; set; }
}