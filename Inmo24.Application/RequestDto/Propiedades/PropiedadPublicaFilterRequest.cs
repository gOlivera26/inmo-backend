using Inmo24.Application.RequestDto.Common;

namespace Inmo24.Application.RequestDto.Propiedades;

public class PropiedadPublicaFilterRequest : PaginatedRequest
{
    public string? Operacion { get; set; }
    public string? Busqueda { get; set; }
    public string? TipoPropiedad { get; set; }
    public Guid? TenantId { get; set; }
}
