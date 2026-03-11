using Inmo24.Application.RequestDto.Propiedades;
using Inmo24.Application.ResponseDto.Common;
using Inmo24.Application.ResponseDto.Propiedades;
using Inmo24.Application.Services.Implementations;
using Inmo24.Domain.Models;


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
    [ProducesResponseType(typeof(OperationResponse<List<Propiedades>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMisPropiedades([FromQuery] int page = 1, [FromQuery] int pageSize = 10) =>
        Return(await _propiedadesService.ObtenerMisPropiedadesAsync(page, pageSize));

    [AllowAnonymous]
    [HttpGet("catalogo-publico")]
    [ProducesResponseType(typeof(OperationResponse<List<PropiedadPublicaDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCatalogoPublico([FromQuery] int page = 1, [FromQuery] int pageSize = 20) =>
        Return(await _propiedadesService.ObtenerCatalogoPublicoAsync(page, pageSize));
}