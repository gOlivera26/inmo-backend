using Inmo24.Application.ResponseDto.Documentos;


namespace Inmo24.API.Controllers;

[ApiController]
public class DocumentosController(IDocumentosService documentosService) : BaseController
{
    private readonly IDocumentosService _documentosService = documentosService;

    [HttpGet("cliente/{clienteId:guid}")]
    [ProducesResponseType(typeof(OperationResponse<List<DocumentoResponseDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDocumentosCliente(Guid clienteId) =>
        Return(await _documentosService.ObtenerDocumentosClienteAsync(clienteId));

    [HttpPost("cliente/{clienteId:guid}")]
    [ProducesResponseType(typeof(OperationResponse<DocumentoResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> SubirDocumento(
        Guid clienteId,
        [FromForm] IFormFile file,
        [FromForm] string tipoEntidad,
        [FromForm] string categoria,
        [FromForm] Guid? entidadReferenciaId) =>
        Return(await _documentosService.SubirDocumentoAsync(clienteId, file, tipoEntidad, categoria, entidadReferenciaId));

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(OperationResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteDocumento(Guid id) =>
        Return(await _documentosService.EliminarDocumentoAsync(id));

    [HttpPut("{id:guid}/estado")]
    [ProducesResponseType(typeof(OperationResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CambiarEstado(Guid id, [FromBody] string nuevoEstado) =>
        Return(await _documentosService.CambiarEstadoDocumentoAsync(id, nuevoEstado));
}