namespace Inmo24.Application.Services.Interfaces;

public interface IStorageService
{
    Task<string> SubirImagenAsync(IFormFile file, Guid tenantId, string prefix = "propiedades");
    Task<bool> BorrarImagenAsync(string fileUrl);
    Task<string> SubirDocumentoAsync(IFormFile file, Guid tenantId, string prefix = "documentos");
}