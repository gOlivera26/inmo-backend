using Inmo24.Application.RequestDto.Propiedades;
using Inmo24.Application.ResponseDto.Propiedades;

namespace Inmo24.Application.Services.Implementations;

public interface IPropiedadService
{
    Task<OperationResponse<Propiedades>> CrearBorradorAsync(PropiedadCreateRequestDto request);
    Task<OperationResponse<List<PropiedadBackofficeDto>>> ObtenerMisPropiedadesAsync(int page, int pageSize);

    // Para el portal público (Marketplace)
    Task<OperationResponse<List<PropiedadPublicaDto>>> ObtenerCatalogoPublicoAsync(int page, int pageSize);
}
