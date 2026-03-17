using Inmo24.Application.RequestDto.Visitas;
using Inmo24.Application.ResponseDto.Visitas;
using Inmo24.Application.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Inmo24.API.Controllers;

[ApiController]
public class VisitasController(IVisitaService visitaService) : BaseController
{
    private readonly IVisitaService _visitaService = visitaService;

    [HttpGet("mes/{anio:int}/{mes:int}")]
    [ProducesResponseType(typeof(OperationResponse<List<VisitaResponseDto>>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetVisitasMes(int anio, int mes) =>
        Return(await _visitaService.ObtenerVisitasMesAsync(anio, mes));

    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(typeof(OperationResponse<VisitaResponseDto>), StatusCodes.Status200OK)]
    public async Task<IActionResult> AgendarVisita([FromBody] VisitaCreateRequestDto request) =>
        Return(await _visitaService.AgendarVisitaAsync(request));

    [HttpPut("{id:guid}/estado")]
    [ProducesResponseType(typeof(OperationResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CambiarEstado(Guid id, [FromBody] string nuevoEstado) =>
        Return(await _visitaService.CambiarEstadoVisitaAsync(id, nuevoEstado));

    [HttpPost("cancelar-bot")]
    [AllowAnonymous] // Descomenta esto temporalmente si tu bot de n8n no está enviando el token de autenticación
    [ProducesResponseType(typeof(OperationResponse<bool>), StatusCodes.Status200OK)]
    public async Task<IActionResult> CancelarVisitaBot([FromBody] VisitaCancelRequestDto request) =>
        Return(await _visitaService.CancelarVisitaBotAsync(request));
}