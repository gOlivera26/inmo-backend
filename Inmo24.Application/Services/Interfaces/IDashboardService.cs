using Inmo24.Application.ResponseDto.Dashboard;
namespace Inmo24.Application.Services.Interfaces;

public interface IDashboardService
{
    Task<OperationResponse<DashboardResponseDto>> ObtenerResumenAsync();
}
