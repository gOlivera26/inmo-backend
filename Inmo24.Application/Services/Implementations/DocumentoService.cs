using Inmo24.Application.ResponseDto.Documentos;

namespace Inmo24.Application.Services.Implementations;

public class DocumentosService : BaseService, IDocumentosService
{
    private readonly IStorageService _storageService;

    public DocumentosService(
        InmobiliariaContext context,
        IMapper mapper,
        IHttpContextAccessor httpContextAccessor,
        IMemoryCache cache,
        IStorageService storageService)
        : base(context, mapper, httpContextAccessor, cache)
    {
        _storageService = storageService;
    }

    public async Task<OperationResponse<DocumentoResponseDto>> SubirDocumentoAsync(Guid clienteId, IFormFile file, string tipoEntidad, string categoria, Guid? entidadReferenciaId = null)
    {
        var clienteExiste = await _context.Set<Cliente>().AnyAsync(c => c.Id == clienteId);
        if (!clienteExiste) return NotFound<DocumentoResponseDto>();

        var tenantId = GetCurrentTenantId();

        // 1. Subimos a Cloudflare R2
        var url = await _storageService.SubirDocumentoAsync(file, tenantId);

        // 2. Guardamos el registro en la BD
        var nuevoDoc = new DocumentosAdjunto
        {
            TenantId = tenantId,
            ClienteId = clienteId,
            EntidadReferenciaId = entidadReferenciaId,
            TipoEntidad = tipoEntidad.ToUpper(),
            Categoria = categoria,
            NombreArchivo = file.FileName,
            Extension = Path.GetExtension(file.FileName).ToLower(),
            TamanioBytes = file.Length,
            Url = url,
            Estado = "PENDIENTE" // Por defecto arranca pendiente
        };

        PrepareAuditableEntity(nuevoDoc, isNew: true);
        _context.Set<DocumentosAdjunto>().Add(nuevoDoc);
        await _context.SaveChangesAsync();

        return Ok(MapearADto(nuevoDoc));
    }

    public async Task<OperationResponse<bool>> EliminarDocumentoAsync(Guid id)
    {
        var doc = await _context.Set<DocumentosAdjunto>().FirstOrDefaultAsync(d => d.Id == id);
        if (doc == null) return NotFound<bool>();

        if (doc.Estado.ToUpper() == "APROBADO")
        {
            return BadRequest<bool>("No se puede eliminar un documento que ya ha sido aprobado.");
        }

        await _storageService.BorrarImagenAsync(doc.Url);
        _context.Set<DocumentosAdjunto>().Remove(doc);
        await _context.SaveChangesAsync();

        return Ok(true);
    }
    public async Task<OperationResponse<List<DocumentoResponseDto>>> ObtenerDocumentosClienteAsync(Guid clienteId)
    {
        var docs = await _context.Set<DocumentosAdjunto>()
            .Where(d => d.ClienteId == clienteId && !d.IsDeleted)
            .OrderByDescending(d => d.CreadoEl)
            .ToListAsync();

        var dtoList = docs.Select(MapearADto).ToList();

        return Ok(dtoList);
    }

    public async Task<OperationResponse<bool>> CambiarEstadoDocumentoAsync(Guid id, string nuevoEstado)
    {
        var doc = await _context.Set<DocumentosAdjunto>().FirstOrDefaultAsync(d => d.Id == id);
        if (doc == null) return NotFound<bool>();

        doc.Estado = nuevoEstado.ToUpper();

        PrepareAuditableEntity(doc, isNew: false);
        _context.Set<DocumentosAdjunto>().Update(doc);
        await _context.SaveChangesAsync();

        return Ok(true);
    }

    private DocumentoResponseDto MapearADto(DocumentosAdjunto doc)
    {
        return new DocumentoResponseDto
        {
            Id = doc.Id,
            ClienteId = doc.ClienteId,
            EntidadReferenciaId = doc.EntidadReferenciaId,
            TipoEntidad = doc.TipoEntidad,
            Categoria = doc.Categoria,
            NombreArchivo = doc.NombreArchivo,
            Extension = doc.Extension,
            TamanioBytes = doc.TamanioBytes,
            Url = doc.Url,
            Estado = doc.Estado,
            CreadoEl = doc.CreadoEl
        };
    }
}