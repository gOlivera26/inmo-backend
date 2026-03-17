using Inmo24.Application.RequestDto.Bot;
using Inmo24.Application.ResponseDto.Bot;
using Inmo24.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Inmo24.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class BotConfigController(IBotConfigService botConfigService) : BaseController
{
    private readonly IBotConfigService _botConfigService = botConfigService;

    [HttpGet]
    [ProducesResponseType(typeof(OperationResponse<BotConfigResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetConfiguracion() =>
        Return(await _botConfigService.ObtenerConfiguracionAsync());

    [HttpPut]
    [ProducesResponseType(typeof(OperationResponse<BotConfigResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateConfiguracion([FromBody] BotConfigUpdateRequestDto request) =>
        Return(await _botConfigService.ActualizarConfiguracionAsync(request));

    [AllowAnonymous]
    [HttpGet("n8n-prompt/{tenantId:guid}")]
    [ProducesResponseType(typeof(OperationResponse<BotPromptActivoDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetPromptActivoN8n(Guid tenantId) =>
        Return(await _botConfigService.ObtenerPromptActivoParaN8nAsync(tenantId));

    [AllowAnonymous]
    [HttpGet("n8n-config/{instanceName}")]
    public async Task<IActionResult> GetConfigByInstance(string instanceName) =>
    Return(await _botConfigService.ObtenerConfiguracionPorInstanciaAsync(instanceName));
}