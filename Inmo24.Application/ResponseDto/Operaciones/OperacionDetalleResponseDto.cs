using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Inmo24.Application.ResponseDto.Operaciones;

public class OperacionDetalleResponseDto
{
    public string TipoOperacion { get; set; } = string.Empty;
    public Guid ClienteId { get; set; }
    public string ClienteNombre { get; set; } = string.Empty;
    public string ClienteTelefono { get; set; } = string.Empty;

    public string Moneda { get; set; } = string.Empty;
    public DateTime FechaOperacion { get; set; }
    public decimal? MontoTotal { get; set; }
    public decimal? ComisionInmobiliaria { get; set; }
    public string? Escribania { get; set; }
    public decimal? MontoMensual { get; set; }
    public DateTime? FechaFin { get; set; }
    public short? DiaVencimientoPago { get; set; }
    public string? EstadoContrato { get; set; }
}