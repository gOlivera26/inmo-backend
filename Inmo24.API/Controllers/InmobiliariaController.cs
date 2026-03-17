using Inmo24.Application.RequestDto.Inmobiliaria;
using Inmo24.Application.ResponseDto.Inmobiliaria;

namespace Inmo24.API.Controllers;

[ApiController]
public class InmobiliariaController(IInmobiliariaService inmobiliariaService) : BaseController
{
    private readonly IInmobiliariaService _inmobiliariaService = inmobiliariaService;

    [HttpGet("mi-agencia")]
    [ProducesResponseType(typeof(OperationResponse<InmobiliariaResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetMiAgencia() =>
        Return(await _inmobiliariaService.ObtenerMiAgenciaAsync());

    [HttpPut("mi-agencia")]
    [ProducesResponseType(typeof(OperationResponse<InmobiliariaResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateMiAgencia([FromBody] InmobiliariaUpdateRequestDto request) =>
        Return(await _inmobiliariaService.ActualizarMiAgenciaAsync(request));

    [HttpPost("mi-agencia/logo")]
    [ProducesResponseType(typeof(OperationResponse<string>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UploadLogo(IFormFile file) =>
        Return(await _inmobiliariaService.ActualizarLogoAsync(file));
}