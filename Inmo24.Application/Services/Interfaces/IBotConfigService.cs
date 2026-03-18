using Inmo24.Application.RequestDto.Bot;
using Inmo24.Application.ResponseDto.Bot;
namespace Inmo24.Application.Services.Interfaces;

public interface IBotConfigService
{
    Task<OperationResponse<BotConfigResponseDto>> ObtenerConfiguracionAsync();
    Task<OperationResponse<BotConfigResponseDto>> ActualizarConfiguracionAsync(BotConfigUpdateRequestDto request);
    Task<OperationResponse<BotPromptActivoDto>> ObtenerPromptActivoParaN8nAsync(Guid tenantId);
    Task<OperationResponse<BotPromptActivoDto>> ObtenerConfiguracionPorInstanciaAsync(string instanceName);
    Task<OperationResponse<bool>> RegistrarLeadAsync(BotLeadRequestDto request);
    Task<OperationResponse<List<string>>> ObtenerImagenesPropiedadBotAsync(string codigo, Guid tenantId);
}
