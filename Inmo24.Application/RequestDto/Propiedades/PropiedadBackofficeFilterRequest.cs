using Inmo24.Application.RequestDto.Common;

namespace Inmo24.Application.RequestDto.Propiedades;

public class PropiedadBackofficeFilterRequest : PaginatedRequest
{
    public string? Busqueda { get; set; }
    public short? FaseCargaId { get; set; }
    public int? EstadoComercialId { get; set; }
}