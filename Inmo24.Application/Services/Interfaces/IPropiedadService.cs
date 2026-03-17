using Inmo24.Application.RequestDto.Propiedades;
using Inmo24.Application.ResponseDto.Propiedades;

namespace Inmo24.Application.Services.Implementations;

public interface IPropiedadService
{
    Task<OperationResponse<Propiedades>> CrearBorradorAsync(PropiedadCreateRequestDto request);
    Task<OperationResponse<List<PropiedadBackofficeDto>>> ObtenerMisPropiedadesAsync(PropiedadBackofficeFilterRequest request);
    Task<OperationResponse<List<PropiedadPublicaDto>>> ObtenerCatalogoPublicoAsync(PropiedadPublicaFilterRequest request);
    Task<OperationResponse<PropiedadDetalleDto>> ObtenerPorIdAsync(Guid id);
    Task<OperationResponse<PropiedadDetalleDto>> ActualizarAsync(Guid id, PropiedadUpdateRequestDto request);
    Task<OperationResponse<bool>> EliminarAsync(Guid id);
    Task<OperationResponse<PropiedadImagenDto>> SubirImagenAsync(Guid propiedadId, IFormFile file);
    Task<OperationResponse<bool>> EliminarImagenAsync(Guid propiedadId, Guid imagenId);
    Task<OperationResponse<bool>> EstablecerImagenPrincipalAsync(Guid propiedadId, Guid imagenId);
    Task<OperationResponse<List<PropiedadHistorialResponseDto>>> ObtenerHistorialAsync(Guid propiedadId);
    Task<OperationResponse<List<string>>> ObtenerUrlsImagenesAsync(Guid propiedadId);
    Task<OperationResponse<PropiedadPublicaDto>> ObtenerDetallePublicoAsync(Guid id);
}
