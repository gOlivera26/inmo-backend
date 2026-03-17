using Inmo24.Application.RequestDto.Operaciones;
using Inmo24.Application.ResponseDto.Operaciones;

namespace Inmo24.API.Controllers;

[ApiController]
public class OperacionesController(IOperacionComercialService operacionService) : BaseController
{
    private readonly IOperacionComercialService _operacionService = operacionService;

    [HttpPost]
    [ProducesResponseType(typeof(OperationResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CerrarOperacion([FromBody] CerrarOperacionRequestDto request) =>
            Return(await _operacionService.CerrarOperacionAsync(request));

    [HttpGet("propiedad/{propiedadId}")]
    [ProducesResponseType(typeof(OperationResponse<OperacionDetalleResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> ObtenerOperacionPorPropiedad(Guid propiedadId) =>
        Return(await _operacionService.ObtenerOperacionPorPropiedadAsync(propiedadId));
}
