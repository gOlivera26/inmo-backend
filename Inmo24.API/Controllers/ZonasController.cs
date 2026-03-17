using Inmo24.Application.RequestDto.Zonas;
using Inmo24.Application.ResponseDto.Zonas;

namespace Inmo24.API.Controllers;

[ApiController]
public class ZonasController(IZonaService zonaService) : BaseController
{
    private readonly IZonaService _zonaService = zonaService;

    [HttpGet]
    [ProducesResponseType(typeof(OperationResponse<List<ZonaResponseDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetZonas() =>
        Return(await _zonaService.ObtenerZonasAsync());

    [HttpPost]
    [ProducesResponseType(typeof(OperationResponse<ZonaResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CreateZona([FromBody] ZonaCreateRequestDto request) =>
        Return(await _zonaService.CrearZonaAsync(request));

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(OperationResponse<ZonaResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateZona(int id, [FromBody] ZonaUpdateRequestDto request) =>
        Return(await _zonaService.ActualizarZonaAsync(id, request));

    [HttpDelete("{id:int}")]
    [ProducesResponseType(typeof(OperationResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> DeleteZona(int id) =>
        Return(await _zonaService.EliminarZonaAsync(id));
}