using Inmo24.Application.ResponseDto.Documentos;

namespace Inmo24.Application.Services.Interfaces;

public interface IDocumentosService
{
    Task<OperationResponse<DocumentoResponseDto>> SubirDocumentoAsync(Guid clienteId, IFormFile file, string tipoEntidad, string categoria, Guid? entidadReferenciaId = null);
    Task<OperationResponse<bool>> EliminarDocumentoAsync(Guid id);
    Task<OperationResponse<List<DocumentoResponseDto>>> ObtenerDocumentosClienteAsync(Guid clienteId);
    Task<OperationResponse<bool>> CambiarEstadoDocumentoAsync(Guid id, string nuevoEstado);
}