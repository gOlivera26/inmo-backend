namespace Inmo24.API.Controllers;

[ApiController]
public class PropiedadesController(IPropiedadService propiedadesService) : BaseController
{
    private readonly IPropiedadService _propiedadesService = propiedadesService;

    [HttpPost("crear-borrador")]
    [ProducesResponseType(typeof(OperationResponse<Propiedades>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CrearBorrador([FromBody] PropiedadCreateRequestDto request) =>
        Return(await _propiedadesService.CrearBorradorAsync(request));

    [HttpGet("mis-propiedades")]
    [ProducesResponseType(typeof(OperationResponse<List<PropiedadBackofficeDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMisPropiedades([FromQuery] PropiedadBackofficeFilterRequest request) =>
        Return(await _propiedadesService.ObtenerMisPropiedadesAsync(request));

    [AllowAnonymous]
    [HttpGet("catalogo-publico")]
    [ProducesResponseType(typeof(OperationResponse<List<PropiedadPublicaDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCatalogoPublico([FromQuery] PropiedadPublicaFilterRequest request) =>
            Return(await _propiedadesService.ObtenerCatalogoPublicoAsync(request));

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(OperationResponse<PropiedadDetalleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(OperationResponse<PropiedadDetalleDto>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id) =>
        Return(await _propiedadesService.ObtenerPorIdAsync(id));

    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(OperationResponse<PropiedadDetalleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(OperationResponse<PropiedadDetalleDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(OperationResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(Guid id, [FromBody] PropiedadUpdateRequestDto request) =>
            Return(await _propiedadesService.ActualizarAsync(id, request));

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(typeof(OperationResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(OperationResponse<bool>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id) =>
        Return(await _propiedadesService.EliminarAsync(id));

    [HttpPost("{id:guid}/imagenes")]
    [ProducesResponseType(typeof(OperationResponse<PropiedadImagenDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(OperationResponse<PropiedadImagenDto>), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(OperationResponse<object>), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> SubirImagen(Guid id, IFormFile file) =>
        Return(await _propiedadesService.SubirImagenAsync(id, file));

    [HttpDelete("{id:guid}/imagenes/{imagenId:guid}")]
    [ProducesResponseType(typeof(OperationResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(OperationResponse<bool>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteImagen(Guid id, Guid imagenId) =>
        Return(await _propiedadesService.EliminarImagenAsync(id, imagenId));

    [HttpPut("{id:guid}/imagenes/{imagenId:guid}/principal")]
    [ProducesResponseType(typeof(OperationResponse<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(OperationResponse<bool>), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> SetImagenPrincipal(Guid id, Guid imagenId) =>
        Return(await _propiedadesService.EstablecerImagenPrincipalAsync(id, imagenId));

    [HttpGet("{id}/historial")]
    [ProducesResponseType(typeof(OperationResponse<List<PropiedadHistorialResponseDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetHistorial(Guid id) =>
        Return(await _propiedadesService.ObtenerHistorialAsync(id));

    [AllowAnonymous]
    [HttpGet("{id:guid}/urls-imagenes")]
    [ProducesResponseType(typeof(OperationResponse<List<string>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetUrlsImagenes(Guid id) =>
        Return(await _propiedadesService.ObtenerUrlsImagenesAsync(id));

    [AllowAnonymous]
    [HttpGet("catalogo-publico/{id:guid}")]
    [ProducesResponseType(typeof(OperationResponse<PropiedadPublicaDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetDetallePublico(Guid id) =>
        Return(await _propiedadesService.ObtenerDetallePublicoAsync(id));


}