namespace Inmo24.Application.Services.Interfaces;

public interface ICatalogoService
{
    Task<OperationResponse<CatalogosResponseDto>> ObtenerTodosAsync();
}
