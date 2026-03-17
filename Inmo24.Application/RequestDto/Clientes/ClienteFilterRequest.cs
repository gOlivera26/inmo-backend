using Inmo24.Application.RequestDto.Common;
namespace Inmo24.Application.RequestDto.Clientes;

public class ClienteFilterRequest : PaginatedRequest
{
    public string? Busqueda { get; set; }
    public string? EtapaCrm { get; set; }
}